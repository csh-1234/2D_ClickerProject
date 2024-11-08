using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DataManager;

public class Skill_BeastEyes : Skill
{
    //��ü ��ų���� ����
    private SkillData _beastEyes;

    //�����ϴ� ���ݸ� ����
    private Stats _buffStat = new Stats();

    private string BuffId { get { return $"BeastEyes_{CurrentLevel}"; } }

    protected override void Awake()
    {
        base.Awake();
        SetInfo();
    }

    public override string GetCurrentSkillInfo()
    {
        if (_beastEyes == null) return string.Empty;

        try
        {
            return _data.GetFormattedInfo(
            Duration,          // {0}
            CriRateBonus,     // {1}
            Cooldown           // {2}
        );
        }
        catch (Exception e)
        {
            Debug.LogError($"Error formatting skill info: {e.Message}");
            return _beastEyes.SkillInfo;  // ������ ���н� �⺻ �ؽ�Ʈ ��ȯ
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
        if (SkillDic.TryGetValue(200003, out SkillData beastEyes))
        {
            _beastEyes = beastEyes;
            SkillName = beastEyes.SkillName;
            skillType = beastEyes.SkillType;
            MaxLevel = beastEyes.MaxLevel;
            DataId = beastEyes.DataId;

            if (CurrentLevel == 0)
            {
                CurrentLevel = beastEyes.BaseLevel;
                Cooldown = beastEyes.Cooldown;
                Duration = beastEyes.Duration;
                CriRateBonus = beastEyes.CriRateBonus;
            }
            SkillInfo = beastEyes.SkillInfo;

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
        Cooldown = Mathf.Max(_beastEyes.MaxCooldown, Cooldown - 0.01f);
        Duration = Mathf.Min(_beastEyes.MaxDuration, Duration + 0.01f);
        CriRateBonus = Mathf.Max(50, CriRateBonus + 0.01f);
        BuffStatUpdate();

        // ������ Ȱ��ȭ ���¿��ٸ� ���ο� �������� �ٽ� ����
        if (_isBuffActive && _currentTarget != null)
        {
            _currentTarget.ApplyBuff(BuffId, _buffStat, Duration);
        }
    }

    public override void UpdateSkillByLoadedLevel()
    {
        Cooldown = Mathf.Max(_beastEyes.MaxCooldown, Cooldown - (0.01f * CurrentLevel-1));
        Duration = Mathf.Min(_beastEyes.MaxDuration, Duration + (0.01f * CurrentLevel-1));
        CriRateBonus += CriRateBonus * (CurrentLevel-1);
        BuffStatUpdate();
    }

    private void BuffStatUpdate()
    {
        _buffStat.CriticalRate = CriRateBonus;
    }


    private Coroutine startSkill;
    public override IEnumerator StartSkill()
    {
        print("�ͼ��� ����");
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
