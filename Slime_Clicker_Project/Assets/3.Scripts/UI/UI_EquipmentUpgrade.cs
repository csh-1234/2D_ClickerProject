using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static DataManager;
using static Enums;
public class UI_EquipmentUpgrade : RootUI
{
    [SerializeField] private GameObject slotTemplate;
    [SerializeField] private Transform slotsParent;
    [SerializeField] private ItemType equipmentType;  // Inspector에서 설정

    public Dictionary<int, ItemData> _itemDic = new Dictionary<int, ItemData>();
    private List<UI_EquipmentTemplate> _slots = new List<UI_EquipmentTemplate>();

    protected override void Awake()
    {
        _itemDic = Managers.Instance.Data.ItemDic;
        base.Awake();
        SetTemplate();
    }
    private void SetTemplate()
    {
        foreach (var itemData in _itemDic.Values)
        {
            if (itemData.ItemType != equipmentType)
                continue;

            var _slot = Instantiate(slotTemplate, slotsParent).GetComponent<UI_EquipmentTemplate>();
            var _item = new Item(itemData);
            _slot.SetItem(_item);
            _slot.OnEquipStateChanged = UpdateAllSlots;
            _slots.Add(_slot);
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
