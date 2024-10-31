using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class UI_SkillButton : RootUI
{
    public Image cooldownFill;
    public Skill skill;

    protected override void Awake()
    {
        base.Awake();
        if (cooldownFill == null)
        {
            cooldownFill = GetComponent<Image>();
        }
    }

    private void Start()
    {
        if (skill != null)
        {
            skill.OnCooldownUpdate += UpdateCooldownUI;
        }
    }

    private void OnDisable()
    {
        if (skill != null)
        {
            skill.OnCooldownUpdate -= UpdateCooldownUI;
        }
    }

    private void UpdateCooldownUI(float ratio)
    {
        if (cooldownFill != null)
        {
            cooldownFill.fillAmount = 1f - ratio;
        }
    }

    // 스킬 참조 설정을 위한 public 메서드
    public void SetSkill(Skill newSkill)
    {
        if (skill != null)
        {
            skill.OnCooldownUpdate -= UpdateCooldownUI;
        }

        skill = newSkill;

        if (skill != null)
        {
            skill.OnCooldownUpdate += UpdateCooldownUI;
        }
    }
}
