using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DataManager;

public class Skill_EatChur : Skill
{
    //전체 스킬정보 관리
    private SkillData _eatChur;

    //증가하는 스텟만 관리
    private Stats _buffStat = new Stats();

    private string BuffId { get { return $"EatChur_{CurrentLevel}"; } }

    protected override void Awake()
    {
        base.Awake();

        //SkillDic.TryGetValue(200000, out _zoomies);
        SetInfo();
        //CurrentSkillUpdate();
    }

    void SetInfo()
    {
        //TODO : 저장된 데이터를 불러올때 어떻게 처리할지 정해야함 일단은 보류
        if (SkillDic.TryGetValue(200002, out SkillData EatChur))
        {
            _eatChur = EatChur;
            SkillName = EatChur.SkillName;
            skillType = EatChur.SkillType;
            SkillInfo = EatChur.SkillInfo;
            CurrentLevel = EatChur.BaseLevel;
            MaxLevel = EatChur.MaxLevel;
            Cooldown = EatChur.Cooldown;
            BuffStatUpdate();
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
        Cooldown = Mathf.Max(_eatChur.MaxCooldown, Cooldown - 0.01f);
        Duration = Mathf.Min(_eatChur.MaxDuration, Duration + 0.01f);
        BuffStatUpdate();

        // 버프가 활성화 상태였다면 새로운 스탯으로 다시 적용
        if (_isBuffActive && _currentTarget != null)
        {
            _currentTarget.ApplyBuff(BuffId, _buffStat, Duration);
        }
    }

    public override void UpdateSkillByLoadedLevel()
    {
        Cooldown = Mathf.Max(_eatChur.MaxCooldown, Cooldown - (0.01f * CurrentLevel));
        Duration = Mathf.Min(_eatChur.MaxDuration, Duration + (0.01f * CurrentLevel));
    }


    private void BuffStatUpdate()
    {

    }


    private Coroutine startSkill;
    public override IEnumerator StartSkill()
    {
        print("츄르먹기");
        //중첩 방지 및 재사용 금지
        if (startSkill != null)
        {
            Debug.LogError($"{SkillName} 중첩 및 재사용 시도...");
            yield break;
        }

        startSkill = StartCoroutine(StartBuffSkill());
        yield return null;
    }

    protected override IEnumerator StartBuffSkill()
    {
        Player player = Managers.Instance.Game.player;
        _currentTarget = player;
        if (player != null)
        {
            _isOnCooldown = true;
            _cooldownEndTime = Time.time + Cooldown;
            _isBuffActive = true;
            // 버프 적용 및 시작 이벤트 발생
            player.ApplyBuff(BuffId, _buffStat, Duration);
            BuffActivate();
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

    private void BuffActivate()
    {
        if (Managers.Instance.Game.player != null)
        {

            List<Skill> _skillList = Managers.Instance.Game.player.SkillList;
            foreach (Skill skill in _skillList)
            {
                if (skill.name != "Skill_EatChur")
                {
                    skill.InitCooldown();
                }
            }
        }
        else
        {
            Debug.LogError("SkillActivate - Cant find GameManager Player");
        }
    }

    //private void BuffDeActivate()
    //{
    //    if (Managers.Instance.Game.player != null)
    //    {
    //        var _player = Managers.Instance.Game.player;
    //        _player.Hp -= HpBonus;
    //        _player.Atk -= AtkBonus;
    //        _player.Def -= DefBonus;
    //        _player.AttackSpeed -= AtkSpeedBonus;
    //        _player.CriRate -= CriRateBonus;
    //        _player.CriDamage -= CriDamageBonus;
    //        _player.MoveSpeed -= MoveSpeedBonus;
    //    }
    //    else
    //    {
    //        Debug.LogError("SkillActivate - Cant find GameManager Player");
    //    }
    //}
}
