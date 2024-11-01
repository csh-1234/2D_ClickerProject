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


public class DataManager
{
    public Dictionary<int, ItemData> ItemDic { get; private set; } = new Dictionary<int, ItemData>();

    public void Initialize()
    {
        LoadItemData();
    }

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
                ItemType = ConvertValue<string>(row[i++]),
                ItemRarity = ConvertValue<string>(row[i++]),
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

    [Serializable]
    public class ItemData
    {
     public int     DataId;
     public string  ItemName;
     public string  ItemType;
     public string  ItemRarity;
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
}