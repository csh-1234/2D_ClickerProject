using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Player : BaseObject
{
    private Vector2 _input;
    public Projectile projectile;
    private Monster target;
    private Coroutine shootingCoroutine;
    [SerializeField] private float _fireRate = 1f;  // 발사 간격 (초)
    private float _nextFireTime;  // 다음 발사 가능 시간
    protected override void Awake()
    {
        base.Awake();
        Managers.Instance.Game.player = this;
        MaxHp = 1000;
        Hp = 1000;
        Atk = 10;
        Def = 3;
        CriRate = 20f;
        CriDamage = 100f;
        MoveSpeed = 5f;
        _nextFireTime = 0f;
    }

    private void Start()
    {
    }

    private void Update()
    {
        PlayerInput();
        SetTarget();
    }
    private void FixedUpdate()
    {
        MovePlayer();
        CheckMonsterListAndControlShooting();
    }

    Vector2 fireDir;

    void PlayerInput()
    {
        _input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }    

    void MovePlayer()
    {
        Vector2 movement = _input.normalized * Time.fixedDeltaTime * MoveSpeed;
        RigidBody.MovePosition(RigidBody.position + movement);
    }

    void SetTarget()
    {
        float distance = float.MaxValue;
        if (Managers.Instance.Game.MonsterList.Count != 0)
        {
            foreach (Monster monster in Managers.Instance.Game.MonsterList)
            {
                if ((monster.transform.position - transform.position).magnitude < distance)
                {
                    distance = (monster.transform.position - transform.position).magnitude;
                    target = monster;
                }
            }
        }
        fireDir = (target.transform.position- transform.position).normalized;
    }

    IEnumerator ShootProjectile()
    {
        while (true)
        {
            yield return new WaitForSeconds(_fireRate);  // 1초 대기
            if (Managers.Instance.Game.MonsterList.Count > 0 && target != null)
            {
                Projectile proj = Instantiate(projectile, transform.position, Quaternion.identity);
                proj.SetInfo(fireDir);
            }
        }
    }

    private void CheckMonsterListAndControlShooting()
    {
        bool hasMonsters = Managers.Instance.Game.MonsterList.Count > 0;

        if (hasMonsters && shootingCoroutine == null)
        {
            // 몬스터가 있고 발사 중이 아니면 시작
            shootingCoroutine = StartCoroutine(ShootProjectile());
        }
        else if (!hasMonsters && shootingCoroutine != null)
        {
            // 몬스터가 없고 발사 중이면 중지
            StopCoroutine(shootingCoroutine);
            shootingCoroutine = null;
        }
    }


    public override void OnDamaged(BaseObject attacker, int damage)
    {

    }

    public override void OnDead()
    {
        base.OnDead();
    }

    

}