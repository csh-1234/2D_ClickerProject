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
        Normal,         //¿œπ›(»Ú)
        Advanced,       //∞Ì±ﬁ(∆ƒ)
        Rare,           //»Ò±Õ(∫∏)
        Legend,         //¿¸º≥(≥Î)
        Myth,           //Ω≈»≠(ª°)
    }

    public enum ItemType
    {
        Weapon,
        Armor,
    }

    #region ¿”Ω√ √≥∏Æ
    public static Color HexToColor(string color)
    {
        Color parsedColor;
        ColorUtility.TryParseHtmlString("#" + color, out parsedColor);

        return parsedColor;
    }

    public static readonly Color Noraml =   HexToColor("FFFFFF");
    public static readonly Color Advanced = HexToColor("6e6eff");
    public static readonly Color Rare =     HexToColor("ae00ff");
    public static readonly Color Legend =   HexToColor("ffff00");
    public static readonly Color Myth =     HexToColor("ff4d4d");

    #endregion
}
