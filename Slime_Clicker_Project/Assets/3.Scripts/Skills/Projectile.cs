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

    public float arcHeight = 5f;   // 포물선 높이
    public float arcSpeed = 10f;    // 포물선 주기

    private float distance = 0f;
    private Vector3 startPos;
    public void Initialize(Vector2 direction)
    {
        fireDirection = direction.normalized;
        startPos = transform.position;
    }

    void ProjectileMove()
    {
        // 기본 이동
        Vector2 movement = fireDirection * Time.deltaTime * MoveSpeed;
        transform.Translate(movement);
        RigidBody.AddForce(transform.up * -1 * RigidBody.gravityScale);


        //RigidBody.MovePosition(RigidBody.position + movement);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Monster")       
        {
            print("투사체가 몬스터 때림");
            collision.gameObject.GetComponent<Monster>().OnDamaged(this, 100);
            Destroy(this.gameObject);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            print("투사체가 벽 때림");
            Destroy(this.gameObject);
        }
    }
    //public void SetInfo(int damage)
    //{
    //    this.Atk = damage;
    //}


}
