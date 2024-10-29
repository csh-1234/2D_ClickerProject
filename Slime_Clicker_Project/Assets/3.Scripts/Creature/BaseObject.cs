using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject : RootInit
{
    public Rigidbody2D RigidBody { get; set; }
    public Collider2D Collider { get; set; }
    public SpriteRenderer SpriteRender { get; set; }
    public Vector3 CenterPosition { get { return Collider.bounds.center; } }

    [SerializeField] protected int _hp;
    [SerializeField] protected int _maxHp;
    [SerializeField] protected int _atk;
    [SerializeField] protected int _def;
    [SerializeField] protected float _criRate;
    [SerializeField] protected float _criDamage;
    [SerializeField] protected float _moveSpeed;

    // 프로퍼티는 유지하되 내부 필드를 사용하도록 수정
    public int Hp { get => _hp; set => _hp = value; }
    public int MaxHp { get => _maxHp; set => _maxHp = value; }
    public int Atk { get => _atk; set => _atk = value; }
    public int Def { get => _def; set => _def = value; }
    public float CriRate { get => _criRate; set => _criRate = value; }
    public float CriDamage { get => _criDamage; set => _criDamage = value; }
    public float MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }

    public override bool Initialize()
    {
        if (base.Initialize() == false)
        {

            return false;
        }
        Collider = GetComponent<Collider2D>();
        RigidBody = GetComponent<Rigidbody2D>();
        SpriteRender = GetComponent<SpriteRenderer>();

        return true;
    }

    protected override void Awake()
    {
        base.Awake();
        Initialize();
    }

    public virtual void OnDamaged(BaseObject attacker, int damage)
    {
        bool isCritical = false;
        Player player = attacker as Player;
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
        //Managers.Instance.Object.ShowDamageFont(CenterPosition, damage, 0, transform, isCritical);
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
