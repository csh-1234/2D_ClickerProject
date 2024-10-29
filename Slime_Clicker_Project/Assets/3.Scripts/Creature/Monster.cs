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
        Debug.Log(Target.transform.position);
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
        print("몬스터 사망");
        base.OnDead();
        //OnDeadEvent?.Invoke();
        Managers.Instance.Game.MonsterList.Remove(this);
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            print("몬스터가 플레이어 때림");
            collision.gameObject.GetComponent<Player>().OnDamaged(this, 10);

        }
    }

}
