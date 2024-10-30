using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Projectile : BaseObject
{
    private Monster target;
    private Vector2 fireDirection;
    //protected override void Awake()
    //{
    //    base.Awake();
    //    Atk = 10;
    //}

    
    void Update()
    {
        //SetTarget();
        
    }

    private void FixedUpdate()
    {
        ProjectileMove();
    }

    public void SetInfo(Vector2 firedir)
    {
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
        Vector2 movement = fireDirection * Time.deltaTime * MoveSpeed;
        transform.Translate(movement);
        RigidBody.AddForce(transform.up * -1 * RigidBody.gravityScale);


        //RigidBody.MovePosition(RigidBody.position + movement);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Monster")       
        {
            print("����ü�� ���� ����");
            collision.gameObject.GetComponent<Monster>().OnDamaged(this, 100);
            Destroy(this.gameObject);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            print("����ü�� �� ����");
            Destroy(this.gameObject);
        }
    }
    //public void SetInfo(int damage)
    //{
    //    this.Atk = damage;
    //}


}
