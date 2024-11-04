using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Coffee.UIExtensions.UIParticleAttractor;
using static Enums;
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

    public override void SetInfo(int dataId)
    {
        base.SetInfo(dataId);
        ObjectType = ObjectType.Monster;
    }

    private void Update()
    {
        MoveMonster();
    }
    void MoveMonster()
    {
        if (Target == null) return;

        Vector2 direction = (Target.transform.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, Target.transform.position);
        Vector3 movment = direction * Time.deltaTime * MoveSpeed;

        if (distance > 1f)
        {
            transform.Translate(movment, Space.World);
        }
        //RigidBody.MovePosition(RigidBody.position + movment);
    }

    public event Action OnDeadEvent;
    public override void OnDead()
    {
        //print("���� ���");
        base.OnDead();
        //OnDeadEvent?.Invoke();
        Managers.Instance.Game.MonsterList.Remove(this);
        DropGold gold = Managers.Instance.Object.Spawn<DropGold>(this.transform.position, 0, "Gold");
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
