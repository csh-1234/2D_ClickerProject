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

    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount,
    }


    #region »ö °ü¸®
    public static Color HexToColor(string color)
    {
        Color parsedColor;
        ColorUtility.TryParseHtmlString("#" + color, out parsedColor);

        return parsedColor;
    }

    public static readonly Color Normal =   HexToColor("FFFFFF");
    public static readonly Color Advanced = HexToColor("7575FF");
    public static readonly Color Rare =     HexToColor("CA58FF");
    public static readonly Color Legend =   HexToColor("FFFF36");
    public static readonly Color Myth =     HexToColor("FF5858");

    #endregion

    #region Data
    public enum EDataId
    {
        Player = 100000,
        Slime_Tanker = 100001,
        Slime_Attacker = 100002,
        Slime_Ranger = 100003,
    }

    #endregion
}
