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
        string jsonStr = JsonConvert.SerializeObject(wrapper, Newtonsoft.Json.Formatting.Indented); // Formatting.Indented : �ڵ����� ���� / �鿩����� ���ڿ��� ������
        File.WriteAllText($"{Application.dataPath}/1.Resources/Data/JsonData/{filename}Data.json", jsonStr);
        AssetDatabase.Refresh(); // ���� ������ json������ �ν��ϰ� �ϱ� ���� �ʱ�ȭ

        ItemDic = itemList.ToDictionary(c => c.DataId);
    }



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