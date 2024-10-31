using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Creature : BaseObject
{
    public Rigidbody2D RigidBody { get; set; }
    public Collider2D Collider { get; set; }
    public SpriteRenderer SpriteRender { get; set; }
    public Animator Anim { get; set; }
    public Vector2 CenterPosition { get { return transform.position; } }

    Enums.CreatureState _creatureState = Enums.CreatureState.Moving;
    public virtual Enums.CreatureState CreatureState
    {
        get { return _creatureState; }
        set
        {
            _creatureState = value;
            UpdateAnimation();
        }
    }

    [SerializeField] protected int _hp;
    [SerializeField] protected int _maxHp;
    [SerializeField] protected int _atk;
    [SerializeField] protected int _def;
    [SerializeField] protected float _atkSpeed;
    [SerializeField] protected float _criRate;
    [SerializeField] protected float _criDamage;
    [SerializeField] protected float _moveSpeed;

    public int Hp { get => _hp; set => _hp = value; }
    public int MaxHp { get => _maxHp; set => _maxHp = value; }
    public int Atk { get => _atk; set => _atk = value; }
    public int Def { get => _def; set => _def = value; }
    public float CriRate { get => _criRate; set => _criRate = value; }
    public float CriDamage { get => _criDamage; set => _criDamage = value; }
    public float MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }
    public float AttackSpeed { get => _atkSpeed; set => _atkSpeed = value; }


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

        return true;
    }

    public virtual void UpdateAnimation() { }
    protected virtual void UpdateIdle() { }
    protected virtual void UpdateSkill() { }
    protected virtual void UpdateMoving() { }
    protected virtual void UpdateDead() { }

    public virtual void OnDamaged(BaseObject attacker, int damage)
    {
        bool isCritical = false;
        Player player = attacker as Player; // player만 크리티컬 판정
        if (player != null)
        {
            //크리티컬
            if (UnityEngine.Random.Range(0, 100) <= player.CriRate)
            {
                damage *= 2;
                isCritical = true;
            }
        }
        Hp -= damage;
        Managers.Instance.Object.ShowDamageFont(Collider.bounds.center, damage, 0, transform, isCritical);
        if (Hp <= 0)
        {
            Hp = 0;
            OnDead();
        }
    }
    public virtual void OnDead()
    {

    }
}
