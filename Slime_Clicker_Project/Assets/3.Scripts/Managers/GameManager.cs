using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using static DataManager;
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
        //SaveGame();
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
    public Item GetOwnedItem(int dataId)
    {
        if (OwnedItems.TryGetValue(dataId, out Item item))
        {
            return item;
        }
        return null;
    }
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
        //SaveOwnedItems();
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

        //// ���� Ÿ���� ��� �̹� �ִٸ� ���� ����
        //if (EquippedItems.ContainsKey(item.Type))
        //{
        //    EquippedItems.Remove(item.Type);
        //}

        //// �� ��� ����
        //EquippedItems[item.Type] = item;

        //// ��� ���� ����
        //CalcEquipItem();
        //SaveOwnedItems();
        //Debug.Log($"��� ���� �Ϸ� �� �÷��̾� ���� - ATK: {player.Atk}, DEF: {player.Def}");
        Item ownedItem = OwnedItems[item.DataId];

        // ���� Ÿ���� ��� �̹� �ִٸ� ���� ����
        if (EquippedItems.ContainsKey(item.Type))
        {
            EquippedItems.Remove(item.Type);
        }

        // �� ��� ���� (OwnedItems�� ������ ���� ���)
        EquippedItems[item.Type] = ownedItem;

        // ��� ���� ���
        CalcEquipItem();

        // ������ ������ ����
        //SaveOwnedItems();
        OnEquipmentChanged?.Invoke();

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
        //SaveOwnedItems();
        return true;
    }
    #endregion

    #region Save&Load
    public void SaveGame()
    {
        SaveStatData();
        SaveOwnedItems();
        SaveSkillData();
    }

    public void LoadGame()
    {
        // ���� ������ �ε�
        StatLevelData loadedStatData = LoadStatData();
        if (loadedStatData != null)
        {
            // �ε�� ������ ����
            Managers.Instance.StatUpgrade.AtkLevel = loadedStatData.AtkLevel;
            Managers.Instance.StatUpgrade.HpLevel = loadedStatData.HpLevel;
            Managers.Instance.StatUpgrade.DefLevel = loadedStatData.DefLevel;
            Managers.Instance.StatUpgrade.CriRateLevel = loadedStatData.CriRateLevel;
            Managers.Instance.StatUpgrade.criDamageLevel = loadedStatData.CriDamageLevel;
            Managers.Instance.StatUpgrade.AtkSpeedLevel = loadedStatData.AtkSpeedLevel;
        }
        SkillLevelData loadedSkillData = LoadSkillData();
        if(loadedSkillData != null)
        {
            player.SkillList[0].CurrentLevel = loadedSkillData.Skill_Zoomies_Level;
            player.SkillList[1].CurrentLevel = loadedSkillData.Skill_BakeBread_Level;
            player.SkillList[2].CurrentLevel = loadedSkillData.Skill_EatChur_Level;
            player.SkillList[3].CurrentLevel = loadedSkillData.Skill_BeastEyes_Level;
            player.SkillList[4].CurrentLevel = loadedSkillData.Skill_FatalStrike_Level;
        }

        LoadOwnedItems();
        // �ٸ� ���� �����͵� ���⼭ �ε�
    }

    #region saveStatData
    [System.Serializable]
    public class StatLevelData
    {
        public int AtkLevel { get; set; }
        public int HpLevel { get; set; }
        public int DefLevel { get; set; }
        public int CriRateLevel { get; set; }
        public int CriDamageLevel { get; set; }
        public int AtkSpeedLevel { get; set; }
    }

    private void SaveStatData()
    {
        StatLevelData statData = new StatLevelData
        {
            //0�̸� 1�� ���ֱ�
            AtkLevel = Managers.Instance.StatUpgrade.AtkLevel == 0 ? 1 : Managers.Instance.StatUpgrade.AtkLevel,
            HpLevel = Managers.Instance.StatUpgrade.HpLevel == 0 ? 1 : Managers.Instance.StatUpgrade.HpLevel,
            DefLevel = Managers.Instance.StatUpgrade.DefLevel == 0 ? 1 : Managers.Instance.StatUpgrade.DefLevel,
            CriRateLevel = Managers.Instance.StatUpgrade.CriRateLevel == 0 ? 1 : Managers.Instance.StatUpgrade.CriRateLevel,
            CriDamageLevel = Managers.Instance.StatUpgrade.criDamageLevel == 0 ? 1 : Managers.Instance.StatUpgrade.criDamageLevel,
            AtkSpeedLevel = Managers.Instance.StatUpgrade.AtkSpeedLevel == 0 ? 1 : Managers.Instance.StatUpgrade.AtkSpeedLevel,
        };

        string jsonPath = $"{Application.dataPath}/1.Resources/Data/SaveData/StatLevelData.json";

        try
        {
            // ���丮�� ������ ����
            Directory.CreateDirectory(Path.GetDirectoryName(jsonPath));

            // �����͸� JSON ���ڿ��� ��ȯ
            string jsonStr = JsonConvert.SerializeObject(statData, Formatting.Indented);

            // ���Ͽ� ����
            File.WriteAllText(jsonPath, jsonStr);

#if UNITY_EDITOR
            AssetDatabase.Refresh();
            Debug.Log($"���� ������ ���� �Ϸ�: {jsonPath}");
#endif
        }
        catch (Exception e)
        {
            Debug.LogError($"���� ������ ���� ����: {e.Message}");
        }
    
    }


    private StatLevelData LoadStatData()
    {
        string jsonPath = Path.Combine(Application.dataPath, "1.Resources/Data/SaveData/StatLevelData.json");

        try
        {
            if (File.Exists(jsonPath))
            {
                string jsonStr = File.ReadAllText(jsonPath);
                return JsonConvert.DeserializeObject<StatLevelData>(jsonStr);
            }
            else
            {
                Debug.Log("����� ���� �����Ͱ� �����ϴ�. �⺻���� ����մϴ�.");
                return new StatLevelData(); // �⺻������ �ʱ�ȭ�� ��ü ��ȯ
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"���� ������ �ε� ����: {e.Message}");
            return new StatLevelData();
        }
    }

    #endregion

    #region saveSkillData

    [System.Serializable]
    public class SkillLevelData
    {
        public int Skill_Zoomies_Level { get; set; }
        public int Skill_BakeBread_Level { get; set; }
        public int Skill_BeastEyes_Level { get; set; }
        public int Skill_EatChur_Level { get; set; }
        public int Skill_FatalStrike_Level { get; set; }
    }

    private void SaveSkillData()
    {
        SkillLevelData skillData = new SkillLevelData
        {
            //0�̸� 1�� ���ֱ�
            Skill_Zoomies_Level = player.SkillList[0].CurrentLevel == 0 ? 1 : player.SkillList[0].CurrentLevel,
            Skill_BakeBread_Level = player.SkillList[1].CurrentLevel == 0 ? 1 : player.SkillList[1].CurrentLevel,
            Skill_EatChur_Level = player.SkillList[2].CurrentLevel == 0 ? 1 : player.SkillList[2].CurrentLevel,
            Skill_BeastEyes_Level = player.SkillList[3].CurrentLevel == 0 ? 1 : player.SkillList[3].CurrentLevel,
            Skill_FatalStrike_Level = player.SkillList[4].CurrentLevel == 0 ? 1 : player.SkillList[4].CurrentLevel,
        };

        string jsonPath = $"{Application.dataPath}/1.Resources/Data/SaveData/SkillLevelData.json";

        try
        {
            // ���丮�� ������ ����
            Directory.CreateDirectory(Path.GetDirectoryName(jsonPath));

            // �����͸� JSON ���ڿ��� ��ȯ
            string jsonStr = JsonConvert.SerializeObject(skillData, Formatting.Indented);

            // ���Ͽ� ����
            File.WriteAllText(jsonPath, jsonStr);

#if UNITY_EDITOR
            AssetDatabase.Refresh();
            Debug.Log($"���� ������ ���� �Ϸ�: {jsonPath}");
#endif
        }
        catch (Exception e)
        {
            Debug.LogError($"���� ������ ���� ����: {e.Message}");
        }

    }

    private SkillLevelData LoadSkillData()
    {
        string jsonPath = Path.Combine(Application.dataPath, "1.Resources/Data/SaveData/SkillLevelData.json");

        try
        {
            if (File.Exists(jsonPath))
            {
                string jsonStr = File.ReadAllText(jsonPath);
                return JsonConvert.DeserializeObject<SkillLevelData>(jsonStr);
            }
            else
            {
                Debug.Log("����� ��ų �����Ͱ� �����ϴ�. �⺻���� ����մϴ�.");
                return new SkillLevelData(); // �⺻������ �ʱ�ȭ�� ��ü ��ȯ
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"��ų ������ �ε� ����: {e.Message}");
            return new SkillLevelData();
        }
    }
    #endregion

    #region saveItemData
    [System.Serializable]
    public class ItemSaveData
    {
        public int DataId { get; set; }
        public int CurrentLevel { get; set; }
        public int CurrentAtk { get; set; }
        public int CurrentDef { get; set; }
    }

    [System.Serializable]
    public class ItemInventorySaveData
    {
        public List<ItemSaveData> OwnedItems { get; set; } = new List<ItemSaveData>();
        public Dictionary<ItemType, int> EquippedItemIds { get; set; } = new Dictionary<ItemType, int>();
    }

    private void SaveOwnedItems()
    {
        var saveData = new ItemInventorySaveData();

        // ���� ������ ����
        foreach (var item in OwnedItems.Values)
        {
            saveData.OwnedItems.Add(new ItemSaveData
            {
                DataId = item.DataId,
                CurrentLevel = item.CurrentLevel,
                CurrentAtk = item.CurrentAtk,
                CurrentDef = item.CurrentDef
            });
        }

        // ���� ���� ������ ����
        foreach (var kvp in EquippedItems)
        {
            if (kvp.Value != null)
            {
                saveData.EquippedItemIds[kvp.Key] = kvp.Value.DataId;
            }
        }

        string jsonPath = $"{Application.dataPath}/1.Resources/Data/SaveData/ItemData.json";

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(jsonPath));
            string jsonStr = JsonConvert.SerializeObject(saveData, Formatting.Indented);
            File.WriteAllText(jsonPath, jsonStr);

#if UNITY_EDITOR
            AssetDatabase.Refresh();
            Debug.Log($"������ ������ ���� �Ϸ�: {jsonPath}");
#endif
        }
        catch (Exception e)
        {
            Debug.LogError($"������ ������ ���� ����: {e.Message}");
        }
    }
    public event Action OnEquipmentChanged;
    private void LoadOwnedItems()
    {
        string jsonPath = Path.Combine(Application.dataPath, "1.Resources/Data/SaveData/ItemData.json");

        try
        {
            if (File.Exists(jsonPath))
            {
                string jsonStr = File.ReadAllText(jsonPath);
                var saveData = JsonConvert.DeserializeObject<ItemInventorySaveData>(jsonStr);

                // ���� ������ �ε�
                OwnedItems.Clear();
                foreach (var itemData in saveData.OwnedItems)
                {
                    if (Managers.Instance.Data.ItemDic.TryGetValue(itemData.DataId, out ItemData baseItemData))
                    {
                        Item item = new Item(baseItemData);
                        item.LoadFromSaveData(itemData);
                        OwnedItems.Add(item.DataId, item);
                    }
                }

                // ���� ������ �ε�
                EquippedItems.Clear();
                foreach (var kvp in saveData.EquippedItemIds)
                {
                    if (OwnedItems.TryGetValue(kvp.Value, out Item item))
                    {
                        EquippedItems[kvp.Key] = item;
                    }
                }

                // ��� ���� ����
                CalcEquipItem();

                // UI ������Ʈ �˸�
                OnEquipmentChanged?.Invoke();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"������ ������ �ε� ����: {e.Message}");
        }
    }

    #endregion
    #endregion

}

