using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_EatChur : Skill
{
    protected override void Awake()
    {
        base.Awake();
        skillType = Enums.SkillType.Buff;
        SkillName = "츄르먹기";
        SkillInfo = $"모든 스킬의 쿨타임을 초기화합니다.";
        SkillLevel = 1;
        MaxSkillLevel = 1000;
        Cooldonwn = 30f;

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
            Cooldonwn = Mathf.Max(15f, Cooldonwn - (SkillLevel * 0.01f)); // cooldown은 15초까지 줄어든다. 레벨 1당 쿨타임-0.01 
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
        print("츄르먹기");
        //중첩 방지 및 재사용 금지
        if (startSkill != null)
        {
            Debug.LogError($"{SkillName} 중첩 및 재사용 시도...");
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
        print("버프켜짐");
        //float buffEndTime = Time.time + Duration;
        //while (Time.time < buffEndTime)
        //{
        //    UpdateCooldown();  // 쿨타임 UI 업데이트
        //    yield return null;
        //}

        //BuffDeActivate();
        print("버프끝남");
        while (Time.time < _cooldownEndTime)
        {
            UpdateCooldown();  // 쿨타임 UI 업데이트
            yield return null;
        }

        _isOnCooldown = false;
        UpdateCooldown();  // 쿨타임 완료 시 마지막 업데이트


        //버프스킬 활성화 중에 중첩 및 재사용 방지
        StopCoroutine(startSkill);
        startSkill = null;
        print("버프제거");
        yield return null;
    }

    private void BuffActivate()
    {
        if (Managers.Instance.Game.player != null)
        {
            
            List<Skill> _skillList = Managers.Instance.Game.player.SkillList;
            foreach (Skill skill in _skillList)
            {
                if(skill.name != "Skill_EatChur")
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

    private void BuffDeActivate()
    {
        if (Managers.Instance.Game.player != null)
        {
            var _player = Managers.Instance.Game.player;
            _player.Hp -= HpBonus;
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
