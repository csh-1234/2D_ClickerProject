using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using static DataManager;
using static Enums;

public class Skill : MonoBehaviour
{
    public void InitializeWithData(SkillData data)
    {
        _data = data;
        CurrentLevel = data.BaseLevel;

        // 필요한 데이터 초기화
        DataId = data.DataId;
        SkillName = data.SkillName;
        SkillType = data.SkillType;
        BaseLevel = data.BaseLevel;
        MaxLevel = data.MaxLevel;
        Cooldown = data.Cooldown;
        Duration = data.Duration;
        MaxDuration= data.MaxDuration;
        HpBonus= data.HpBonus;
        AtkBonus= data.AtkBonus;
        DefBonus= data.DefBonus;
        CriRateBonus= data.CriRateBonus;
        CriDamageBonus= data.CriDamageBonus;
        AtkSpeedBonus= data.AtkSpeedBonus;
        HealAmount= data.HealAmount;
        SkillInfo= data.SkillInfo;
        SpriteName= data.SpriteName;
        UpgradeCost = data.UpgradCost;
}
    protected SkillData _data;

    public SkillType skillType;

    public int DataId;
    public string SkillName;
    public SkillType SkillType;
    public int BaseLevel;
    public int MaxLevel;
    public float Cooldown;
    public float MaxCooldown;
    public float Duration;
    public float MaxDuration;
    public int HpBonus;
    public int AtkBonus;
    public int DefBonus;
    public float CriRateBonus;
    public float CriDamageBonus;
    public float AtkSpeedBonus;
    public int HealAmount;
    public int Damage;
    public string SkillInfo;
    public string SpriteName;
    public int UpgradeCost;

    public int CurrentLevel { get;  set; }

    //public string SkillName;
    //public string SkillInfo;
    //public int    SkillLevel;
    //public int    MaxSkillLevel;
    //public float  Cooldonwn;
    //public float  Duration;
    //public int    HpBonus;
    //public int    HealAmount;
    //public int    AtkBonus;
    //public int    DefBonus;
    //public float  AtkSpeedBonus;
    //public float  CriRateBonus;
    //public float  CriDamageBonus;
    //public float  MoveSpeedBonus;

    public int CurrentSkillLevel;
    public int CurrentMaxSkillLevel;
    public float CurrentCoolDonwn;
    public float CurrentDuration;
    public int CurrentHpBonus;
    public int CurrentHealAmount;
    public int CurrentAtkBonus;
    public int CurrentDefBonus;
    public float CurrentAtkSpeedBonus;
    public float CurrentCriRateBonus;
    public float CurrentCriDamageBonus;
    public float CurrentMoveSpeedBonus;

    public GameObject Prefab;

    private bool isInnitialized = false;
    public event Action<float> OnCooldownUpdate;  // 쿨타임 업데이트 이벤트

    public event Action OnBuffStart;
    public event Action OnBuffEnd;
    protected void InvokeBuffStart()
    {
        OnBuffStart?.Invoke();
    }

    protected void InvokeBuffEnd()
    {
        OnBuffEnd?.Invoke();
    }

    public float _cooldownEndTime = 0f;
    protected bool _isOnCooldown = false;

    public Dictionary<int, SkillData> SkillDic { get; private set; } = new Dictionary<int, SkillData>();

    public float CooldownRatio
    {
        get
        {
            if (!_isOnCooldown) return 1f;
            float remainTime = _cooldownEndTime - Time.time;

            return 1 - Mathf.Clamp01(remainTime / Cooldown);
        }
    }
    protected void UpdateCooldown()
    {
        OnCooldownUpdate?.Invoke(CooldownRatio);
    }
    public void InitCooldown()
    {
        _isOnCooldown = false;
        _cooldownEndTime = 0;
    }
    protected virtual void Awake()
    {
        Initialize();
        SkillDic = Managers.Instance.Data.SkillDic;
    }
    public virtual bool Initialize()
    {
        if (isInnitialized) { return false; }
        isInnitialized = true;
        return true;
    }
    protected virtual IEnumerator StartActiveSkill()
    {
        yield return null;
    }
    protected virtual IEnumerator StartBuffSkill()
    {
        yield return null;
    }
    public virtual IEnumerator StartSkill()
    {
        yield return null;
    }
    protected void StartPassiveSkill()
    {

    }

    public bool CanUpgrade() => CurrentLevel < _data.MaxLevel;

    public bool TryUpgrade(int currentGold)
    {
        if (!CanUpgrade() || currentGold < UpgradeCost)
        {
            Debug.Log("골드가 부족하거나 최대레벨 상한선");
            return false;
        }

        return true;
    }

    public virtual void UpdateSkillByLoadedLevel()
    {

    }
}
