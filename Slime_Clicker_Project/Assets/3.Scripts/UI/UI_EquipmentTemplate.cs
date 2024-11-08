using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

public class UI_EquipmentTemplate : RootUI
{
    protected override void Awake()
    {
        base.Awake();

    }
    [SerializeField] private Image iconImage;
    [SerializeField] private Image rarityBoard;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI statText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI infoText;

    [SerializeField] private Button buyButton;
    [SerializeField] private Button equipButton;
    [SerializeField] private Button upgradeButton;

    public Action OnEquipStateChanged;


    private Item _item;

    public void SetItem(Item item)
    {
        _item = item;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (_item == null) return;

        if (!Managers.Instance.Game.HasItem(_item.DataId))
        {
            //가지고 있지 않으면 구매창
            buyButton.gameObject.SetActive(true);
            equipButton.gameObject.SetActive(false);
            upgradeButton.gameObject.SetActive(false);
        }
        //가지고만 있으면 장착창
        if (Managers.Instance.Game.HasItem(_item.DataId))
        {
            buyButton.gameObject.SetActive(false);
            equipButton.gameObject.SetActive(true);
            upgradeButton.gameObject.SetActive(false);
        }
        //장착중이면 강화창
        if (Managers.Instance.Game.IsEquipped(_item))
        {
            buyButton.gameObject.SetActive(false);
            equipButton.gameObject.SetActive(false);
            upgradeButton.gameObject.SetActive(true);
        }


        iconImage.sprite = Managers.Instance.Resource.Load<Sprite>(_item.SpriteName);
        if (_item.Rarity == ItemRarity.Normal) { rarityBoard.color = Normal; }
        else if (_item.Rarity == ItemRarity.Advanced) { rarityBoard.color = Advanced; }
        else if (_item.Rarity == ItemRarity.Rare) { rarityBoard.color = Rare; }
        else if (_item.Rarity == ItemRarity.Legend) { rarityBoard.color = Legend; }
        else if (_item.Rarity == ItemRarity.Myth) { rarityBoard.color = Myth; }
        levelText.text = $"LV.{_item.CurrentLevel}";
        nameText.text = _item.ItemName;
        statText.text = _item.GetMainStatText();
        costText.text = _item.UpgradeCost.ToString();
        priceText.text = _item.ItemPrice.ToString();
        infoText.text = _item.ItemInfo;

        //infoText.text = Managers.Instance.Data.ItemDic[_item.DataId].ItemInfo;
        upgradeButton.interactable = _item.CurrentLevel < 1000;
    }

    public void OnBuyClick()
    {
        
        print("구매");
        if (Managers.Instance.Game.TryBuyItem(_item))
        {
            Managers.Instance.Sound.Play("Buy", SoundManager.Sound.Effect);
            print($"{_item.ItemName} 구매완료");
            equipButton.gameObject.SetActive(true);
            buyButton.gameObject.SetActive(false);
        }
        else
        {
            print($"{_item.ItemName} 구매실패");
        }
        OnEquipStateChanged.Invoke();
        UpdateUI();
    }

    public void OnEquipClick()
    {
        Managers.Instance.Sound.Play("Equip", SoundManager.Sound.Effect);
        print("장착");

        if (Managers.Instance.Game.TryEquipItem(_item))
        {
            print($"{_item.ItemName} 장착완료");
            equipButton.gameObject.SetActive(false);
            upgradeButton.gameObject.SetActive(true);
        }
        else
        {
            print($"{_item.ItemName} 장착실패");
        }
        OnEquipStateChanged.Invoke();
        UpdateUI();

    }

    public void OnUpgradeClick()
    {
        Managers.Instance.Sound.Play("Click", SoundManager.Sound.Effect);
        print("업글");
        if (Managers.Instance.Game.TryUpgrade(Managers.Instance.Currency.GetCurrentGold(), _item))
        {
            Managers.Instance.Currency.RemoveGold(_item.UpgradeCost);

            UpdateUI();
        }
    }
}
