using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using static Enums;

public class Skill : MonoBehaviour
{
    public SkillType skillType;

    public string SkillName;
    public string SkillInfo;
    public int    SkillLevel;
    public int    MaxSkillLevel;
    public float  Cooldonwn;
    public float  Duration;
    public int    HpBonus;
    public int    HealAmount;
    public int    AtkBonus;
    public int    DefBonus;
    public float  AtkSpeedBonus;
    public float  CriRateBonus;
    public float  CriDamageBonus;
    public float  MoveSpeedBonus;

    public int    CurrentSkillLevel;
    public int    CurrentMaxSkillLevel;
    public float  CurrentCoolDonwn;
    public float  CurrentDuration;
    public int    CurrentHpBonus;
    public int    CurrentHealAmount;
    public int    CurrentAtkBonus;
    public int    CurrentDefBonus;
    public float  CurrentAtkSpeedBonus;
    public float  CurrentCriRateBonus;
    public float  CurrentCriDamageBonus;
    public float  CurrentMoveSpeedBonus;

    public GameObject Prefab;
    public SpriteRenderer Icon;

    private bool isInnitialized = false;


    public event System.Action<float> OnCooldownUpdate;  // 쿨타임 업데이트 이벤트

    protected float _cooldownEndTime = 0f;
    protected bool _isOnCooldown = false;

    public float CooldownRatio
    {
        get
        {
            if (!_isOnCooldown) return 1f;
            float remainTime = _cooldownEndTime - Time.time;
            
            return 1 - Mathf.Clamp01(remainTime / Cooldonwn);
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

}
