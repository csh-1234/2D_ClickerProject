using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject : RootInit
{
    public Rigidbody2D RigidBody { get; set; }
    public Collider2D Collider { get; set; }
    public SpriteRenderer SpriteRender { get; set; }
    public Vector3 CenterPosition { get { return Collider.bounds.center; } }
    public int Hp { get; set; } = 100;
    public int MaxHp { get; set; } = 100;
    public int Atk { get; set; } = 10;
    public int Def { get; set; } = 5;
    public float CriRate { get; set; } = 20f;
    public float CriDamage { get; set; } = 10f;
    public float MoveSpeed { get; set; } = 10f;

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
