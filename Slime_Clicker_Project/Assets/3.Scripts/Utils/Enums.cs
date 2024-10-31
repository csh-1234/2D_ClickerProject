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

}
