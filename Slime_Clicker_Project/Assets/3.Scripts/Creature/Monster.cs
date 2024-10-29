using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : BaseObject
{

    private Player Target;

    protected override void Awake()
    {
        base.Awake();
        MaxHp = 100;
        Hp = 100;
        Atk = 3;
        Def = 1;
        CriRate = 0f;
        CriDamage = 0f;
        MoveSpeed = 3f;
    }

    private void Start()
    {
        Target = Managers.Instance.Game.player;
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        MoveMonster();
    }


    void MoveMonster()
    {
        Vector2 movment = (Target.transform.position - transform.position).normalized * Time.deltaTime * MoveSpeed;
        RigidBody.MovePosition(RigidBody.position + movment);
    }

    public override void OnDamaged(BaseObject attacker, int damage)
    {
        bool isCritical = false;
        Player player = attacker as Player;
        if (player != null)
        {
            //ũ��Ƽ��
            if (UnityEngine.Random.Range(0, 100) <= player.CriRate)
            {
                damage *= 2;
                isCritical = true;
            }
        }
        Hp -= damage;
        Managers.Instance.Object.ShowDamageFont(CenterPosition, damage, 0, transform, isCritical);
        if (Hp - damage <= 0)
        {
            OnDead();
        }
        else
        {
            Hp -= damage;
        }
    }

    public event Action OnDeadEvent;
    public override void OnDead()
    {
        print("���� ���");
        base.OnDead();
        //OnDeadEvent?.Invoke();
        Managers.Instance.Game.MonsterList.Remove(this);
        Destroy(gameObject);
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            print("���Ͱ� �÷��̾� ����");
            collision.gameObject.GetComponent<Player>().OnDamaged(this, 10);

        }
    }

}
