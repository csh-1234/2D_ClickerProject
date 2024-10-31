using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Skill_CatCatPunch : Skill
{
    protected override void Awake()
    {
        base.Awake();
        skillType = Enums.SkillType.Buff;
        SkillName = "�ɳ���ġ!";
        SkillInfo = $"�ɳ���ġ�� ���� ���� ���ظ� �����ϴ�.";
        SkillLevel = 1;
        MaxSkillLevel = 1000;
        Cooldonwn = 10f;
        CurrentSkillUpdate();
    }

    //��ų �������� ����� ���� ����
    private void SkillLevelUp()
    {
        if (SkillLevel + 1 > MaxSkillLevel)
        {
            Debug.LogError("SkillUpdate - skillLevel�� maxLevel�� �ʰ��� �� �����ϴ�.");
        }
        else
        {
            SkillLevel++;
            Cooldonwn = Mathf.Max(10f, Cooldonwn - 0.01f); // cooldown�� 5�ʱ��� �پ���. ���� 1�� ��Ÿ��-0.01 
        }
    }

    //current������ ����, ��ų �ߵ��� �����ϱ�
    private void CurrentSkillUpdate()
    {
        CurrentSkillLevel = SkillLevel;
        CurrentCoolDonwn = Cooldonwn;
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

        CurrentSkillUpdate();
        startSkill = StartCoroutine(StartActiveSkill());
        yield return null;
    }


    protected override IEnumerator StartActiveSkill()
    {
        print($"{SkillName}�ߵ�");
        SkillActive();

        while (Time.time < _cooldownEndTime)
        {
            UpdateCooldown();  // ��Ÿ�� UI ������Ʈ
            yield return null;
        }

        _isOnCooldown = false;
        UpdateCooldown();  // ��Ÿ�� �Ϸ� �� ������ ������Ʈ
        yield return null;
    }

    private void SkillActive()
    {
        Managers.Instance.Game.player.shootProjectileOnce();
        StopCoroutine(startSkill);
        startSkill = null;
    }
}
