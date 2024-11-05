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
    private bool _isForcedCritical;  // ���� ũ��Ƽ�� ���� �߰�
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
        _isForcedCritical = isForcedCritical;  // ũ��Ƽ�� ���� ����
    }

    private Vector3 startPos;
    public void Initialize(Vector2 direction)
    {
        fireDirection = direction.normalized;
        startPos = transform.position;
    }
    private bool hasHit = false;  // �浹 ���θ� üũ�ϴ� �÷��� �߰�
    void ProjectileMove()
    {
        // �⺻ �̵�
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
            hasHit = true;  // �浹 �÷��� ����
            print("����ü�� ���� ����");
            GameObject go = Managers.Instance.Resource.Instantiate("HitEffect2");
            go.transform.position = transform.position  ;
            
            Monster monster = collision.gameObject.GetComponent<Monster>();
            if (monster != null)
            {
                // ���� ũ��Ƽ���̸� ũ��Ƽ�� Ȯ���� 100%�� ����
                if (_isForcedCritical)
                {
                    Player player = _owner as Player;
                    float originalCritRate = player._currentStats.CriticalRate;
                    player._currentStats.CriticalRate = 100f;

                    monster.OnDamaged(_owner, _owner.Atk);

                    // ���� ũ��Ƽ�� Ȯ���� ����
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
            hasHit = true;  // �浹 �÷��� ����
            print("����ü�� �÷��̾� ����");
            collision.gameObject.GetComponent<Player>().OnDamaged(_owner, _owner.Atk);
            Managers.Instance.Object.Despawn(this);
        }
    }
}
