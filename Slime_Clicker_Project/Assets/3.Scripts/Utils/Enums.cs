using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums
{
	public enum ObjectType
	{
        Player,
        Monster,
        Boss,
        Projectile,
        Gold,
    }
    public enum CreatureState
    {
        Idle,
        Skill,
        Moving,
        OnDamaged,
        Dead
    }

    public enum SkillType
    {
        Active,
        Buff,
        Passive,
    }

    public enum ItemRarity
    {
        Normal,         //ÀÏ¹Ý(Èò)
        Advanced,       //°í±Þ(ÆÄ)
        Rare,           //Èñ±Í(º¸)
        Legend,         //Àü¼³(³ë)
        Myth,           //½ÅÈ­(»¡)
    }

    public enum ItemType
    {
        Weapon,
        Armor,
    }


    public enum StatType
    {
        Hp,
        MaxHp,
        Atk,
        Def,
        AtkSpeed,
        CriRate,
        CriDamage,
        MoveSpeed,
    }


    #region »ö °ü¸®
    public static Color HexToColor(string color)
    {
        Color parsedColor;
        ColorUtility.TryParseHtmlString("#" + color, out parsedColor);

        return parsedColor;
    }

    public static readonly Color Normal =   HexToColor("FFFFFF");
    public static readonly Color Advanced = HexToColor("6e6eff");
    public static readonly Color Rare =     HexToColor("ae00ff");
    public static readonly Color Legend =   HexToColor("ffff00");
    public static readonly Color Myth =     HexToColor("ff4d4d");

    #endregion

    #region Data
    public enum EDataId
    {
        Player = 100000,
        Slime_Yellow = 100001,
        Slime_Green = 100002,
        Slime_Purple = 100003,
    }

    #endregion
}
