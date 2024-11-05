using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Coffee.UIExtensions.UIParticleAttractor;
using static Enums;
public class Monster : Creature
{
    private Player Target;
    private Vector2 originalScale;
    public Projectile projectile;
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
        originalScale = transform.localScale;
    }

    private void Start()
    {
        Target = Managers.Instance.Game.player;
        // ���� ���ȿ� �⺻ ���� ����
        _currentStats.CopyStats(_baseStats);

        StartPulseEffect();
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
    private bool hasStartedShooting = false;  // �߻� ���� ���� üũ
    void MoveMonster()
    {

        if (Target == null) return;
        Vector2 direction = (Target.transform.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, Target.transform.position);
        Vector3 movment = direction * Time.deltaTime * MoveSpeed;

        if (DataId == (int)EDataId.Slime_Ranger )
        {
            if (distance > 5f)
            {
                transform.Translate(movment, Space.World);
                hasStartedShooting = false;  // �Ÿ��� �־����� �߻� ���� ����
            }
            else if (!hasStartedShooting)  // ó������ �����Ÿ� �ȿ� ������ ��
            {
                hasStartedShooting = true;
                StartCoroutine(ShootProjectile());
            }
        }
        else
        {
            if (distance > 1f)
            {
                transform.Translate(movment, Space.World);
            }
        }

        
        //RigidBody.MovePosition(RigidBody.position + movment);
    }

    [SerializeField] private float _fireRate = 1f;  // �߻� ���� (��)
    Vector2 fireDir;

    IEnumerator ShootProjectile()
    {
        // ù �߻�� �ٷ�
        fireDir = (Target.transform.position - transform.position).normalized;
        if (Managers.Instance.Game.player != null)
        {
            Projectile proj = Instantiate(projectile, transform.position, Quaternion.identity);
            proj.SetInfo(this, fireDir);
        }

        while (hasStartedShooting)  // �߻� ���°� true�� ���ȸ� �ݺ�
        {
            yield return new WaitForSeconds(_fireRate);

            fireDir = (Target.transform.position - transform.position).normalized;
            if (Managers.Instance.Game.player != null)
            {
                Projectile proj = Instantiate(projectile, transform.position, Quaternion.identity);
                proj.SetInfo(this, fireDir);
            }
        }
    }
    //IEnumerator ShootProjectile()
    //{
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(_fireRate);  // 1�� ���

    //        fireDir = (Target.transform.position - transform.position).normalized;
    //        if (Managers.Instance.Game.player != null)
    //        {
    //            Projectile proj = Instantiate(projectile, transform.position, Quaternion.identity);
    //            proj.SetInfo(this, fireDir);
    //        }
    //    }
    //}

    public event Action OnDeadEvent;
    public override void OnDead()
    {
        //print("���� ���");
        base.OnDead();
        //OnDeadEvent?.Invoke();
        Managers.Instance.Game.MonsterList.Remove(this);    
        //DropGold gold = Managers.Instance.Object.Spawn<DropGold>(this.transform.position, 0, "Gold");
        int goldAmount = 100; // ���� ��, �����δ� ���� �����Ϳ��� ��������
        Managers.Instance.Currency.AddGold(goldAmount);

        UI_GoldEffect.Instance.PlayGoldEffect(transform.position, goldAmount);
        Destroy(gameObject);
        
    }

    Coroutine _coStartDamage;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        print("�÷��̾� ����");
        Player target = collision.gameObject.GetComponent<Player>();

        if (collision.gameObject.tag == "Player")
        {
            if (_coStartDamage != null)
                StopCoroutine(_coStartDamage);

            _coStartDamage = StartCoroutine(CoStartDamage(target));
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Player target = collision.gameObject.GetComponent<Player>(); // �浹�� ������Ʈ(�÷��̾�)�� Ÿ������ �����Ѵ�

        if (collision.gameObject.tag == "Player")
        {
            if (_coStartDamage != null)
            {
                StopCoroutine(_coStartDamage);
            }
            _coStartDamage = null;
        }
    }
   
    public IEnumerator CoStartDamage(Player target)
    {
        while (true)
        {
            target.OnDamaged(this, Atk);
            
            yield return new WaitForSeconds(1f);
        }
    }
    void StartPulseEffect()
    {
        // ũ�⸦ ���� ũ��� Ȯ��� ũ�� ���̿��� �ݺ������� ����
        transform.DOScale(originalScale * 1.05f, 1f / 2)
            .SetEase(Ease.InOutSine) // �ε巯�� ȿ���� ���� Ease �Լ� ���
            .SetLoops(-1, LoopType.Yoyo) // ���� �ݺ�, Yoyo Ÿ������ ����
            .SetUpdate(true); // �ð� �����Ͽ� ������ ���� �ʵ��� ���� (���û���)
    }


}
