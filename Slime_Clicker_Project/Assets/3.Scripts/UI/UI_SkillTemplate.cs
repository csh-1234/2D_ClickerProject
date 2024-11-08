using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

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
        infoText.text = _skill.GetCurrentSkillInfo();
        costText.text = _skill.UpgradeCost.ToString();
    }


    public void SetLoadedLevel()
    {
        if (_skill == null) return;

        // 스킬 타입에 관계없이 공통적으로 레벨에 따른 업데이트 수행
        _skill.UpdateSkillByLoadedLevel();
        UpdateUI();
    }

    public void OnUpgradeClick()
    {
        Managers.Instance.Sound.Play("Click", SoundManager.Sound.Effect);
        if (_skill.TryUpgrade(Managers.Instance.Currency.GetCurrentGold()))
        {
            Managers.Instance.Currency.RemoveGold(_skill.UpgradeCost);

            if (_skill is Skill_Zoomies zoomiesSkill)
            {
                zoomiesSkill.SkillLevelUp();
            }
            if (_skill is Skill_BakeBread bakeBread)
            {
                bakeBread.SkillLevelUp();
            }
            if (_skill is Skill_BeastEyes beastEyes)
            {
                beastEyes.SkillLevelUp();
            }
            if (_skill is Skill_FatalStrike fatalStrike)
            {
                fatalStrike.SkillLevelUp();
            }
            if (_skill is Skill_EatChur eatChur)
            {
                eatChur.SkillLevelUp();
            }

            OnSkillLevelChanged?.Invoke();
            UpdateUI();
        }
    }

}
