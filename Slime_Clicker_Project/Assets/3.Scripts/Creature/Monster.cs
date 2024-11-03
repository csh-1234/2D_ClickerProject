using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Coffee.UIExtensions.UIParticleAttractor;

public class Monster : Creature
{
    private Player Target;

    protected override void Awake()
    {
        base.Awake();
        // 기본 스탯 설정
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
        // 현재 스탯에 기본 스탯 복사
        _currentStats.CopyStats(_baseStats);
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
        //print("몬스터 사망");
        base.OnDead();
        //OnDeadEvent?.Invoke();
        Managers.Instance.Game.MonsterList.Remove(this);
        Destroy(gameObject);
        
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
