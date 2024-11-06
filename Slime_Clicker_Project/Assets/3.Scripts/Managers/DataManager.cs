using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEngine;
using static Enums;


public class DataManager
{
    public Dictionary<int, CreatureData> CreatureDic { get; private set; } = new Dictionary<int, CreatureData>();
    public Dictionary<int, SkillData> SkillDic { get; private set; } = new Dictionary<int, SkillData>();
    public Dictionary<int, ItemData> ItemDic { get; private set; } = new Dictionary<int, ItemData>();

    public void Initialize()
    {
        LoadCreatureData();
        LoadSklillData();
        LoadItemData();
    }

    #region Creature
    private void LoadCreatureData()
    {
        string csvPath = $"{Application.dataPath}/1.Resources/Data/Excel/CreatureData.csv";
        string jsonPath = $"{Application.dataPath}/1.Resources/Data/JsonData/CreatureData.json";

        //csv������ �����ϰ�, json������ �ֽ�ȭ ���� ������ �ٽ� �о dic�� �־���
        if (File.Exists(csvPath) && (!File.Exists(jsonPath) || File.GetLastWriteTime(csvPath) > File.GetLastWriteTime(jsonPath)))
        {
            ParseCreatureData("Creature");
        }

        if (File.Exists(jsonPath))
        {
            string jsonContent = File.ReadAllText(jsonPath);
            CreatureDataWrapper wrapper = JsonConvert.DeserializeObject<CreatureDataWrapper>(jsonContent);
            CreatureDic = wrapper.Creatures.ToDictionary(c => c.DataId);
        }
        else
        {
            Debug.LogError("CreatureData.json file not found. Please make sure to parse CSV first.");
        }
    }

    public void ParseCreatureData(string filename)
    {
        List<CreatureData> creatureList = new List<CreatureData>();

        string[] lines = File.ReadAllText($"{Application.dataPath}/1.Resources/Data/Excel/{filename}Data.csv").Split("\n");

        //0��° ���� � ���������� ��Ÿ���� ������ 1���� ����
        for (int y = 1; y < lines.Length; y++)
        {
            // csv �� ���� �� �ܾ �߶� row�� ������
            string[] row = lines[y].Replace("\r", "").Split(',');

            if (row.Length == 0 || string.IsNullOrEmpty(row[0])) continue;

            int i = 0;
            CreatureData cd = new CreatureData
            {
                //������� ����Ǿ��� ������ row�� ����� �ܾ �ϳ��� ������
                DataId = ConvertValue<int>(row[i++]),
                CreatureName = ConvertValue<string>(row[i++]),  
                CreatureType = ConvertValue<ObjectType>(row[i++]),
                Hp = ConvertValue<int>(row[i++]),
                MaxHp = ConvertValue<int>(row[i++]),
                Atk = ConvertValue<int>(row[i++]),
                Def = ConvertValue<int>(row[i++]),
                CriRate = ConvertValue<float>(row[i++]),
                CriDamage = ConvertValue<float>(row[i++]),
                MoveSpeed = ConvertValue<float>(row[i++]),
                AtkSpeed = ConvertValue<float>(row[i++]),
                SpriteName = ConvertValue<string>(row[i++]),
            };
            creatureList.Add(cd);
            CreatureDic = creatureList.ToDictionary(c => c.DataId);
        }

        CreatureDataWrapper wrapper = new CreatureDataWrapper { Creatures = creatureList };
        string jsonStr = JsonConvert.SerializeObject(wrapper, Newtonsoft.Json.Formatting.Indented); // Formatting.Indented : �ڵ����� ���� / �鿩����� ���ڿ��� ������
        File.WriteAllText($"{Application.dataPath}/1.Resources/Data/JsonData/{filename}Data.json", jsonStr);
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif

        CreatureDic = creatureList.ToDictionary(c => c.DataId);
    }
    #endregion

    #region Item
    private void LoadItemData()
    {
        string csvPath = $"{Application.dataPath}/1.Resources/Data/Excel/ItemData.csv";
        string jsonPath = $"{Application.dataPath}/1.Resources/Data/JsonData/ItemData.json";

        //csv������ �����ϰ�, json������ �ֽ�ȭ ���� ������ �ٽ� �о dic�� �־���
        if (File.Exists(csvPath) && (!File.Exists(jsonPath) || File.GetLastWriteTime(csvPath) > File.GetLastWriteTime(jsonPath)))
        {
            ParseItemData("Item");
        }

        if (File.Exists(jsonPath))
        {
            string jsonContent = File.ReadAllText(jsonPath);
            ItemDataWrapper wrapper = JsonConvert.DeserializeObject<ItemDataWrapper>(jsonContent);
            //foreach (var creature in wrapper.creatures)
            //{
            //    CreatureDic.Add(creature.DataId, creature);
            //}
            ItemDic = wrapper.Items.ToDictionary(c => c.DataId);
        }
        else
        {
            Debug.LogError("CreatureData.json file not found. Please make sure to parse CSV first.");
        }
    }

    public void ParseItemData(string filename)
    {
        List<ItemData> itemList = new List<ItemData>();

        string[] lines = File.ReadAllText($"{Application.dataPath}/1.Resources/Data/Excel/{filename}Data.csv").Split("\n");

        //0��° ���� � ���������� ��Ÿ���� ������ 1���� ����
        for (int y = 1; y < lines.Length; y++)
        {
            // csv �� ���� �� �ܾ �߶� row�� ������
            string[] row = lines[y].Replace("\r", "").Split(',');

            if (row.Length == 0 || string.IsNullOrEmpty(row[0])) continue;

            int i = 0;
            ItemData id = new ItemData
            {
                //������� ����Ǿ��� ������ row�� ����� �ܾ �ϳ��� ������
                DataId = ConvertValue<int>(row[i++]),
                ItemName = ConvertValue<string>(row[i++]),
                ItemType = ConvertValue<ItemType>(row[i++]),
                ItemRarity = ConvertValue<ItemRarity>(row[i++]),
                BaseLevel = ConvertValue<int>(row[i++]),
                MaxLevel = ConvertValue<int>(row[i++]),
                BaseAtk = ConvertValue<int>(row[i++]),
                BonusAtk = ConvertValue<int>(row[i++]),
                BaseDef = ConvertValue<int>(row[i++]),
                BonusDef = ConvertValue<int>(row[i++]),
                ItemPrice = ConvertValue<int>(row[i++]),
                UpgradeCost = ConvertValue<int>(row[i++]),
                CostRatio = ConvertValue<float>(row[i++]),
                ItemInfo = ConvertValue<string>(row[i++]),
                SpriteName = ConvertValue<string>(row[i++]),
            };
            itemList.Add(id);
            ItemDic = itemList.ToDictionary(c => c.DataId);
        }
        
        ItemDataWrapper wrapper = new ItemDataWrapper { Items = itemList };
        string jsonStr = JsonConvert.SerializeObject(wrapper, Newtonsoft.Json.Formatting.Indented); // Formatting.Indented : �ڵ����� ���� / �鿩����� ���ڿ��� ������
        File.WriteAllText($"{Application.dataPath}/1.Resources/Data/JsonData/{filename}Data.json", jsonStr);
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif

        ItemDic = itemList.ToDictionary(c => c.DataId);
    }
    #endregion

    #region Skill
    private void LoadSklillData()
    {
        string csvPath = $"{Application.dataPath}/1.Resources/Data/Excel/SkillData.csv";
        string jsonPath = $"{Application.dataPath}/1.Resources/Data/JsonData/SkillData.json";

        //csv������ �����ϰ�, json������ �ֽ�ȭ ���� ������ �ٽ� �о dic�� �־���
        if (File.Exists(csvPath) && (!File.Exists(jsonPath) || File.GetLastWriteTime(csvPath) > File.GetLastWriteTime(jsonPath)))
        {
            ParseSkillData("Skill");
        }

        if (File.Exists(jsonPath))
        {
            string jsonContent = File.ReadAllText(jsonPath);
            SkillDataWrapper wrapper = JsonConvert.DeserializeObject<SkillDataWrapper>(jsonContent);
            SkillDic = wrapper.Skills.ToDictionary(c => c.DataId);
        }
        else
        {
            Debug.LogError("CreatureData.json file not found. Please make sure to parse CSV first.");
        }
    }

    public void ParseSkillData(string filename)
    {
        List<SkillData> skillList = new List<SkillData>();

        string[] lines = File.ReadAllText($"{Application.dataPath}/1.Resources/Data/Excel/{filename}Data.csv").Split("\n");

        //0��° ���� � ���������� ��Ÿ���� ������ 1���� ����
        for (int y = 1; y < lines.Length; y++)
        {
            // csv �� ���� �� �ܾ �߶� row�� ������
            string[] row = lines[y].Replace("\r", "").Split(',');

            if (row.Length == 0 || string.IsNullOrEmpty(row[0])) continue;

            int i = 0;
            SkillData id = new SkillData
            {
                //������� ����Ǿ��� ������ row�� ����� �ܾ �ϳ��� ������
                DataId = ConvertValue<int>(row[i++]),
                SkillName = ConvertValue<string>(row[i++]),
                SkillType = ConvertValue<SkillType>(row[i++]),
                BaseLevel = ConvertValue<int>(row[i++]),
                MaxLevel = ConvertValue<int>(row[i++]),
                Cooldown = ConvertValue<float>(row[i++]),
                MaxCooldown = ConvertValue<float>(row[i++]),
                Duration = ConvertValue<float>(row[i++]),
                MaxDuration = ConvertValue<float>(row[i++]),
                HpBonus = ConvertValue<int>(row[i++]),
                AtkBonus = ConvertValue<int>(row[i++]),
                DefBonus = ConvertValue<int>(row[i++]),
                CriRateBonus = ConvertValue<float>(row[i++]),
                CriDamageBonus = ConvertValue<float>(row[i++]),
                AtkSpeedBonus = ConvertValue<float>(row[i++]),
                HealAmount = ConvertValue<int>(row[i++]),
                Damage = ConvertValue<int>(row[i++]),
                SkillInfo = ConvertValue<string>(row[i++]),
                UpgradCost = ConvertValue<int>(row[i++]),
                SpriteName = ConvertValue<string>(row[i++]),
            };
            skillList.Add(id);
            SkillDic = skillList.ToDictionary(c => c.DataId);
        }

        SkillDataWrapper wrapper = new SkillDataWrapper { Skills = skillList };
        string jsonStr = JsonConvert.SerializeObject(wrapper, Newtonsoft.Json.Formatting.Indented); // Formatting.Indented : �ڵ����� ���� / �鿩����� ���ڿ��� ������
        File.WriteAllText($"{Application.dataPath}/1.Resources/Data/JsonData/{filename}Data.json", jsonStr);
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif

        SkillDic = skillList.ToDictionary(c => c.DataId);
    }
    #endregion

    #region Util
    //�Է¹��� ���ڿ��� �Է¹��� TŸ������ ��ȯ
    private static T ConvertValue<T>(string value)
    {
        if (string.IsNullOrEmpty(value))
            return default(T);

        TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
        return (T)converter.ConvertFromString(value);
    }

    //csv���� &'�� ���е� ���ڿ��� T Ÿ���� ����Ʈ�� ��ȯ
    private static List<T> ConvertList<T>(string value)
    {
        if (string.IsNullOrEmpty(value))
            return new List<T>();

        return value.Split('&').Select(x => ConvertValue<T>(x)).ToList();
    }
    #endregion

    #region Data
    [Serializable]
    public class CreatureData
    {
        public int DataId;
        public string CreatureName;
        public ObjectType CreatureType;
        public int Hp;
        public int MaxHp;
        public int Atk;
        public int Def;
        public float CriRate;
        public float CriDamage;
        public float MoveSpeed;
        public float AtkSpeed;
        public string SpriteName;
    }

    [Serializable]
    private class CreatureDataWrapper
    {
        public List<CreatureData> Creatures;
    }

    [Serializable]
    public class SkillData
    {
        public int DataId;
        public string SkillName;
        public SkillType SkillType;
        public int BaseLevel;
        public int MaxLevel;
        public float Cooldown;
        public float MaxCooldown;
        public float Duration;
        public float MaxDuration;
        public int HpBonus;
        public int AtkBonus;
        public int DefBonus;
        public float CriRateBonus;
        public float CriDamageBonus;
        public float AtkSpeedBonus;
        public int HealAmount;
        public int Damage;
        public string SkillInfo;
        public int UpgradCost;
        public string SpriteName;
    }

    [Serializable]
    private class SkillDataWrapper
    {
        public List<SkillData> Skills;
    }

    [Serializable]
    public class ItemData
    {
         public int     DataId;
         public string  ItemName;
         public ItemType  ItemType;
         public ItemRarity  ItemRarity;
         public int     BaseLevel;
         public int     MaxLevel;
         public int     BaseAtk;
         public int     BonusAtk;
         public int     BaseDef;
         public int     BonusDef;
         public int     ItemPrice;
         public int     UpgradeCost;
         public float   CostRatio;
         public string  ItemInfo;
         public string  SpriteName;
    }

    [Serializable]
    private class ItemDataWrapper
    {
        public List<ItemData> Items;
    }
    #endregion
}