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
         
        //SkillDic.TryGetValue(200000, out _zoomies);
        SetInfo();
        //CurrentSkillUpdate();
    }

    void SetInfo()
    {
        //TODO : ����� �����͸� �ҷ��ö� ��� ó������ ���ؾ��� �ϴ��� ����
        if (SkillDic.TryGetValue(200000, out SkillData Zoomies))
        {
            _zoomies = Zoomies;
            SkillName = Zoomies.SkillName;
            skillType = Zoomies.SkillType;
            SkillInfo = Zoomies.SkillInfo;
            CurrentLevel = Zoomies.BaseLevel;
            MaxLevel = Zoomies.MaxLevel;
            Cooldown = Zoomies.Cooldown;
            Duration = Zoomies.Duration;
            AtkBonus = Zoomies.AtkBonus;
            AtkSpeedBonus = Zoomies.AtkSpeedBonus;
            BuffStatUpdate();
        }
    }

    //��ų �������� ����� ���� ����
    //public void SkillLevelUp()
    //{
    //    if (CurrentLevel + 1 > MaxLevel)
    //    {
    //        Debug.LogError("SkillUpdate - skillLevel�� maxLevel�� �ʰ��� �� �����ϴ�.");
    //    }
    //    else
    //    {
    //        CurrentLevel++;
    //        Cooldown = Mathf.Max(_zoomies.MaxCooldown, Cooldown - 0.01f); // cooldown�� 10�ʱ��� �پ���. ���� 1�� ��Ÿ��-0.01 
    //        Duration = Mathf.Min(_zoomies.MaxDuration, Duration + 0.01f); // duration�� 10�ʱ��� �þ��. ���� 1�� ��Ÿ��-0.01 
    //        AtkBonus++;         // ���� 1�� ���ݷ�+1
    //        AtkSpeedBonus++;     // ���� 1�� ����+1
    //        BuffStatUpdate();
    //    }
    //}
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
        AtkSpeedBonus++;
        BuffStatUpdate();

        // ������ Ȱ��ȭ ���¿��ٸ� ���ο� �������� �ٽ� ����
        if (_isBuffActive && _currentTarget != null)
        {
            _currentTarget.ApplyBuff(BuffId, _buffStat, Duration);
        }
    }
    private void BuffStatUpdate()
    {
        _buffStat.Attack = AtkBonus;
        _buffStat.AttackSpeed = AtkSpeedBonus;
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
