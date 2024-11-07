using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Coffee.UIExtensions.UIParticleAttractor;
using static Enums;
using Random = UnityEngine.Random;
public class Monster : Creature
{
    private Player Target;
    private Vector2 originalScale;
    public Projectile projectile;

    //private void OnEnable()
    //{
    //    float ratio = Managers.Instance.Stage.DifficultyByLevel;
    //    _currentStats.Hp *= (int)ratio;
    //    _currentStats.MaxHp *= (int)ratio;
    //    _currentStats.Attack *= (int)ratio;
    //    _currentStats.Defense *= (int)ratio;

    //}
    protected override void Awake()
    {
        base.Awake();
        // 기본 스탯 설정

        originalScale = transform.localScale;
    }

    private void Start()
    {
        Target = Managers.Instance.Game.player;
        // 현재 스탯에 기본 스탯 복사

        StartPulseEffect();
    }

    public override void SetInfo(int dataId)
    {
        base.SetInfo(dataId);
        ObjectType = ObjectType.Monster;

        // 기본 스탯 저장
        int baseHp = _baseStats.Hp;
        int baseAtk = _baseStats.Attack;
        int baseDef = _baseStats.Defense;

        // 기본 스탯을 현재 스탯에 복사
        _currentStats.CopyStats(_baseStats);

        // 스테이지 레벨에 따른 스탯 보정
        float difficultyMultiplier = Managers.Instance.Stage.DifficultyByLevel;

        // 스탯 증가 (소수점 이하 버림)
        _currentStats.Hp = Mathf.FloorToInt(baseHp * difficultyMultiplier);
        _currentStats.MaxHp = _currentStats.Hp;  // MaxHp도 같이 설정
        _currentStats.Attack = Mathf.FloorToInt(baseAtk * difficultyMultiplier);
        _currentStats.Defense = Mathf.FloorToInt(baseDef * difficultyMultiplier);

        Debug.Log($"Monster {dataId} Stats - Level {Managers.Instance.Stage.CurrentStageLevel}:");
        Debug.Log($"Multiplier: {difficultyMultiplier}");
        Debug.Log($"Base HP: {baseHp} -> Current HP: {_currentStats.Hp}");
        Debug.Log($"Base ATK: {baseAtk} -> Current ATK: {_currentStats.Attack}");
        Debug.Log($"Base DEF: {baseDef} -> Current DEF: {_currentStats.Defense}");
    }

    private void Update()
    {
        MoveMonster();
    }
    private bool hasStartedShooting = false;  // 발사 시작 여부 체크
    void MoveMonster()
    {
        if (Target == null || Target.Hp <= 0)
        {
            hasStartedShooting = false;  // 플레이어가 죽으면 발사 중지
            return;
        }

        Vector2 direction = (Target.transform.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, Target.transform.position);
        Vector3 movment = direction * Time.deltaTime * MoveSpeed;

        if (DataId == (int)EDataId.Slime_Ranger)
        {
            if (distance > 4.3f)
            {
                transform.Translate(movment, Space.World);
                hasStartedShooting = false;
            }
            else if (!hasStartedShooting)
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
    }

    public void RetreatFromPlayer(float duration)
    {
        // 현재 플레이어와의 방향을 구함
        Vector2 directionFromPlayer = (transform.position - Target.transform.position).normalized;

        // 현재 위치에서 해당 방향으로 일정 거리만큼 이동
        Vector3 targetPosition = transform.position + (Vector3)(directionFromPlayer * 4f); // 3f는 후퇴 거리

        // DOTween을 사용하여 부드럽게 이동
        transform.DOMove(targetPosition, duration)
            .SetEase(Ease.OutQuad);
    }
    public void StopRetreat()
    {
        transform.DOKill(); // 현재 진행 중인 DOTween 애니메이션 중지
    }


    [SerializeField] private float _fireRate = 1f;  // 발사 간격 (초)
    Vector2 fireDir;

    IEnumerator ShootProjectile()
    {
        while (hasStartedShooting && Target != null && Target.Hp > 0)  // 플레이어 체력 체크 추가
        {
            yield return new WaitForSeconds(1f / _currentStats.AttackSpeed);
            fireDir = (Target.transform.position - transform.position).normalized;
            if (Managers.Instance.Game.player != null)
            {
                Projectile proj = Managers.Instance.Object.Spawn<Projectile>(CenterPosition, 0, "MonsterProjectile");
                proj.SetInfo(this, fireDir);
            }

            
        }

        hasStartedShooting = false;  // 루프 종료 시 발사 상태 해제
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
        //Managers.Instance.Sound.Play("SlimeDie", SoundManager.Sound.Effect);
        int goldAmount = (int)(Random.Range(100, 1000) * Managers.Instance.Stage.DifficultyByLevel);
        Managers.Instance.Currency.AddGold(goldAmount);
        

        UI_GoldEffect.Instance.PlayGoldEffect(transform.position, goldAmount);
        Managers.Instance.Object.Despawn(this);

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
