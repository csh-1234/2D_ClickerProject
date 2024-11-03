using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Creature
{
    private Player Target;

    protected override void Awake()
    {
        base.Awake();
        // �⺻ ���� ����
        _baseStats.MaxHp = 100;
        _baseStats.Hp = 100;
        _baseStats.Attack = 3;
        _baseStats.Defense = 1;
        _baseStats.CriticalRate = 0f;
        _baseStats.CriticalDamage = 0f;
        _baseStats.MoveSpeed = 3f;
        _baseStats.AttackSpeed = 1f;
    }

    private void Start()
    {
        Target = Managers.Instance.Game.player;
        // ���� ���ȿ� �⺻ ���� ����
        _currentStats.CopyStats(_baseStats);
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
        if (Target == null) return;
        Vector3 movment = (Target.transform.position - transform.position).normalized * Time.deltaTime * MoveSpeed;
        transform.Translate(movment, Space.World);
        //RigidBody.MovePosition(RigidBody.position + movment);
    }

    public event Action OnDeadEvent;
    public override void OnDead()
    {
        //print("���� ���");
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
