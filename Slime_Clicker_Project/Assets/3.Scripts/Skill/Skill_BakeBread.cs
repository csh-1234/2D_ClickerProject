using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DataManager;

public class Skill_BakeBread : Skill
{
    //��ü ��ų���� ����
    private SkillData _bakeBread;

    //�����ϴ� ���ݸ� ����
    private Stats _buffStat = new Stats();

    private string BuffId { get { return $"BakeBread_{CurrentLevel}"; } }

    protected override void Awake()
    {
        base.Awake();
        SetInfo();
    }

    public override string GetCurrentSkillInfo()
    {
        if (_bakeBread == null) return string.Empty;
        try
        {
            return _data.GetFormattedInfo(
            Duration,          // {0}
            DefBonus,     // {1}
            HealAmount,          // {2}
            Cooldown           // {3}
        );
        }
        catch (Exception e)
        {
            Debug.LogError($"Error formatting skill info: {e.Message}");
            return _bakeBread.SkillInfo;  // ������ ���н� �⺻ �ؽ�Ʈ ��ȯ
        }
    }

    void SetInfo()
    {
        if (SkillDic == null)
        {
            Debug.LogError("SkillDic is not initialized!");
            return;
        }
        //TODO : ����� �����͸� �ҷ��ö� ��� ó������ ���ؾ��� �ϴ��� ����
        if (SkillDic.TryGetValue(200001, out SkillData BakeBread))
        {
            _bakeBread = BakeBread;
            SkillName = BakeBread.SkillName;
            skillType = BakeBread.SkillType;
            MaxLevel = BakeBread.MaxLevel;
            DataId = BakeBread.DataId;
            if (CurrentLevel == 0)
            {
                CurrentLevel = BakeBread.BaseLevel;
                Cooldown = BakeBread.Cooldown;
                Duration = BakeBread.Duration;
                HealAmount = BakeBread.HealAmount;
                DefBonus = BakeBread.DefBonus;
            }
            //SkillInfo = GetCurrentSkillInfo();
            SkillInfo = BakeBread.SkillInfo;
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
        Cooldown = Mathf.Max(_bakeBread.MaxCooldown, Cooldown - 0.01f);
        Duration = Mathf.Min(_bakeBread.MaxDuration, Duration + 0.01f);
        DefBonus++;
        HealAmount++;
        BuffStatUpdate();

        // ������ Ȱ��ȭ ���¿��ٸ� ���ο� �������� �ٽ� ����
        if (_isBuffActive && _currentTarget != null)
        {
            _currentTarget.ApplyBuff(BuffId, _buffStat, Duration);
        }
    }

    public override void UpdateSkillByLoadedLevel()
    {
        float baseCooldown = _bakeBread.Cooldown;
        float baseDuration = _bakeBread.Duration;
        int baseDefBonus = _bakeBread.DefBonus;
        float baseHealAmount = _bakeBread.HealAmount;

        Cooldown = Mathf.Max(_bakeBread.MaxCooldown, Cooldown - (0.01f * CurrentLevel));
        Duration = Mathf.Min(_bakeBread.MaxDuration, Duration + (0.01f * CurrentLevel));
        DefBonus += DefBonus * (CurrentLevel);
        HealAmount += HealAmount * (CurrentLevel);
        BuffStatUpdate();
    }

    private void BuffStatUpdate()
    {
        _buffStat.Defense = DefBonus;
    }

    private Coroutine startSkill;
    public override IEnumerator StartSkill()
    {
        print("�Ļ�����");
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
        Player player = Managers.Instance.Game.player;
        _currentTarget = player;
        if (player != null)
        {
            _isOnCooldown = true;
            _cooldownEndTime = Time.time + Cooldown;
            _isBuffActive = true;
            // ���� ���� �� ���� �̺�Ʈ �߻�
            player.OnHeal(HealAmount);
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
    }
    
}
