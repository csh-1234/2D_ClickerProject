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
