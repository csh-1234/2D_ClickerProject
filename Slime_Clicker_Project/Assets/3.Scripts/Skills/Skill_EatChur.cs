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

        //SkillDic.TryGetValue(200000, out _zoomies);
        SetInfo();
        //CurrentSkillUpdate();
    }

    void SetInfo()
    {
        //TODO : ����� �����͸� �ҷ��ö� ��� ó������ ���ؾ��� �ϴ��� ����
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
        Duration = Mathf.Min(_eatChur.MaxDuration, Duration + 0.01f);
        BuffStatUpdate();

        // ������ Ȱ��ȭ ���¿��ٸ� ���ο� �������� �ٽ� ����
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
