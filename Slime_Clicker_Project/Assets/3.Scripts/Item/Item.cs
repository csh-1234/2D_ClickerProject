using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DataManager;
using static Enums;
using static GameManager;

public class Item
{
    public Item(ItemData data)
    {
        _data = data;
        CurrentLevel = data.BaseLevel;
    }

    protected ItemData _data;
    public int CurrentLevel { get; set; } 
    public int DataId { get {return _data.DataId; } }
    public string ItemName { get { return _data.ItemName; } }
    public ItemType Type { get { return _data.ItemType; } }
    public ItemRarity Rarity { get { return _data.ItemRarity; } }
    public int UpgradeCost { get { return _data.UpgradeCost; } }
    public string SpriteName { get { return _data.SpriteName; } }
    public int ItemPrice { get { return _data.ItemPrice; } }
    public string ItemInfo { get { return _data.ItemInfo; } }

    public int CurrentAtk
    {
        get
        {
            if (Type == ItemType.Weapon)
            {
                return _data.BaseAtk + ((CurrentLevel-1) * _data.BonusAtk);
            }
            return 0; 
        }
        set
        {

        }
    }
    public int CurrentDef
    {
        get
        {
            if (Type == ItemType.Armor)
            {
                return _data.BaseDef + ((CurrentLevel-1) * _data.BonusDef);
            }
            return 0;
        }
        set
        {

        }
    }

    

    public string GetMainStatText() => Type switch
    {
        ItemType.Weapon => $"ATK +{CurrentAtk}",
        ItemType.Armor => $"DEF +{CurrentDef}",
        _ => string.Empty
    };


    public bool CanUpgrade() => CurrentLevel < _data.MaxLevel;

    public void Upgrade()
    {
        if (CanUpgrade())
        {
            CurrentLevel++;
        }
    }

    public void LoadFromSaveData(ItemSaveData saveData)
    {
        CurrentLevel = saveData.CurrentLevel;
        CurrentAtk = saveData.CurrentAtk;
        CurrentDef = saveData.CurrentDef;
    }
}
