using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums
{
	public enum EObjectType
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
}
