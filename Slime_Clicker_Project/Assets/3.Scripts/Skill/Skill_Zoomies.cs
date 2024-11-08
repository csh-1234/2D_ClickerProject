using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static DataManager;
using static Enums;

public class Skill_Zoomies : Skill
{
    //전체 스킬정보 관리
    private SkillData _zoomies;

    //증가하는 스텟만 관리
    private Stats _buffStat= new Stats();

    private string BuffId { get { return $"Zoomies_{CurrentLevel}"; } }

    protected override void Awake()
    {
        base.Awake();
        SetInfo();
    }
    public override string GetCurrentSkillInfo()
    {
        if (_zoomies == null) return string.Empty;

        try
        {
            return _data.GetFormattedInfo(
            Duration,          // {0}
            AtkBonus,     // {1}
            AtkSpeedBonus,          // {2}
            Cooldown           // {3}
        );
        }
        catch (Exception e)
        {
            Debug.LogError($"Error formatting skill info: {e.Message}");
            return _zoomies.SkillInfo;  // 포맷팅 실패시 기본 텍스트 반환
        }
    }
    public void SetInfo()
    {
        //TODO : 저장된 데이터를 불러올때 어떻게 처리할지 정해야함 일단은 보류
        if (SkillDic.TryGetValue(200000, out SkillData Zoomies))
        {
            _zoomies = Zoomies;
            SkillName = Zoomies.SkillName;
            skillType = Zoomies.SkillType;
            MaxLevel = Zoomies.MaxLevel;
            DataId = Zoomies.DataId;

            if (CurrentLevel == 0)
            {
                CurrentLevel = Zoomies.BaseLevel;
                Cooldown = Zoomies.Cooldown;
                Duration = Zoomies.Duration;
                AtkBonus = Zoomies.AtkBonus;
                AtkSpeedBonus = Zoomies.AtkSpeedBonus;
            }
            SkillInfo = Zoomies.SkillInfo;
        }
    }

    private bool _isBuffActive = false;  // 버프 활성화 상태 추적
    private Player _currentTarget = null; // 현재 버프가 적용된 대상
    public void SkillLevelUp()
    {
        if (CurrentLevel + 1 > MaxLevel)
        {
            Debug.LogError("SkillUpdate - skillLevel이 maxLevel을 초과할 수 없습니다.");
            return;
        }

        // 현재 적용 중인 버프가 있다면 제거
        if (_isBuffActive && _currentTarget != null)
        {
            _currentTarget.RemoveBuff(BuffId);
        }

        CurrentLevel++;
        Cooldown = Mathf.Max(_zoomies.MaxCooldown, Cooldown - 0.01f);
        Duration = Mathf.Min(_zoomies.MaxDuration, Duration + 0.01f);
        AtkBonus++;
        AtkSpeedBonus+=0.01f;
        BuffStatUpdate();

        // 버프가 활성화 상태였다면 새로운 스탯으로 다시 적용
        if (_isBuffActive && _currentTarget != null)
        {
            _currentTarget.ApplyBuff(BuffId, _buffStat, Duration);
        }
    }

    public override void UpdateSkillByLoadedLevel()
    {
        float baseCooldown = _zoomies.Cooldown;
        float baseDuration = _zoomies.Duration;
        int baseAtkBonus = _zoomies.AtkBonus;
        float baseAtkSpeedBonus = _zoomies.AtkSpeedBonus;

        Cooldown = Mathf.Max(_zoomies.MaxCooldown, Cooldown - (0.01f * CurrentLevel-1));
        Duration = Mathf.Min(_zoomies.MaxDuration, Duration + (0.01f * CurrentLevel-1));
        AtkBonus += AtkBonus * (CurrentLevel -1);
        AtkSpeedBonus += AtkSpeedBonus * CurrentLevel;
        BuffStatUpdate();
    }


    private void BuffStatUpdate()
    {
        _buffStat.Attack = AtkBonus;
        _buffStat.AttackSpeed = AtkSpeedBonus;
        Debug.Log($"BuffStatUpdate cooldown{Cooldown}");
        Debug.Log($"BuffStatUpdate Duration{Duration}");
    }


    private Coroutine startSkill;
    public override IEnumerator StartSkill()
    {
        print("우다다다!");
        //중첩 방지 및 재사용 금지
        if (startSkill != null)
        {
            Debug.LogError($"{SkillName} 중첩 및 재사용 시도...");
            yield break;
        }
        //CurrentSkillUpdate();
        startSkill = StartCoroutine(StartBuffSkill());
        yield return null;
    }

    protected override IEnumerator StartBuffSkill()
    {
        Debug.Log($"zoomies 쿨타임 : {Cooldown}");
        Player player = Managers.Instance.Game.player;
        _currentTarget = player;
        if (player != null)
        {
            _isOnCooldown = true;
            _cooldownEndTime = Time.time + Cooldown;
            _isBuffActive = true;
            // 버프 적용 및 시작 이벤트 발생
            player.ApplyBuff(BuffId, _buffStat, Duration);
            InvokeBuffStart();
            // 버프 지속 시간 동안 대기
            float buffEndTime = Time.time + Duration;
            while (Time.time < buffEndTime)
            {
                UpdateCooldown();
                yield return null;
            }
            print("버프시작");

            // 버프 제거 및 종료 이벤트 발생
            _isBuffActive = false;
            player.RemoveBuff(BuffId);
            _currentTarget = null;
            InvokeBuffEnd();
            print("버프종료");

            // 쿨다운 시간 동안 대기
            while (Time.time < _cooldownEndTime)
            {
                UpdateCooldown();
                yield return null;
            }

            _isOnCooldown = false;
            UpdateCooldown();

            StopCoroutine(startSkill);
            startSkill = null;
            print("스킬종료");
        }

        yield return null;
    }
}
