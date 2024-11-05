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

        //SkillDic.TryGetValue(200000, out _zoomies);
        SetInfo();
        //CurrentSkillUpdate();
    }

    void SetInfo()
    {
        //TODO : ����� �����͸� �ҷ��ö� ��� ó������ ���ؾ��� �ϴ��� ����
        if (SkillDic.TryGetValue(200003, out SkillData beastEyes))
        {
            _beastEyes = beastEyes;
            SkillName = beastEyes.SkillName;
            skillType = beastEyes.SkillType;
            SkillInfo = beastEyes.SkillInfo;
            CurrentLevel = beastEyes.BaseLevel;
            MaxLevel = beastEyes.MaxLevel;
            Cooldown = beastEyes.Cooldown;
            Duration = beastEyes.Duration;
            CriRateBonus = beastEyes.CriRateBonus;
            BuffStatUpdate();
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
        Cooldown = Mathf.Max(_beastEyes.MaxCooldown, Cooldown - (0.01f * CurrentLevel));
        Duration = Mathf.Min(_beastEyes.MaxDuration, Duration + (0.01f * CurrentLevel));
        CriRateBonus += CriRateBonus * CurrentLevel;
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
