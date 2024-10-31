using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Skill_Zoomies : Skill
{
    protected override void Awake()
    {
        base.Awake();
        skillType = Enums.SkillType.Buff;
        SkillName = "��ٴٴ�";
        SkillInfo = $"{Duration}�� ���� ���ݷ�(+{AtkBonus})�� ���ݼӵ�(+{AtkSpeedBonus})�� �����մϴ�.";
        SkillLevel = 1;
        MaxSkillLevel = 1000;

        Cooldonwn = 15f;
        Duration = 5f;
        AtkBonus = 10;
        AtkSpeedBonus = 10f;

        CurrentSkillUpdate();
    }

    //��ų �������� ����� ���� ����
    private void SkillLevelUp()
    {
        if(SkillLevel + 1 > MaxSkillLevel)
        {
            Debug.LogError("SkillUpdate - skillLevel�� maxLevel�� �ʰ��� �� �����ϴ�.");
        }
        else
        {
            SkillLevel++;
            Cooldonwn = Mathf.Max(10f, Cooldonwn - 0.01f); // cooldown�� 10�ʱ��� �پ���. ���� 1�� ��Ÿ��-0.01 
            Duration = Mathf.Min(10f, Duration +0.01f); // duration�� 10�ʱ��� �þ��. ���� 1�� ��Ÿ��-0.01 
            AtkBonus++;     // ���� 1�� ���ݷ�+1
            AtkSpeedBonus++;    // ���� 1�� ����+1
        }
    }

    //current������ ����, ��ų �ߵ��� �����ϱ�
    private void CurrentSkillUpdate()
    {
        CurrentSkillLevel = SkillLevel;
        CurrentCoolDonwn = Cooldonwn;
        CurrentDuration = Duration;
        CurrentAtkBonus = AtkBonus;
        CurrentAtkSpeedBonus = AtkSpeedBonus;
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

        CurrentSkillUpdate();
        startSkill = StartCoroutine(StartBuffSkill());
        yield return null;
    }

    protected override IEnumerator StartBuffSkill()
    {
        _isOnCooldown = true;
        _cooldownEndTime = Time.time + Cooldonwn;

        BuffActivate();
        print("��������");
        float buffEndTime = Time.time + Duration;
        while (Time.time < buffEndTime)
        {
            UpdateCooldown();  // ��Ÿ�� UI ������Ʈ
            yield return null;
        }
        
        //yield return new WaitForSeconds(Duration);
        BuffDeActivate();
        print("��������");

        while (Time.time < _cooldownEndTime)
        {
            UpdateCooldown();  // ��Ÿ�� UI ������Ʈ
            yield return null;
        }

        _isOnCooldown = false;
        UpdateCooldown();  // ��Ÿ�� �Ϸ� �� ������ ������Ʈ


        //������ų Ȱ��ȭ �߿� ��ø �� ���� ����
        StopCoroutine(startSkill);
        startSkill = null;
        print("��������");
        yield return null;
    }

    private void BuffActivate()
    {
        if(Managers.Instance.Game.player != null)
        {
            var _player = Managers.Instance.Game.player;
            _player.Hp += HpBonus;
            _player.MaxHp += HpBonus;
            _player.Atk += AtkBonus;
            _player.Def += DefBonus;
            _player.AttackSpeed += AtkSpeedBonus;
            _player.CriRate += CriRateBonus;
            _player.CriDamage += CriDamageBonus;
            _player.MoveSpeed += MoveSpeedBonus;
        }
        else
        {
            Debug.LogError("SkillActivate - Cant find GameManager Player");
        }
    }

    private void BuffDeActivate()
    {
        if (Managers.Instance.Game.player != null)
        {
            var _player = Managers.Instance.Game.player;
            _player.Hp -= HpBonus;
            _player.MaxHp -= HpBonus;
            _player.Atk -= AtkBonus;
            _player.Def -= DefBonus;
            _player.AttackSpeed -= AtkSpeedBonus;
            _player.CriRate -= CriRateBonus;
            _player.CriDamage -= CriDamageBonus;
            _player.MoveSpeed -= MoveSpeedBonus;
        }
        else
        {
            Debug.LogError("SkillActivate - Cant find GameManager Player");
        }
    }
}
