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
        // �⺻ ���� ����

        originalScale = transform.localScale;
    }

    private void Start()
    {
        Target = Managers.Instance.Game.player;
        // ���� ���ȿ� �⺻ ���� ����

        StartPulseEffect();
    }

    public override void SetInfo(int dataId)
    {
        base.SetInfo(dataId);
        ObjectType = ObjectType.Monster;

        // �⺻ ���� ����
        int baseHp = _baseStats.Hp;
        int baseAtk = _baseStats.Attack;
        int baseDef = _baseStats.Defense;

        // �⺻ ������ ���� ���ȿ� ����
        _currentStats.CopyStats(_baseStats);

        // �������� ������ ���� ���� ����
        float difficultyMultiplier = Managers.Instance.Stage.DifficultyByLevel;

        // ���� ���� (�Ҽ��� ���� ����)
        _currentStats.Hp = Mathf.FloorToInt(baseHp * difficultyMultiplier);
        _currentStats.MaxHp = _currentStats.Hp;  // MaxHp�� ���� ����
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
    private bool hasStartedShooting = false;  // �߻� ���� ���� üũ
    void MoveMonster()
    {
        if (Target == null || Target.Hp <= 0)
        {
            hasStartedShooting = false;  // �÷��̾ ������ �߻� ����
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
        // ���� �÷��̾���� ������ ����
        Vector2 directionFromPlayer = (transform.position - Target.transform.position).normalized;

        // ���� ��ġ���� �ش� �������� ���� �Ÿ���ŭ �̵�
        Vector3 targetPosition = transform.position + (Vector3)(directionFromPlayer * 4f); // 3f�� ���� �Ÿ�

        // DOTween�� ����Ͽ� �ε巴�� �̵�
        transform.DOMove(targetPosition, duration)
            .SetEase(Ease.OutQuad);
    }
    public void StopRetreat()
    {
        transform.DOKill(); // ���� ���� ���� DOTween �ִϸ��̼� ����
    }


    [SerializeField] private float _fireRate = 1f;  // �߻� ���� (��)
    Vector2 fireDir;

    IEnumerator ShootProjectile()
    {
        while (hasStartedShooting && Target != null && Target.Hp > 0)  // �÷��̾� ü�� üũ �߰�
        {
            yield return new WaitForSeconds(1f / _currentStats.AttackSpeed);
            fireDir = (Target.transform.position - transform.position).normalized;
            if (Managers.Instance.Game.player != null)
            {
                Projectile proj = Managers.Instance.Object.Spawn<Projectile>(CenterPosition, 0, "MonsterProjectile");
                proj.SetInfo(this, fireDir);
            }

            
        }

        hasStartedShooting = false;  // ���� ���� �� �߻� ���� ����
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
        //Managers.Instance.Sound.Play("SlimeDie", SoundManager.Sound.Effect);
        int goldAmount = (int)(Random.Range(100, 1000) * Managers.Instance.Stage.DifficultyByLevel);
        Managers.Instance.Currency.AddGold(goldAmount);
        

        UI_GoldEffect.Instance.PlayGoldEffect(transform.position, goldAmount);
        Managers.Instance.Object.Despawn(this);

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
