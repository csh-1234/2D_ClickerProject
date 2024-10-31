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
        SkillName = "냥냥펀치!";
        SkillInfo = $"냥냥펀치를 날려 물리 피해를 입힙니다.";
        SkillLevel = 1;
        MaxSkillLevel = 1000;
        Cooldonwn = 10f;
        CurrentSkillUpdate();
    }

    //스킬 레벨업시 변경된 스탯 적용
    private void SkillLevelUp()
    {
        if (SkillLevel + 1 > MaxSkillLevel)
        {
            Debug.LogError("SkillUpdate - skillLevel의 maxLevel을 초과할 수 없습니다.");
        }
        else
        {
            SkillLevel++;
            Cooldonwn = Mathf.Max(10f, Cooldonwn - 0.01f); // cooldown은 5초까지 줄어든다. 레벨 1당 쿨타임-0.01 
        }
    }

    //current데이터 갱신, 스킬 발동전 동작하기
    private void CurrentSkillUpdate()
    {
        CurrentSkillLevel = SkillLevel;
        CurrentCoolDonwn = Cooldonwn;
    }

    private Coroutine startSkill;
    public override IEnumerator StartSkill()
    {
        print($"{SkillName}호출");
        //중첩 방지 및 재사용 금지
        if (startSkill != null)
        {
            Debug.LogError($"{SkillName} 중첩 및 재사용 시도...");
            yield break;
        }

        CurrentSkillUpdate();
        startSkill = StartCoroutine(StartActiveSkill());
        yield return null;
    }


    protected override IEnumerator StartActiveSkill()
    {
        print($"{SkillName}발동");
        SkillActive();

        while (Time.time < _cooldownEndTime)
        {
            UpdateCooldown();  // 쿨타임 UI 업데이트
            yield return null;
        }

        _isOnCooldown = false;
        UpdateCooldown();  // 쿨타임 완료 시 마지막 업데이트
        yield return null;
    }

    private void SkillActive()
    {
        Managers.Instance.Game.player.shootProjectileOnce();
        StopCoroutine(startSkill);
        startSkill = null;
    }
}
