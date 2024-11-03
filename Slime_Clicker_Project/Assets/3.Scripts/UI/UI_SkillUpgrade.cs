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
        //���⼭ �������� ������ ������ �װ� ��������
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
            // Player�� ���� ��ų ����Ʈ���� �ش� ��ų ã��
            //Skill playerSkill = Managers.Instance.Game.player.SkillList.Find(skill => skill.GetType().Name == $"Skill_Zoomies");
            Skill playerSkill = Managers.Instance.Game.player.SkillList.Find(skill => skill.SpriteName == skillData.SpriteName);
            if (playerSkill != null)
            {
                var _slot = Instantiate(slotTemplate, slotsParent).GetComponent<UI_SkillTemplate>();
                _slot.name = $"Skill_{skillData.SkillName}_Template";
                _slot.SetItem(playerSkill);
                _slot.OnSkillLevelChanged = UpdateAllSlots;
                _slots.Add(_slot);
            }
        }



        //foreach (var skillData in _SkillDic.Values)
        //{
        //    // ���� ������ ����
        //    var _slot = Instantiate(slotTemplate, slotsParent).GetComponent<UI_SkillTemplate>();
        //    _slot.name = $"Skill_{skillData.SkillName}_Template";

        //    // Skill ������Ʈ�� ���� ���ӿ�����Ʈ ����(monobehavior �� ��ӹް� �־� new�� ������ �Ұ��ϴ�)
        //    var skillObject = new GameObject($"Skill_{skillData.SkillName}");
        //    skillObject.transform.SetParent(gameObject.transform);

        //    Skill _skill = null;
        //    switch (skillData.DataId)
        //    {
        //        case 200000: // Zoomies ��ų ID
        //            _skill = skillObject.AddComponent<Skill_Zoomies>();
        //            break;
        //        // �ٸ� ��ų�鿡 ���� case �߰�
        //        default:
        //            _skill = skillObject.AddComponent<Skill>();
        //            break;
        //    }


        //    _skill.InitializeWithData(skillData);  // ���ο� �ʱ�ȭ �޼��� ���
        //    _slot.SetItem(_skill);
        //    _slot.OnSkillLevelChanged = UpdateAllSlots;
        //    _slots.Add(_slot);
        //}
    }

    public void UpdateAllSlots()
    {
        foreach (var slot in _slots)
        {
            slot.UpdateUI();
        }
    }


    //private void Start()
    //{
    //    SkillLevel = gameObject.transform.Find("IconBoard/LevelText").GetComponent<TextMeshProUGUI>();
    //    SkillCost = gameObject.transform.Find("MenuButton/Board/CurrencyIcon/CostText").GetComponent<TextMeshProUGUI>();
    //    statType = gameObject.name;

    //    Managers.Instance.StatUpgrade.OnStatChanged += OnStatChanged;
    //    var initialStats = Managers.Instance.StatUpgrade.GetStatInfo(statType);

    //    UpdateStat(initialStats.level, initialStats.bonus, initialStats.cost);
    //    //BindEventToObjects();
    //}

    //#region ObjectEvent
    //private List<UI_EventHandler> _boundHandlers = new List<UI_EventHandler>();
    //private void BindEvent(string objectName, Action action)
    //{
    //    Transform objectTransform = transform.Find(objectName);
    //    if (objectTransform != null)
    //    {
    //        UI_EventHandler eventHandler = objectTransform.GetOrAddComponent<UI_EventHandler>();
    //        eventHandler.OnClickHandler += action;
    //        _boundHandlers.Add(eventHandler);
    //    }
    //    else
    //    {
    //        Debug.LogWarning($"[{objectName}] Object NotFound");
    //    }
    //}


    //private void OnStatChanged(string type, int level, int bonus, int cost)
    //{
    //    // �ڽ��� ���� Ÿ�Կ� ���� ������׸� ó��
    //    if (type == statType)
    //    {
    //        UpdateStat(level, bonus, cost);
    //    }
    //}

    //private void UpdateStat(int level, int bonus, int cost)
    //{
    //    SkillLevel.text = $"LV.{level}";
    //    SkillCost.text = $"{cost}";
    //}

    //private void BindEventToObjects()
    //{
    //    // �̺�Ʈ�� �߰��� (������Ʈ �̸�, �޼���)
    //    BindEvent("MenuButton", OnUpgradeButtonClick);
    //}

    //private void OnUpgradeButtonClick()
    //{
    //    Managers.Instance.StatUpgrade.statUpgrade(statType);
    //}

    //�̺�Ʈ ����, �ϴ� ������ �ֵ�����
    //private void OnDisable()
    //{
    //    foreach (var handler in _boundHandlers)
    //    {
    //        if (handler != null)
    //        {
    //            handler.OnClickHandler = null;
    //            handler.OnPressedHandler = null;
    //            handler.OnPointerDownHandler = null;
    //            handler.OnPointerUpHandler = null;
    //            handler.OnDragHandler = null;
    //            handler.OnBeginDragHandler = null;
    //            handler.OnEndDragHandler = null;
    //        }
    //    }
    //    //if (Managers.Instance != null && Managers.Instance.StatUpgrade != null)
    //    //{
    //    Managers.Instance.StatUpgrade.OnStatChanged -= OnStatChanged;
    //    //}
    //    _boundHandlers.Clear();
    //}

}
