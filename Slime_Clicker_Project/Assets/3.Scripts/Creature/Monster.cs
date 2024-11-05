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
        // 기본 스탯 설정
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
        // 현재 스탯에 기본 스탯 복사
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
    private bool hasStartedShooting = false;  // 발사 시작 여부 체크
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
                hasStartedShooting = false;  // 거리가 멀어지면 발사 상태 리셋
            }
            else if (!hasStartedShooting)  // 처음으로 사정거리 안에 들어왔을 때
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

    [SerializeField] private float _fireRate = 1f;  // 발사 간격 (초)
    Vector2 fireDir;

    IEnumerator ShootProjectile()
    {
        // 첫 발사는 바로
        fireDir = (Target.transform.position - transform.position).normalized;
        if (Managers.Instance.Game.player != null)
        {
            Projectile proj = Instantiate(projectile, transform.position, Quaternion.identity);
            proj.SetInfo(this, fireDir);
        }

        while (hasStartedShooting)  // 발사 상태가 true인 동안만 반복
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
    //        yield return new WaitForSeconds(_fireRate);  // 1초 대기

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
        //print("몬스터 사망");
        base.OnDead();
        //OnDeadEvent?.Invoke();
        Managers.Instance.Game.MonsterList.Remove(this);    
        //DropGold gold = Managers.Instance.Object.Spawn<DropGold>(this.transform.position, 0, "Gold");
        int goldAmount = 100; // 예시 값, 실제로는 몬스터 데이터에서 가져오기
        Managers.Instance.Currency.AddGold(goldAmount);

        UI_GoldEffect.Instance.PlayGoldEffect(transform.position, goldAmount);
        Destroy(gameObject);
        
    }

    Coroutine _coStartDamage;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        print("플레이어 접촉");
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
        Player target = collision.gameObject.GetComponent<Player>(); // 충돌한 오브젝트(플레이어)를 타켓으로 설정한다

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
        // 크기를 원래 크기와 확대된 크기 사이에서 반복적으로 변경
        transform.DOScale(originalScale * 1.05f, 1f / 2)
            .SetEase(Ease.InOutSine) // 부드러운 효과를 위해 Ease 함수 사용
            .SetLoops(-1, LoopType.Yoyo) // 무한 반복, Yoyo 타입으로 설정
            .SetUpdate(true); // 시간 스케일에 영향을 받지 않도록 설정 (선택사항)
    }


}
