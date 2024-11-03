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

    public void SetInfo(Creature Owner, Vector2 firedir)
    {
        _owner = Owner;
        fireDirection = firedir;
    }

    public float arcHeight = 5f;   // ������ ����
    public float arcSpeed = 10f;    // ������ �ֱ�

    private float distance = 0f;
    private Vector3 startPos;
    public void Initialize(Vector2 direction)
    {
        fireDirection = direction.normalized;
        startPos = transform.position;
    }

    void ProjectileMove()
    {
        // �⺻ �̵�
        Vector2 movement = fireDirection * Time.deltaTime * 10f;
        transform.Translate(movement);
        //RigidBody.AddForce(transform.up * -1 * RigidBody.gravityScale);


        //RigidBody.MovePosition(RigidBody.position + movement);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Monster")
        {
            print("����ü�� ���� ����");
            collision.gameObject.GetComponent<Monster>().OnDamaged(_owner, Managers.Instance.Game.player.Atk);
            Destroy(this.gameObject);
        }
    }
    //public void SetInfo(int damage)
    //{
    //    this.Atk = damage;
    //}


}
