using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static DataManager;
using Random = UnityEngine.Random;

public class Creature : BaseObject
{
    public Rigidbody2D RigidBody { get; set; }
    public Collider2D Collider { get; set; }
    public SpriteRenderer SpriteRender { get; set; }
    public Animator Anim { get; set; }
    public Vector2 CenterPosition { get { return transform.position; } }


    [SerializeField] public Stats _baseStats = new Stats();    // 기본 스탯
    [SerializeField] public Stats _currentStats = new Stats(); // 최종 함산 스탯

    public CreatureData CreatureData { get; set; }

    public event Action<int> OnHpChanged;   //체력변화 감지 이벤트

    public int Hp
    {
        get { return _currentStats.Hp; }
        set
        {
            _currentStats.Hp = Mathf.Clamp(value, 0, _currentStats.MaxHp);
            OnHpChanged?.Invoke(_currentStats.Hp);
        }
    }
    public int DataId { get; protected set; }
    public int MaxHp => _currentStats.MaxHp;
    public int Atk => _currentStats.Attack;
    public int Def => _currentStats.Defense;
    public float CriRate => _currentStats.CriticalRate;
    public float CriDamage => _currentStats.CriticalDamage;
    public float MoveSpeed => _currentStats.MoveSpeed;
    public float AttackSpeed => _currentStats.AttackSpeed;


    public virtual void SetInfo(int dataId)
    {
        DataId = dataId;
        if(ObjectType == Enums.ObjectType.Player)
        {
            CreatureData = Managers.Instance.Data.CreatureDic[dataId];
        }
        else if( ObjectType == Enums.ObjectType.Monster)
        {
            CreatureData = Managers.Instance.Data.CreatureDic[dataId];
        }

        gameObject.name = $"{CreatureData.DataId}_{CreatureData.DataId}";

        _baseStats.MaxHp = CreatureData.MaxHp;
        _baseStats.Hp = CreatureData.Hp;
        _baseStats.Attack = CreatureData.Atk;
        _baseStats.Defense = CreatureData.Def;
        _baseStats.CriticalRate = CreatureData.CriRate;
        _baseStats.CriticalDamage = CreatureData.CriDamage;
        _baseStats.MoveSpeed = CreatureData.MoveSpeed;
        _baseStats.AttackSpeed = CreatureData.AtkSpeed;
        if(SpriteRender != null && CreatureData.SpriteName != null)
        {
            SpriteRender.sprite = Managers.Instance.Resource.Load<Sprite>(CreatureData.SpriteName);
        }
        Initialize();
    }


    private BuffManagement _buffManagement = new BuffManagement();
    public virtual void ResetStats(Stats baseStats)
    {
        _currentStats.CopyStats(baseStats);
    }

    public virtual void ApplyStatModifier(Stats modifier, bool isOn)
    {
        if (modifier == null) return;

        Debug.Log($"버프 적용 전 스탯 - ATK: {_currentStats.Attack}, DEF: {_currentStats.Defense}");

        if (isOn)
        {
            _currentStats.AddStats(modifier);
            Debug.Log($"버프 추가 후 스탯 - ATK: {_currentStats.Attack}, DEF: {_currentStats.Defense}");
        }
        else
        {
            _currentStats.SubStats(modifier);
        }
    }

    // 장비 변경시 사용할 메서드 추가
    public virtual void UpdateBaseStats(Stats newStats)
    {
        _currentStats.CopyStats(newStats);  // 기본 스탯 설정시에만 CopyStats 사용
    }

    public void ApplyBuff(string buffId, Stats buffStats, float duration)
    {
        _buffManagement.ApplyBuff(this, buffId, buffStats, duration);
    }

    public void RemoveBuff(string buffId)
    {
        _buffManagement.RemoveBuff(this, buffId);
    }

    protected override void Awake()
    {
        base.Awake();
        Initialize();
    }
    public override bool Initialize()
    {
        base.Initialize();

        RigidBody = GetComponent<Rigidbody2D>();
        Collider = GetComponent<Collider2D>();
        SpriteRender = GetComponent<SpriteRenderer>();
        Anim = GetComponent<Animator>();
        _currentStats.CopyStats(_baseStats);
        return true;
    }

    public virtual void Applybuff(Stats modifier, bool isOn = true)
    {
        if (isOn)
            _currentStats.AddStats(modifier);
        else
            _currentStats.SubStats(modifier);
    }

    public virtual void OnDamaged(BaseObject attacker, int damage)
    {
        bool isCritical = false;
        if (attacker is Player player)
        {
            if (Random.Range(0f, 100f) <= player.CriRate)
            {
                damage = Mathf.RoundToInt(damage * player.CriDamage);
                isCritical = true;
            }
        }
        // 방어력 적용
        damage = Mathf.Max(1, damage - Def);

        _currentStats.Hp -= damage;
        Managers.Instance.Object.ShowDamageFont(Collider.bounds.center, damage, 0, transform, isCritical);

        if (Hp <= 0)
        {
            OnDead();
        }
    }
    public virtual void OnDead()
    {

    }
}
