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
using Formatting = Newtonsoft.Json.Formatting;


public class DataManager
{
    private string GetDataPath(string fileName, bool isLoad = true)
    {
        if (isLoad)
        {
#if UNITY_EDITOR
            return Path.Combine(Application.dataPath, "1.Resources/Data", fileName);
#else
        // 빌드된 버전에서는 StreamingAssets 폴더 사용
        string streamingPath = Path.Combine(Application.streamingAssetsPath, "Data", fileName);
        
        // 파일이 존재하는지 확인하고 로그 출력
        if (File.Exists(streamingPath))
        {
            Debug.Log($"Data file found at: {streamingPath}");
        }
        else
        {
            Debug.LogError($"Data file not found at: {streamingPath}");
        }
        
        return streamingPath;
#endif
        }
        else
        {
            // 저장 데이터용 경로
            string savePath = Path.Combine(Application.persistentDataPath, "SaveData", fileName);

            // 저장 폴더가 없다면 생성
            Directory.CreateDirectory(Path.GetDirectoryName(savePath));

            return savePath;
        }
    }

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
        string csvPath = GetDataPath("Excel/CreatureData.csv", true);
        string jsonPath = GetDataPath("JsonData/CreatureData.json", true);

        //csv파일이 존재하고, json파일이 최신화 되지 않으면 다시 읽어서 dic에 넣어줌
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
        string csvPath = GetDataPath($"Excel/{filename}Data.csv", true);
        string[] lines = File.ReadAllText(csvPath).Split("\n");

        //0번째 줄은 어떤 데이터인지 나타내기 때문에 1부터 시작
        for (int y = 1; y < lines.Length; y++)
        {
            // csv 한 줄을 한 단어씩 잘라 row에 저장함
            string[] row = lines[y].Replace("\r", "").Split(',');

            if (row.Length == 0 || string.IsNullOrEmpty(row[0])) continue;

            int i = 0;
            CreatureData cd = new CreatureData
            {
                //순서대로 저장되었기 때문에 row에 저장된 단어를 하나씩 가져감
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
        string jsonPath = GetDataPath($"JsonData/{filename}Data.json", true);
        string jsonStr = JsonConvert.SerializeObject(wrapper, Formatting.Indented);
        File.WriteAllText(jsonPath, jsonStr);


        CreatureDic = creatureList.ToDictionary(c => c.DataId);
    }
    #endregion

    #region Item
        private void LoadItemData()
    {
        string csvPath = GetDataPath("Excel/ItemData.csv", true);
        string jsonPath = GetDataPath("JsonData/ItemData.json", true);

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
        string csvPath = GetDataPath($"Excel/{filename}Data.csv", true);
        string[] lines = File.ReadAllText(csvPath).Split("\n");

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
        string jsonPath = GetDataPath($"JsonData/{filename}Data.json", true);
        string jsonStr = JsonConvert.SerializeObject(wrapper, Formatting.Indented);
        File.WriteAllText(jsonPath, jsonStr);

        ItemDic = itemList.ToDictionary(c => c.DataId);
    }
    #endregion

    #region Skill
    private void LoadSklillData()
    {
        string csvPath = GetDataPath("Excel/SkillData.csv", true);
        string jsonPath = GetDataPath("JsonData/SkillData.json", true);

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
        string csvPath = GetDataPath($"Excel/{filename}Data.csv", true);
        string[] lines = File.ReadAllText(csvPath).Split("\n");

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
        string jsonPath = GetDataPath($"JsonData/{filename}Data.json", true);
        string jsonStr = JsonConvert.SerializeObject(wrapper, Formatting.Indented);
        File.WriteAllText(jsonPath, jsonStr);

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