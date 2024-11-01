using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.UI;
using static DataManager;
using static Enums;
public class UI_EquipmentUpgrade : RootUI
{
    public GameObject ItemTemplate;
    public Dictionary<int, ItemData> _itemDic = new Dictionary<int, ItemData>();

    protected override void Awake()
    {
        _itemDic = Managers.Instance.Data.ItemDic;
        base.Awake();
    }

    private void OnEnable()
    {
        UpdateItemInfo();
    }
    private void Start()
    {
        
        SetItem();
    }

    public void SetItem()
    {
        foreach (var item in _itemDic)
        {
            //템플릿 생성
            GameObject _itemTemplate = Instantiate(ItemTemplate, transform);

            //아이템 데이터를 읽어 값을 할당
            Transform ItemIcon = _itemTemplate.transform.Find("IconBoard/Icon");
            if (ItemIcon != null)
            {
                Image _itemIcon = ItemIcon.GetComponent<Image>();
                if (_itemIcon != null)
                {
                    _itemIcon.sprite = Managers.Instance.Resource.Load<Sprite>($"{item.Value.SpriteName}");
                }
                else
                {
                    _itemIcon.sprite = null;
                }
            }

            Transform ItemRarityBoard = _itemTemplate.transform.Find("IconBoard/RarityBoard");
            if (ItemRarityBoard != null)
            {
                Image _rarityBoard = ItemRarityBoard.GetComponent<Image>();
                if (_rarityBoard != null)
                {
                    if (item.Value.ItemRarity == ItemRarity.Normal.ToString()) { _rarityBoard.color = Noraml; }
                    else if(item.Value.ItemRarity == ItemRarity.Advanced.ToString()){ _rarityBoard.color = Advanced; }
                    else if(item.Value.ItemRarity == ItemRarity.Rare.ToString()) { _rarityBoard.color = Rare; }
                    else if(item.Value.ItemRarity == ItemRarity.Legend.ToString()) { _rarityBoard.color = Legend; }
                    else if(item.Value.ItemRarity == ItemRarity.Myth.ToString()) { _rarityBoard.color = Myth; }
                }
                else
                {
                    _rarityBoard.color = Color.black;
                }
            }

            Transform ItemLevel = _itemTemplate.transform.Find("IconBoard/ItemLevel");
            if (ItemLevel != null)
            {
                TextMeshProUGUI _itemLevel = ItemLevel.GetComponent<TextMeshProUGUI>();
                if (_itemLevel != null)
                {
                    _itemLevel.text = $"LV.{item.Value.BaseLevel}";
                }
                else
                {
                    _itemLevel.text = "";
                }
            }
            
            Transform ItemtName = _itemTemplate.transform.Find("ItemTexts/ItemName");
            if (ItemtName != null)
            {
                TextMeshProUGUI _itemName = ItemtName.GetComponent<TextMeshProUGUI>();
                if (_itemName != null)
                {
                    _itemName.text = item.Value.ItemName;
                }
                else
                {
                    _itemName.text = "";
                }
            }

            Transform ItemtStat = _itemTemplate.transform.Find("ItemTexts/ItemStat");
            if (ItemtStat != null)
            {
                TextMeshProUGUI _itemStat = ItemtStat.GetComponent<TextMeshProUGUI>();
                if (_itemStat != null)
                {
                    if(item.Value.ItemType == ItemType.Weapon.ToString())
                    {
                        _itemStat.text = $"ATK +{item.Value.BaseAtk}";
                    }
                    else
                    {
                        _itemStat.text = $"Def +{item.Value.BaseDef}";
                    }
                }
                else
                {
                    _itemStat.text = "";
                }
            }

            Transform ItemtConst = _itemTemplate.transform.Find("UpgradeButton/Board/CurrencyIcon/ItemCost");
            if (ItemtConst != null)
            {
                TextMeshProUGUI _itemCost = ItemtConst.GetComponent<TextMeshProUGUI>();
                if (_itemCost != null)
                {
                    _itemCost.text = $"{item.Value.CostRatio}";
                }
                else
                {
                    _itemCost.text = "";
                }
            }
        }
    }


    public void UpdateItemInfo()
    {
        
    }






}
