using System.Collections;
using System.Collections.Generic;
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


    void ProjectileMove()
    {
        Vector2 movment = fireDirection * Time.deltaTime * MoveSpeed;
        transform.Translate(movment);
        //RigidBody.MovePosition(RigidBody.position + movment);
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
