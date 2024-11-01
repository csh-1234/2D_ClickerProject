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
        Noraml,         //�Ϲ�(��)
        Advanced,       //���(��)
        Rare,           //���(��)
        Legend,         //����(��)
        Myth,           //��ȭ(��)
    }

    public enum ItemType
    {
        Weapon,
        Armor,
    }

}
