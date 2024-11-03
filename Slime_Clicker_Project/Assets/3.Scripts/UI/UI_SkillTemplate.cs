using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using static Enums;
using static UnityEditor.Progress;

public class UI_SkillTemplate : RootUI
{
    protected override void Awake()
    {
        base.Awake();
    }

    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button upgradeButton;

    public Action OnSkillLevelChanged;

    private Skill _skill;
    public void SetItem(Skill skill)
    {
        _skill = skill;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (_skill == null) return;
        iconImage.sprite = Resources.Load<Sprite>(_skill.SpriteName);
        levelText.text = $"LV.{_skill.CurrentLevel}";
        nameText.text = _skill.SkillName;
        infoText.text = _skill.SkillInfo;
        costText.text = _skill.UpgradeCost.ToString();
    }

    public void OnUpgradeClick2()
    {
        print("업글");
        if (_skill.TryUpgrade(Managers.Instance.Currency.GetCurrentGold()))
        {
            Managers.Instance.Currency.RemoveGold(_skill.UpgradeCost);
            
            UpdateUI();
        }
    }

    public void OnUpgradeClick()
    {
        if (_skill.TryUpgrade(Managers.Instance.Currency.GetCurrentGold()))
        {
            Managers.Instance.Currency.RemoveGold(_skill.UpgradeCost);

            // 스킬 타입에 따라 적절한 레벨업 메서드 호출
            if (_skill is Skill_Zoomies zoomiesSkill)
            {
                zoomiesSkill.SkillLevelUp();
            }
            // 스킬 타입에 따라 적절한 레벨업 메서드 호출
            if (_skill is Skill_BakeBread bakeBread)
            {
                bakeBread.SkillLevelUp();
            }
            // 스킬 타입에 따라 적절한 레벨업 메서드 호출
            if (_skill is Skill_BeastEyes beastEyes)
            {
                beastEyes.SkillLevelUp();
            }
            // 스킬 타입에 따라 적절한 레벨업 메서드 호출
            if (_skill is Skill_FatalStrike fatalStrike)
            {
                fatalStrike.SkillLevelUp();
            }
            // 스킬 타입에 따라 적절한 레벨업 메서드 호출
            if (_skill is Skill_EatChur eatChur)
            {
                eatChur.SkillLevelUp();
            }

            OnSkillLevelChanged?.Invoke();
            UpdateUI();
        }
    }

}
