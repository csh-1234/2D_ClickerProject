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
        Noraml,         //ÀÏ¹Ý(Èò)
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

}
