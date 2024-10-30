using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUpgradeManager
{
    //레벨당 코스트
    private int Skill1Cost = 100;    //레벨당 1
    private int Skill2Cost = 100;    //레벨당 1
    private int Skill3Cost = 100;    //레벨당 1
    private int Skill4Cost = 100;    //레벨당 2
    private int Skill5Cost = 100;    //레벨당 2
    private int Skill6Cost = 100;    //레벨당 1

    private int Skill1Level = 1;   
    private int Skill2Level = 1;   
    private int Skill3Level = 1;   
    private int Skill4Level = 2;   
    private int Skill5Level = 2;   
    private int Skill6Level = 1;   

    public void Initialize()
    {
        //TODO : 저장된 스텟레벨 Load
        //AtkLevel = 1;
        //HpLevel = 1;
        //DefLevel = 1;
        //CriRateLevel = 1;
        //criDamageLevel = 1;
        //AtkSpeedLevel = 1;

        //로드한 스텟레벨 적용
        //Managers.Instance.Game.player.Hp += HpLevel * HpBonus;
        //Managers.Instance.Game.player.Atk += AtkLevel * AtkBonus;
        //Managers.Instance.Game.player.Def += DefLevel * DefBonus;
        //Managers.Instance.Game.player.AttackSpeed += AtkSpeedLevel * AtkSpeedBonus;
        //Managers.Instance.Game.player.CriDamage += CriRateLevel * CriRateBonus;
        //Managers.Instance.Game.player.CriRate += criDamageLevel * criDamageBonus;
    }

    // name, level, cost
    public event Action<string, int, int> OnStatChanged;


    //public (int level, int cost) GetStatInfo(string statType)
    //{
    //    switch (statType)
    //    {
    //        //case "AtkUpgrade":
    //        //    return (AtkLevel, AtkLevel * AtkBonus, AtkLevel * AtkCost);
    //        //case "HpUgrade":
    //        //    return (HpLevel, HpLevel * HpBonus, HpLevel * HpCost);
    //        //case "DefUpgrade":
    //        //    return (DefLevel, DefLevel * DefBonus, DefLevel * DefCost);
    //        //case "AtkSpeedUpgrade":
    //        //    return (AtkSpeedLevel, (int)(AtkSpeedLevel * AtkSpeedBonus), AtkSpeedLevel * AtkSpeedCost);
    //        //case "CriRateUpgrade":
    //        //    return (CriRateLevel, (int)(CriRateLevel * CriRateBonus), CriRateLevel * CriRateCost);
    //        //case "CriDamageUpgrade":
    //        //    return (criDamageLevel, (int)(criDamageLevel * criDamageBonus), criDamageLevel * criDamageCost);
    //        //default:
    //        //    return (0, 0, 0);
    //    }
    //}

    public void statUpgrade(string name)
    {
        //switch (name)
        //{
        //    case "AtkUpgrade":
        //        UpgradeAtkLevel();
        //        break;
        //    case "HpUgrade":
        //        UpgradeHpLevel();
        //        break;
        //    case "DefUpgrade":
        //        UpgradeDefLevel();
        //        break;
        //    case "AtkSpeedUpgrade":
        //        UpgradeAtkSpeedLevel();
        //        break;
        //    case "CriRateUpgrade":
        //        UpgradeCriRateLevel();
        //        break;
        //    case "CriDamageUpgrade":
        //        UpgradeCriDamageLevel();
        //        break;
        //    default:
        //        break;
        //}
    }

    public void UpgradeSkill1Level()
    {
        Skill1Level++;
        OnStatChanged?.Invoke("Skill1Upgrade", Skill1Level, Skill1Level * Skill1Cost);
    }
    public void UpgradeSkill2Level()
    {
        Skill2Level++;
        OnStatChanged?.Invoke("Skill2Upgrade", Skill2Level, Skill2Level * Skill2Cost);
    }
    public void UpgradeSkill3Level()
    {
        Skill3Level++;
        OnStatChanged?.Invoke("Skill3Upgrade", Skill3Level, Skill2Level * Skill3Cost);
    }
    public void UpgradeSkill4Level()
    {
        Skill4Level++;
        OnStatChanged?.Invoke("Skill4Upgrade", Skill4Level, Skill3Level * Skill4Cost);
    }
    public void UpgradeSkill5Level()
    {
        Skill5Level++;
        OnStatChanged?.Invoke("Skill5Upgrade", Skill5Level, Skill4Level * Skill5Cost);
    }
    public void UpgradeSkill6Level()
    {
        Skill6Level++;
        OnStatChanged?.Invoke("Skill6Upgrade", Skill6Level, Skill5Level * Skill6Cost);
    }

}
