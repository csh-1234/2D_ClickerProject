using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static DataManager;
using static Enums;

public class Skill_Zoomies : Skill
{
    //��ü ��ų���� ����
    private SkillData _zoomies;

    //�����ϴ� ���ݸ� ����
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
            return _zoomies.SkillInfo;  // ������ ���н� �⺻ �ؽ�Ʈ ��ȯ
        }
    }
    public void SetInfo()
    {
        //TODO : ����� �����͸� �ҷ��ö� ��� ó������ ���ؾ��� �ϴ��� ����
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

    private bool _isBuffActive = false;  // ���� Ȱ��ȭ ���� ����
    private Player _currentTarget = null; // ���� ������ ����� ���
    public void SkillLevelUp()
    {
        if (CurrentLevel + 1 > MaxLevel)
        {
            Debug.LogError("SkillUpdate - skillLevel�� maxLevel�� �ʰ��� �� �����ϴ�.");
            return;
        }

        // ���� ���� ���� ������ �ִٸ� ����
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

        // ������ Ȱ��ȭ ���¿��ٸ� ���ο� �������� �ٽ� ����
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
        print("��ٴٴ�!");
        //��ø ���� �� ���� ����
        if (startSkill != null)
        {
            Debug.LogError($"{SkillName} ��ø �� ���� �õ�...");
            yield break;
        }
        //CurrentSkillUpdate();
        startSkill = StartCoroutine(StartBuffSkill());
        yield return null;
    }

    protected override IEnumerator StartBuffSkill()
    {
        Debug.Log($"zoomies ��Ÿ�� : {Cooldown}");
        Player player = Managers.Instance.Game.player;
        _currentTarget = player;
        if (player != null)
        {
            _isOnCooldown = true;
            _cooldownEndTime = Time.time + Cooldown;
            _isBuffActive = true;
            // ���� ���� �� ���� �̺�Ʈ �߻�
            player.ApplyBuff(BuffId, _buffStat, Duration);
            InvokeBuffStart();
            // ���� ���� �ð� ���� ���
            float buffEndTime = Time.time + Duration;
            while (Time.time < buffEndTime)
            {
                UpdateCooldown();
                yield return null;
            }
            print("��������");

            // ���� ���� �� ���� �̺�Ʈ �߻�
            _isBuffActive = false;
            player.RemoveBuff(BuffId);
            _currentTarget = null;
            InvokeBuffEnd();
            print("��������");

            // ��ٿ� �ð� ���� ���
            while (Time.time < _cooldownEndTime)
            {
                UpdateCooldown();
                yield return null;
            }

            _isOnCooldown = false;
            UpdateCooldown();

            StopCoroutine(startSkill);
            startSkill = null;
            print("��ų����");
        }

        yield return null;
    }
}
