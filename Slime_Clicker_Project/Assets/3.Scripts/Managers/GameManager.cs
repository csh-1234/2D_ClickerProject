using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using static Enums;
using static UnityEditor.Progress;

public class GameManager
{
    public Player player;
    public List<Monster> MonsterList = new();

    #region 스탯 계산
    //소유한 아이템
    private Dictionary<int, Item> OwnedItems = new Dictionary<int, Item>();
    //장착한 아이템, 장비타입별 하나씩만 장착 가능
    private Dictionary<ItemType, Item> EquippedItems = new Dictionary<ItemType, Item>();

    private Stats _baseStats = new Stats();         // 플레이어 기본 스탯
    public Stats _upgradeStats = new Stats();      // 스탯 레벨업 스탯
    private Stats _equipmentStats = new Stats();    // 장비 스탯

    public event Action<Stats> OnStatsChanged;  // 스탯 변경 이벤트 추가

    //전체 스탯 합산
    public Stats UpdateTotalStat()
    {
        Stats total = new Stats();
        total.CopyStats(player._baseStats);  // 기본 스탯
        total.AddStats(_equipmentStats);     // 장비 스탯
        total.AddStats(_upgradeStats);       // 업그레이드 스탯
        return total;
    }

    public void UpdatePlayerStats()
    {
        if (player == null) return;

        //// 현재 버프스탯 저장(버프 없으면 0)
        //Stats buffStats = new Stats();
        //if (player._currentStats != null)
        //{
        //    Stats totalStats = UpdateTotalStat(); //현재 전체 스탯
        //    buffStats.CopyStats(player._currentStats);  // 플레이어의 최종스텟
        //    buffStats.SubStats(totalStats); // 버프스텟
        //}

        //// 현재 스탯을 기본 스탯으로 초기화
        player._currentStats.ClearStat();
        player._currentStats.CopyStats(player._baseStats); //기본스탯만 적용
        Debug.Log($"기본 스탯 적용 - ATK: {player._currentStats.Attack}, DEF: {player._currentStats.Defense}");

        // 업그레이드 스탯 적용
        if (_upgradeStats != null)
        {
            Debug.Log($"업그레이드 스탯 - ATK: {_upgradeStats.Attack}, DEF: {_upgradeStats.Defense}");
            player._currentStats.AddStats(_upgradeStats);
            Debug.Log($"업그레이드 적용 후 - ATK: {player._currentStats.Attack}, DEF: {player._currentStats.Defense}");
        }

        // 장비 스탯 적용
        if (_equipmentStats != null)
        {
            Debug.Log($"장비 스탯 - ATK: {_equipmentStats.Attack}, DEF: {_equipmentStats.Defense}");
            player._currentStats.AddStats(_equipmentStats);
            Debug.Log($"장비 적용 후 - ATK: {player._currentStats.Attack}, DEF: {player._currentStats.Defense}");
        }

        // 버프 스탯 재적용
        if (player is Player playerObj)
        {
            foreach (var buff in playerObj.GetActiveBuffs())
            {
                player._currentStats.AddStats(buff.BuffStats);
            }
        }

        Debug.Log($"최종 스탯 - ATK: {player._currentStats.Attack}, DEF: {player._currentStats.Defense}");

        //OnStatsChanged?.Invoke(player._currentStats);
    }

    public void CalcEquipItem()
    {
        // 장비 스탯 초기화
        _equipmentStats.ClearStat();

        // 모든 장착 장비의 스탯 합산
        foreach (Item item in EquippedItems.Values)
        {
            if (item != null)
            {
                _equipmentStats.AddStatByType(StatType.Atk, item.CurrentAtk);
                _equipmentStats.AddStatByType(StatType.Def, item.CurrentDef);
                Debug.Log($"장비 스탯 계산: {item.Type}, ATK +{item.CurrentAtk}, DEF +{item.CurrentDef}");
            }
        }
        //플레이어 스탯 업데이트
        if (player != null)
        {
            UpdatePlayerStats();
        }
    }



    #endregion

    #region Equipment
    public bool HasItem(int dataId)
    {
        return OwnedItems.ContainsKey(dataId);
    }   
    public bool IsEquipped(Item item)
    {
        if (EquippedItems.TryGetValue(item.Type, out Item equippedItem))
        {
            return equippedItem.DataId == item.DataId;
        }
        return false;
    }
    public bool TryBuyItem(Item item)
    {
        //이미 가지고 있으면 재구매 불가
        if (HasItem(item.DataId)) return false;

        if (Managers.Instance.Currency.GetCurrentGold() >= item.ItemPrice)
        {
            Managers.Instance.Currency.RemoveGold(item.ItemPrice);
            OwnedItems.Add(item.DataId, item);
            return true;
        }
        return false;
    }
    public bool TryUnequipItem(ItemType itemType)
    {
        if (!EquippedItems.TryGetValue(itemType, out Item unequippedItem))
            return false;

        EquippedItems.Remove(itemType);
        CalcEquipItem();
        UpdatePlayerStats();

        return true;
    }
    public bool TryEquipItem(Item item)
    {
        if (!HasItem(item.DataId)) return false;

        // 같은 타입의 장비가 이미 있다면 먼저 해제
        if (EquippedItems.ContainsKey(item.Type))
        {
            EquippedItems.Remove(item.Type);
        }

        // 새 장비 장착
        EquippedItems[item.Type] = item;

        // 장비 스탯 재계산
        CalcEquipItem();

        Debug.Log($"장비 장착 완료 후 플레이어 스탯 - ATK: {player.Atk}, DEF: {player.Def}");
        return true;
    }
    public bool TryUpgrade(int currentGold, Item item)
    {
        if ((item.CurrentLevel >= 1000) || (currentGold < item.UpgradeCost))
        {
            Debug.Log("골드가 부족하거나 최대레벨 상한선");
            return false;
        }

        item.CurrentLevel++;

        if (IsEquipped(item))
        {
            CalcEquipItem();
        }
        return true;
    }
    #endregion


    #region Save/Load
    public void SaveGame()
    {
        // 게임 데이터 저장
        SaveOwnedItems();
        SaveEquippedItems();
        // 다른 게임 데이터도 여기서 저장
    }

    public void LoadGame()
    {
        // 게임 데이터 로드
        LoadOwnedItems();
        LoadEquippedItems();
        // 다른 게임 데이터도 여기서 로드
    }

    private void SaveOwnedItems()
    {
        // 보유 아이템 저장 로직
    }

    private void SaveEquippedItems()
    {
        // 장착 아이템 저장 로직
    }

    private void LoadOwnedItems()
    {
        // 보유 아이템 로드 로직
    }

    private void LoadEquippedItems()
    {
        // 장착 아이템 로드 로직
    }
    #endregion
}
