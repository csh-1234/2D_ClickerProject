using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Projectile : BaseObject
{
    private Creature _owner;
    private Monster target;
    private Vector2 fireDirection;
    private bool _isForcedCritical;  // 강제 크리티컬 여부 추가
    protected override void Awake()
    {
        base.Awake();
    }


    void Update()
    {
        //SetTarget();
        
    }

    private void FixedUpdate()
    {
        ProjectileMove();
    }

    public void SetInfo(Creature Owner, Vector2 firedir, bool isForcedCritical = false)
    {
        _owner = Owner;
        fireDirection = firedir;
        hasHit = false;
        _isForcedCritical = isForcedCritical;  // 크리티컬 정보 저장
    }

    private Vector3 startPos;
    public void Initialize(Vector2 direction)
    {
        fireDirection = direction.normalized;
        startPos = transform.position;
    }
    private bool hasHit = false;  // 충돌 여부를 체크하는 플래그 추가
    void ProjectileMove()
    {
        // 기본 이동
        if (hasHit) return;
        Vector2 movement = fireDirection * Time.deltaTime * 10f;
        transform.Translate(movement);
        //RigidBody.AddForce(transform.up * -1 * RigidBody.gravityScale);


        //RigidBody.MovePosition(RigidBody.position + movement);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;
        if (_owner.ObjectType == Enums.ObjectType.Player &&  collision.gameObject.tag == "Monster")
        {
            hasHit = true;  // 충돌 플래그 설정
            print("투사체가 몬스터 때림");
            GameObject go = Managers.Instance.Resource.Instantiate("HitEffect2");
            go.transform.position = transform.position  ;
            
            Monster monster = collision.gameObject.GetComponent<Monster>();
            if (monster != null)
            {
                // 강제 크리티컬이면 크리티컬 확률을 100%로 설정
                if (_isForcedCritical)
                {
                    Player player = _owner as Player;
                    float originalCritRate = player._currentStats.CriticalRate;
                    player._currentStats.CriticalRate = 100f;

                    monster.OnDamaged(_owner, _owner.Atk);

                    // 원래 크리티컬 확률로 복구
                    player._currentStats.CriticalRate = originalCritRate;
                }
                else
                {
                    monster.OnDamaged(_owner, _owner.Atk);
                }
            }
            Managers.Instance.Object.Despawn(this);
        }

        if (_owner.ObjectType == Enums.ObjectType.Monster && collision.gameObject.tag == "Player")
        {
            hasHit = true;  // 충돌 플래그 설정
            print("투사체가 플레이어 때림");
            collision.gameObject.GetComponent<Player>().OnDamaged(_owner, _owner.Atk);
            Managers.Instance.Object.Despawn(this);
        }
    }
}
