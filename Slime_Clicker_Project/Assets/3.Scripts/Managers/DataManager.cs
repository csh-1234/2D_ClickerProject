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
    public Dictionary<int, ItemData> ItemDic { get; private set; } = new Dictionary<int, ItemData>();
    public Dictionary<int, SkillData> SkillDic { get; private set; } = new Dictionary<int, SkillData>();

    public void Initialize()
    {
        LoadItemData();
        LoadSklillData();
    }

    #region Item
    private void LoadItemData()
    {
        string csvPath = $"{Application.dataPath}/1.Resources/Data/Excel/ItemData.csv";
        string jsonPath = $"{Application.dataPath}/1.Resources/Data/JsonData/ItemData.json";

        //csv파일이 존재하고, json파일이 최신화 되지 않으면 다시 읽어서 dic에 넣어줌
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

        //0번째 줄은 어떤 데이터인지 나타내기 때문에 1부터 시작
        for (int y = 1; y < lines.Length; y++)
        {
            // csv 한 줄을 한 단어씩 잘라 row에 저장함
            string[] row = lines[y].Replace("\r", "").Split(',');

            if (row.Length == 0 || string.IsNullOrEmpty(row[0])) continue;

            int i = 0;
            ItemData id = new ItemData
            {
                //순서대로 저장되었기 때문에 row에 저장된 단어를 하나씩 가져감
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
        string jsonStr = JsonConvert.SerializeObject(wrapper, Newtonsoft.Json.Formatting.Indented); // Formatting.Indented : 자동으로 라인 / 들여쓰기된 문자열을 리턴해
        File.WriteAllText($"{Application.dataPath}/1.Resources/Data/JsonData/{filename}Data.json", jsonStr);
        AssetDatabase.Refresh(); // 새로 생성된 json파일을 인식하게 하기 위한 초기화

        ItemDic = itemList.ToDictionary(c => c.DataId);
    }
    #endregion

    #region Skill
    private void LoadSklillData()
    {
        string csvPath = $"{Application.dataPath}/1.Resources/Data/Excel/SkillData.csv";
        string jsonPath = $"{Application.dataPath}/1.Resources/Data/JsonData/SkillData.json";

        //csv파일이 존재하고, json파일이 최신화 되지 않으면 다시 읽어서 dic에 넣어줌
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

        //0번째 줄은 어떤 데이터인지 나타내기 때문에 1부터 시작
        for (int y = 1; y < lines.Length; y++)
        {
            // csv 한 줄을 한 단어씩 잘라 row에 저장함
            string[] row = lines[y].Replace("\r", "").Split(',');

            if (row.Length == 0 || string.IsNullOrEmpty(row[0])) continue;

            int i = 0;
            SkillData id = new SkillData
            {
                //순서대로 저장되었기 때문에 row에 저장된 단어를 하나씩 가져감
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
        string jsonStr = JsonConvert.SerializeObject(wrapper, Newtonsoft.Json.Formatting.Indented); // Formatting.Indented : 자동으로 라인 / 들여쓰기된 문자열을 리턴해
        File.WriteAllText($"{Application.dataPath}/1.Resources/Data/JsonData/{filename}Data.json", jsonStr);
        AssetDatabase.Refresh(); // 새로 생성된 json파일을 인식하게 하기 위한 초기화

        SkillDic = skillList.ToDictionary(c => c.DataId);
    }
    #endregion

    #region Util
    //입력받은 문자열을 입력받은 T타입으로 변환
    private static T ConvertValue<T>(string value)
    {
        if (string.IsNullOrEmpty(value))
            return default(T);

        TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
        return (T)converter.ConvertFromString(value);
    }

    //csv에서 &'로 구분된 문자열을 T 타입의 리스트로 변환
    private static List<T> ConvertList<T>(string value)
    {
        if (string.IsNullOrEmpty(value))
            return new List<T>();

        return value.Split('&').Select(x => ConvertValue<T>(x)).ToList();
    }
    #endregion

    #region Data
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
    #endregion
}