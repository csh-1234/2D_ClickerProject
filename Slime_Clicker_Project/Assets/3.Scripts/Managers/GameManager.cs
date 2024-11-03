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

    #region ���� ���
    //������ ������
    private Dictionary<int, Item> OwnedItems = new Dictionary<int, Item>();
    //������ ������, ���Ÿ�Ժ� �ϳ����� ���� ����
    private Dictionary<ItemType, Item> EquippedItems = new Dictionary<ItemType, Item>();

    private Stats _baseStats = new Stats();         // �÷��̾� �⺻ ����
    public Stats _upgradeStats = new Stats();      // ���� ������ ����
    private Stats _equipmentStats = new Stats();    // ��� ����

    public event Action<Stats> OnStatsChanged;  // ���� ���� �̺�Ʈ �߰�

    //��ü ���� �ջ�
    public Stats UpdateTotalStat()
    {
        Stats total = new Stats();
        total.CopyStats(player._baseStats);  // �⺻ ����
        total.AddStats(_equipmentStats);     // ��� ����
        total.AddStats(_upgradeStats);       // ���׷��̵� ����
        return total;
    }

    public void UpdatePlayerStats()
    {
        if (player == null) return;

        //// ���� �������� ����(���� ������ 0)
        //Stats buffStats = new Stats();
        //if (player._currentStats != null)
        //{
        //    Stats totalStats = UpdateTotalStat(); //���� ��ü ����
        //    buffStats.CopyStats(player._currentStats);  // �÷��̾��� ��������
        //    buffStats.SubStats(totalStats); // ��������
        //}

        //// ���� ������ �⺻ �������� �ʱ�ȭ
        player._currentStats.ClearStat();
        player._currentStats.CopyStats(player._baseStats); //�⺻���ȸ� ����
        Debug.Log($"�⺻ ���� ���� - ATK: {player._currentStats.Attack}, DEF: {player._currentStats.Defense}");

        // ���׷��̵� ���� ����
        if (_upgradeStats != null)
        {
            Debug.Log($"���׷��̵� ���� - ATK: {_upgradeStats.Attack}, DEF: {_upgradeStats.Defense}");
            player._currentStats.AddStats(_upgradeStats);
            Debug.Log($"���׷��̵� ���� �� - ATK: {player._currentStats.Attack}, DEF: {player._currentStats.Defense}");
        }

        // ��� ���� ����
        if (_equipmentStats != null)
        {
            Debug.Log($"��� ���� - ATK: {_equipmentStats.Attack}, DEF: {_equipmentStats.Defense}");
            player._currentStats.AddStats(_equipmentStats);
            Debug.Log($"��� ���� �� - ATK: {player._currentStats.Attack}, DEF: {player._currentStats.Defense}");
        }

        // ���� ���� ������
        if (player is Player playerObj)
        {
            foreach (var buff in playerObj.GetActiveBuffs())
            {
                player._currentStats.AddStats(buff.BuffStats);
            }
        }

        Debug.Log($"���� ���� - ATK: {player._currentStats.Attack}, DEF: {player._currentStats.Defense}");

        //OnStatsChanged?.Invoke(player._currentStats);
    }

    public void CalcEquipItem()
    {
        // ��� ���� �ʱ�ȭ
        _equipmentStats.ClearStat();

        // ��� ���� ����� ���� �ջ�
        foreach (Item item in EquippedItems.Values)
        {
            if (item != null)
            {
                _equipmentStats.AddStatByType(StatType.Atk, item.CurrentAtk);
                _equipmentStats.AddStatByType(StatType.Def, item.CurrentDef);
                Debug.Log($"��� ���� ���: {item.Type}, ATK +{item.CurrentAtk}, DEF +{item.CurrentDef}");
            }
        }
        //�÷��̾� ���� ������Ʈ
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
        //�̹� ������ ������ �籸�� �Ұ�
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

        // ���� Ÿ���� ��� �̹� �ִٸ� ���� ����
        if (EquippedItems.ContainsKey(item.Type))
        {
            EquippedItems.Remove(item.Type);
        }

        // �� ��� ����
        EquippedItems[item.Type] = item;

        // ��� ���� ����
        CalcEquipItem();

        Debug.Log($"��� ���� �Ϸ� �� �÷��̾� ���� - ATK: {player.Atk}, DEF: {player.Def}");
        return true;
    }
    public bool TryUpgrade(int currentGold, Item item)
    {
        if ((item.CurrentLevel >= 1000) || (currentGold < item.UpgradeCost))
        {
            Debug.Log("��尡 �����ϰų� �ִ뷹�� ���Ѽ�");
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
        // ���� ������ ����
        SaveOwnedItems();
        SaveEquippedItems();
        // �ٸ� ���� �����͵� ���⼭ ����
    }

    public void LoadGame()
    {
        // ���� ������ �ε�
        LoadOwnedItems();
        LoadEquippedItems();
        // �ٸ� ���� �����͵� ���⼭ �ε�
    }

    private void SaveOwnedItems()
    {
        // ���� ������ ���� ����
    }

    private void SaveEquippedItems()
    {
        // ���� ������ ���� ����
    }

    private void LoadOwnedItems()
    {
        // ���� ������ �ε� ����
    }

    private void LoadEquippedItems()
    {
        // ���� ������ �ε� ����
    }
    #endregion
}
