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

        //SkillDic.TryGetValue(200001, out _bakeBread);
        SetInfo();
        //CurrentSkillUpdate();
    }

    void SetInfo()
    {
        //TODO : ����� �����͸� �ҷ��ö� ��� ó������ ���ؾ��� �ϴ��� ����
        if (SkillDic.TryGetValue(200001, out SkillData BakeBread))
        {
            _bakeBread = BakeBread;
            SkillName = BakeBread.SkillName;
            skillType = BakeBread.SkillType;
            SkillInfo = BakeBread.SkillInfo;
            CurrentLevel = BakeBread.BaseLevel;
            MaxLevel = BakeBread.MaxLevel;
            Cooldown = BakeBread.Cooldown;
            Duration = BakeBread.Duration;
            DefBonus = BakeBread.DefBonus;
            HealAmount = BakeBread.HealAmount;
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
        Cooldown = Mathf.Max(_bakeBread.MaxCooldown, Cooldown - (0.01f * CurrentLevel));
        Duration = Mathf.Min(_bakeBread.MaxDuration, Duration + (0.01f * CurrentLevel));
        DefBonus += DefBonus * CurrentLevel;
        HealAmount += HealAmount * CurrentLevel;
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
