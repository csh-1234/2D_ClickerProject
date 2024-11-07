using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static Enums;

// 스탯 업그레이드로 증가하는 스탯
[Serializable]    
public class Stats
{

    [SerializeField] private int hp;
    [SerializeField] private int maxHp;
    [SerializeField] private int attack;
    [SerializeField] private int defense;
    [SerializeField] private float criticalRate;
    [SerializeField] private float criticalDamage;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float attackSpeed;

    // 프로퍼티는 필드를 사용하도록 수정
    public int Hp { get => hp; set => hp = value; }
    public int MaxHp { get => maxHp; set => maxHp = value; }
    public int Attack { get => attack; set => attack = value; }
    public int Defense { get => defense; set => defense = value; }
    public float CriticalRate { get => criticalRate; set => criticalRate = value; }
    public float CriticalDamage { get => criticalDamage; set => criticalDamage = value; }
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public float AttackSpeed { get => attackSpeed; set => attackSpeed = value; }

    public void CopyStats(Stats other)
    {
        Hp = other.Hp;
        MaxHp = other.MaxHp;
        Attack = other.Attack;
        Defense = other.Defense;
        CriticalRate = other.CriticalRate;
        CriticalDamage = other.CriticalDamage;
        MoveSpeed = other.MoveSpeed;
        AttackSpeed = other.AttackSpeed;
    }

    public void AddStats(Stats other)
    {
        if (other == null)
        {
            Debug.LogWarning("Trying to add null stats!");
            return;
        }

        MaxHp += other.MaxHp;
        Hp = Mathf.Min(Hp, MaxHp);  // 이 부분 수정
        Attack += other.Attack;
        Defense += other.Defense;
        CriticalRate += other.CriticalRate;
        CriticalDamage += other.CriticalDamage;
        MoveSpeed += other.MoveSpeed;
        AttackSpeed += other.AttackSpeed;
    }

    public void SubStats(Stats other)
    {
        MaxHp -= other.MaxHp;
        Attack -= other.Attack;
        Defense -= other.Defense;
        CriticalRate -= other.CriticalRate;
        CriticalDamage -= other.CriticalDamage;
        MoveSpeed -= other.MoveSpeed;
        AttackSpeed -= other.AttackSpeed;
    }

    public void ClearStat()
    {
        Hp = 0;
        MaxHp = 0;
        Attack = 0;
        Defense = 0;
        CriticalRate = 0;
        CriticalDamage = 0;
        MoveSpeed = 0;
        AttackSpeed = 0;
    }

    public void AddStatByType(StatType type, float amount)
    {
        switch (type)
        {
            case StatType.Hp:
                Hp += (int)amount;
                break;
            case StatType.MaxHp:
                MaxHp += (int)amount;
                break;
            case StatType.Atk:
                Attack += (int)amount;  
                break;
            case StatType.Def:
                Defense += (int)amount; 
                break;
            case StatType.CriRate:
                criticalRate += amount;
                break;
            case StatType.CriDamage:
                CriticalDamage += amount;
                break;
            case StatType.MoveSpeed:
                MoveSpeed += amount;
                break;
            case StatType.AtkSpeed:
                attackSpeed += amount;  
                break;
        }
    }

    public void UpdatedByType(StatType type, float amount)
    {
        switch (type)
        {
            case StatType.Atk:
                Attack = (int)amount;  
                break;
            case StatType.Def:
                Defense = (int)amount;
                break;
            case StatType.AtkSpeed:
                attackSpeed = amount;
                break;
        }
    }
}
