using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DataManager;

public class Skill_FatalStrike : Skill
{
    //��ü ��ų���� ����
    private SkillData _fatalStrike;

    //�����ϴ� ���ݸ� ����
    private Stats _buffStat = new Stats();

    private string BuffId { get { return $"FatalStrike_{CurrentLevel}"; } }

    protected override void Awake()
    {
        base.Awake();
        SetInfo();
    }
    public override string GetCurrentSkillInfo()
    {
        if (_fatalStrike == null) return string.Empty;

        try
        {
            return _data.GetFormattedInfo(
            Duration,          // {0}
            CriDamageBonus,     // {1}
            Cooldown           // {2}
        );
        }
        catch (Exception e)
        {
            Debug.LogError($"Error formatting skill info: {e.Message}");
            return _fatalStrike.SkillInfo;  // ������ ���н� �⺻ �ؽ�Ʈ ��ȯ
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
        if (SkillDic.TryGetValue(200004, out SkillData FatalStrike))
        {
            _fatalStrike = FatalStrike;
            SkillName = FatalStrike.SkillName;
            skillType = FatalStrike.SkillType;
            MaxLevel = FatalStrike.MaxLevel;

            if (CurrentLevel == 0)
            {
                CurrentLevel = FatalStrike.BaseLevel;
                Cooldown = FatalStrike.Cooldown;
                Duration = FatalStrike.Duration;
                CriDamageBonus = FatalStrike.CriDamageBonus;
            }
            SkillInfo = FatalStrike.SkillInfo;
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
        Cooldown = Mathf.Max(_fatalStrike.MaxCooldown, Cooldown - 0.01f);
        Duration = Mathf.Min(_fatalStrike.MaxDuration, Duration + 0.01f);
        CriDamageBonus++;
        BuffStatUpdate();

        // ������ Ȱ��ȭ ���¿��ٸ� ���ο� �������� �ٽ� ����
        if (_isBuffActive && _currentTarget != null)
        {
            _currentTarget.ApplyBuff(BuffId, _buffStat, Duration);
        }
    }

    public override void UpdateSkillByLoadedLevel()
    {
        Cooldown = Mathf.Max(_fatalStrike.MaxCooldown, Cooldown - (0.01f * CurrentLevel));
        Duration = Mathf.Min(_fatalStrike.MaxDuration, Duration + (0.01f * CurrentLevel));
        CriDamageBonus += CriDamageBonus * (CurrentLevel);
        BuffStatUpdate();
    }


    private void BuffStatUpdate()
    {
        _buffStat.CriticalDamage = CriDamageBonus;
    }

    private Coroutine startSkill;
    public override IEnumerator StartSkill()
    {
        print($"{SkillName}ȣ��");
        //��ø ���� �� ���� ����
        if (startSkill != null)
        {
            Debug.LogError($"{SkillName} ��ø �� ���� �õ�...");
            yield break;
        }

        startSkill = StartCoroutine(StartBuffSkill());
        yield return null;
    }

    protected override IEnumerator StartBuffSkill()
    {
        Player player = Managers.Instance.Game.player;
        player._isCritical = true;
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
