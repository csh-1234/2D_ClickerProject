using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DataManager;

public class Skill_EatChur : Skill
{
    //��ü ��ų���� ����
    private SkillData _eatChur;

    //�����ϴ� ���ݸ� ����
    private Stats _buffStat = new Stats();

    private string BuffId { get { return $"EatChur_{CurrentLevel}"; } }

    protected override void Awake()
    {
        base.Awake();

        SetInfo();
    }

    public override string GetCurrentSkillInfo()
    {
        if (_eatChur == null) return string.Empty;

        try
        {
            return _data.GetFormattedInfo(
            Cooldown           // {0}
        );
        }
        catch (Exception e)
        {
            Debug.LogError($"Error formatting skill info: {e.Message}");
            return _eatChur.SkillInfo;  // ������ ���н� �⺻ �ؽ�Ʈ ��ȯ
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
        if (SkillDic.TryGetValue(200002, out SkillData EatChur))
        {
            _eatChur = EatChur;
            SkillName = EatChur.SkillName;
            skillType = EatChur.SkillType;
            
            MaxLevel = EatChur.MaxLevel;
            DataId = EatChur.DataId;

            if (CurrentLevel == 0)
            {
                CurrentLevel = EatChur.BaseLevel;
                Cooldown = EatChur.Cooldown;
                Duration = EatChur.Duration;
            }
            SkillInfo = EatChur.SkillInfo;
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
        Cooldown = Mathf.Max(_eatChur.MaxCooldown, Cooldown - 0.01f);
        BuffStatUpdate();

        // ������ Ȱ��ȭ ���¿��ٸ� ���ο� �������� �ٽ� ����
        if (_isBuffActive && _currentTarget != null)
        {
            _currentTarget.ApplyBuff(BuffId, _buffStat, Duration);
        }
    }

    public override void UpdateSkillByLoadedLevel()
    {
        Cooldown = Mathf.Max(_eatChur.MaxCooldown, Cooldown - (0.01f * CurrentLevel-1));
        BuffStatUpdate();
    }


    private void BuffStatUpdate()
    {

    }


    private Coroutine startSkill;
    public override IEnumerator StartSkill()
    {
        print("�򸣸Ա�");
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
            BuffActivate();
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

}
