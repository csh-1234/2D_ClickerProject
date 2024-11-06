using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static DataManager;
using static Enums;

public class UI_SkillUpgrade : RootUI
{
    [SerializeField] private GameObject slotTemplate;
    [SerializeField] private Transform slotsParent;

    public Dictionary<int, SkillData> _SkillDic = new Dictionary<int, SkillData>();
    private List<UI_SkillTemplate> _slots = new List<UI_SkillTemplate>();

    protected override void Awake()
    {
        //여기서 유저폴더 데이터 있으면 그거 가져오기
        _SkillDic = Managers.Instance.Data.SkillDic;
        base.Awake();
        
    }

    private void Start()
    {
        SetTemplate();
    }

    private void SetTemplate()
    {
        foreach (var skillData in _SkillDic.Values)
        {
            // Player가 가진 스킬 리스트에서 해당 스킬 찾기
            //Skill playerSkill = Managers.Instance.Game.player.SkillList.Find(skill => skill.GetType().Name == $"Skill_Zoomies");
            Skill playerSkill = Managers.Instance.Game.player.SkillList.Find(skill => skill.DataId == skillData.DataId);
            //Skill playerSkill = Managers.Instance.Game.player.SkillList.Find(skill => skill.SpriteName == skillData.SpriteName);
            if (playerSkill != null)
            {
                var _slot = Instantiate(slotTemplate, slotsParent).GetComponent<UI_SkillTemplate>();
                _slot.name = $"Skill_{skillData.SkillName}_Template";
                _slot.SetItem(playerSkill);
                _slot.OnSkillLevelChanged = UpdateAllSlots;
                _slots.Add(_slot);
            }
        }
    }

    public void UpdateAllSlots()
    {
        foreach (var slot in _slots)
        {
            slot.UpdateUI();
        }
    }
}
