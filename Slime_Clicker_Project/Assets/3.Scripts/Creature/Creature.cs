using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Creature : BaseObject
{
    public Rigidbody2D RigidBody { get; set; }
    public Collider2D Collider { get; set; }
    public SpriteRenderer SpriteRender { get; set; }
    public Animator Anim { get; set; }
    public Vector2 CenterPosition { get { return transform.position; } }

    Enums.CreatureState _creatureState = Enums.CreatureState.Moving;

    [SerializeField] public Stats _baseStats = new Stats();    // �⺻ ����
    [SerializeField] public Stats _currentStats = new Stats(); // ���� �Ի� ����

    public event Action<int> OnHpChanged;   //ü�º�ȭ ���� �̺�Ʈ

    public virtual Enums.CreatureState CreatureState
    {
        get { return _creatureState; }
        set
        {
            _creatureState = value;
            UpdateAnimation();
        }
    }

    public int Hp
    {
        get => _currentStats.Hp;
        set
        {
            _currentStats.Hp = Mathf.Clamp(value, 0, _currentStats.MaxHp);
            OnHpChanged?.Invoke(_currentStats.Hp);
        }
    }
    public int MaxHp => _currentStats.MaxHp;
    public int Atk => _currentStats.Attack;
    public int Def => _currentStats.Defense;
    public float CriRate => _currentStats.CriticalRate;
    public float CriDamage => _currentStats.CriticalDamage;
    public float MoveSpeed => _currentStats.MoveSpeed;
    public float AttackSpeed => _currentStats.AttackSpeed;


    private BuffManagement _buffManagement = new BuffManagement();
    public virtual void ResetStats(Stats baseStats)
    {
        _currentStats.CopyStats(baseStats);
    }

    public virtual void ApplyStatModifier(Stats modifier, bool isOn)
    {
        if (modifier == null) return;

        Debug.Log($"���� ���� �� ���� - ATK: {_currentStats.Attack}, DEF: {_currentStats.Defense}");

        if (isOn)
        {
            _currentStats.AddStats(modifier);
            Debug.Log($"���� �߰� �� ���� - ATK: {_currentStats.Attack}, DEF: {_currentStats.Defense}");
        }
        else
        {
            _currentStats.SubStats(modifier);
        }
    }

    // ��� ����� ����� �޼��� �߰�
    public virtual void UpdateBaseStats(Stats newStats)
    {
        _currentStats.CopyStats(newStats);  // �⺻ ���� �����ÿ��� CopyStats ���
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
    public virtual void UpdateAnimation() { }
    protected virtual void UpdateIdle() { }
    protected virtual void UpdateSkill() { }
    protected virtual void UpdateMoving() { }
    protected virtual void UpdateDead() { }
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
        // ���� ����
        damage = Mathf.Max(1, damage - Def);

        Hp -= damage;
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
