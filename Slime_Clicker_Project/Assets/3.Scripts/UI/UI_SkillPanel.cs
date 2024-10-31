using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SkillPanel : RootUI
{
    [SerializeField] private List<UI_SkillButton> skillButtons;  // Inspector에서 할당
    public GameObject template;

    protected override void Awake()
    {
        base.Awake();
        
    }
    private void Start()
    {
        InitializeSkillButtons();
    }

    private void InitializeSkillButtons()
    {
        Player player = Managers.Instance.Game.player;
        foreach (Skill pSkill in player.SkillList)
        {
            GameObject SkillButton = Instantiate(template, transform);
            SkillButton.transform.Find("Icon").GetComponent<Image>().sprite = Managers.Instance.Resource.Load<Sprite>("BeastEyes_Icon");
            UI_SkillButton ui_SkillButton = SkillButton.GetComponent<UI_SkillButton>();
            ui_SkillButton.name = pSkill.name;
            ui_SkillButton.SetSkill(pSkill);

        }
    }
}
