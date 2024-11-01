using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class StatData
{
    public string Name { get; private set; }
    public int Level { get; private set; }
    public float Bonus { get; private set; }
    public float BonusPerLevel { get; private set; }
    public int Cost { get; private set; }
    public int CostPerLevel { get; private set; }

    public StatData(string name, float bonusPerLevel, int costPerLevel)
    {
        Name = name;
        Level = 1;
        BonusPerLevel = bonusPerLevel;
        CostPerLevel = costPerLevel;
        UpdateValues();
    }

    public void LevelUp()
    {
        Level++;
        UpdateValues();
    }

    private void UpdateValues()
    {
        Bonus = Level * BonusPerLevel;
        Cost = Level * CostPerLevel;
    }

    public (int level, float bonus, int cost) GetInfo()
    {
        return (Level, Bonus, Cost);
    }
}


public class StatUpgradeManager
{
    private int HpBonus = 1;             //레벨당 1
    private int AtkBonus = 1;            //레벨당 1
    private int DefBonus = 1;            //레벨당 1
    private float CriRateBonus = 1f;   //레벨당 0.1
    private float criDamageBonus = 1;    //레벨당 1
    private float AtkSpeedBonus = 1f;    //레벨당 0.1

    //레벨당 코스트
    private int HpCost = 1;             //레벨당 1
    private int AtkCost = 1;            //레벨당 1
    private int DefCost = 1;            //레벨당 1
    private int CriRateCost = 2;        //레벨당 2
    private int criDamageCost = 2;      //레벨당 2
    private int AtkSpeedCost = 1;    //레벨당 1

    //public float MoveSpeedBonus;      //몬스터 전용

    private int AtkLevel = 1;
    private int HpLevel = 1;
    private int DefLevel = 1;
    private int CriRateLevel = 1;
    private int criDamageLevel = 1;
    private int AtkSpeedLevel = 1;
    //public float MoveSpeedLevel; 

    public void Initialize()
    {
        //TODO : 저장된 스텟레벨 Load
        AtkLevel = 1;
        HpLevel = 1;
        DefLevel = 1;
        CriRateLevel = 1;
        criDamageLevel = 1;
        AtkSpeedLevel = 1;

        //로드한 스텟레벨 적용
        Managers.Instance.Game.player.Hp += HpLevel * HpBonus;
        Managers.Instance.Game.player.Atk += AtkLevel * AtkBonus;
        Managers.Instance.Game.player.Def += DefLevel * DefBonus;
        Managers.Instance.Game.player.AttackSpeed += AtkSpeedLevel * AtkSpeedBonus;
        Managers.Instance.Game.player.CriDamage += CriRateLevel * CriRateBonus;
        Managers.Instance.Game.player.CriRate += criDamageLevel * criDamageBonus;
    }

    public event Action<string, int, int, int> OnStatChanged;


    public (int level, int bonus, int cost) GetStatInfo(string statType)
    {
        switch (statType)
        {
            case "AtkUpgrade":
                return (AtkLevel, AtkLevel * AtkBonus, AtkLevel * AtkCost);
            case "HpUgrade":
                return (HpLevel, HpLevel * HpBonus, HpLevel * HpCost);
            case "DefUpgrade":
                return (DefLevel, DefLevel * DefBonus, DefLevel * DefCost);
            case "AtkSpeedUpgrade":
                return (AtkSpeedLevel, (int)(AtkSpeedLevel * AtkSpeedBonus), AtkSpeedLevel * AtkSpeedCost);
            case "CriRateUpgrade":
                return (CriRateLevel, (int)(CriRateLevel * CriRateBonus), CriRateLevel * CriRateCost);
            case "CriDamageUpgrade":
                return (criDamageLevel, (int)(criDamageLevel * criDamageBonus), criDamageLevel * criDamageCost);
            default:
                return (0, 0, 0);
        }
    }

    public void statUpgrade(string name)
    {
        switch (name)
        {
            case "AtkUpgrade":
                UpgradeAtkLevel();
                break;
            case "HpUgrade":
                UpgradeHpLevel();
                break;
            case "DefUpgrade":
                UpgradeDefLevel();
                break;
            case "AtkSpeedUpgrade":
                UpgradeAtkSpeedLevel();
                break;
            case "CriRateUpgrade":
                UpgradeCriRateLevel();
                break;
            case "CriDamageUpgrade":
                UpgradeCriDamageLevel();
                break;
            default:
                break;
        }
    }

    public void UpgradeAtkLevel()
    {
        AtkLevel++;
        OnStatChanged?.Invoke("AtkUpgrade", AtkLevel, AtkLevel * AtkBonus, AtkLevel * AtkCost);
        Managers.Instance.Game.player.Atk += 1;
    }
    public void UpgradeHpLevel()
    {
        HpLevel++;
        OnStatChanged?.Invoke("HpUgrade", HpLevel, HpLevel * HpBonus, HpLevel * HpCost);
        Managers.Instance.Game.player.Hp += 1;
        Managers.Instance.Game.player.MaxHp += 1;
    }
    public void UpgradeDefLevel()
    {
        DefLevel++;
        OnStatChanged?.Invoke("DefUpgrade", DefLevel, DefLevel * DefBonus, DefLevel * DefCost);
        Managers.Instance.Game.player.Def += 1;
    }
    public void UpgradeAtkSpeedLevel()
    {
        AtkSpeedLevel++;
        OnStatChanged?.Invoke("AtkSpeedUpgrade", AtkSpeedLevel, (int)(AtkSpeedLevel * AtkSpeedBonus), AtkSpeedLevel * AtkSpeedCost);
        Managers.Instance.Game.player.AttackSpeed += 0.1f;
    }
    public void UpgradeCriRateLevel()
    {
        CriRateLevel++;
        OnStatChanged?.Invoke("CriRateUpgrade", CriRateLevel, (int)(CriRateLevel * CriRateBonus), CriRateLevel * CriRateCost);
        Managers.Instance.Game.player.CriRate += 0.1f;
    }
    public void UpgradeCriDamageLevel()
    {
        criDamageLevel++;
        OnStatChanged?.Invoke("CriDamageUpgrade", criDamageLevel, (int)(criDamageLevel * criDamageBonus), criDamageLevel * criDamageCost);
        Managers.Instance.Game.player.CriDamage += 1;
    }
}
