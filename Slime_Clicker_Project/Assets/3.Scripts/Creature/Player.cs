using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;
using static UnityEngine.GraphicsBuffer;

public class Player : Creature
{
    [SerializeField] private float _fireRate = 1f;  // 발사 간격 (초)
    private Vector2 _input;
    public Projectile projectile;
    private Monster target;
    private Coroutine shootingCoroutine;
    public List<Skill> SkillList = new List<Skill>();
    private Dictionary<Skill, Coroutine> autoSkillCoroutines = new Dictionary<Skill, Coroutine>();
    public bool IsAuto
    {
        get => _isAuto;
        set
        {
            _isAuto = value;
            if (_isAuto)
            {
                StartAutoSkill();
            }
            else
            {
                StopAutoSkill();
            }
        }
    }
    public bool _isAuto = false;
    public bool _isCritical = false; // [스킬]일격필살 트리거 => 다음공격 확정치명
    private float _nextFireTime;  // 다음 발사 가능 시간
    public int count = 0;
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
        AttackSpeed = 1f;
        _nextFireTime = 0f;

        AddSkill(typeof(Skill_Zoomies));
        AddSkill(typeof(Skill_BakeBread));
        AddSkill(typeof(Skill_BeastEyes));
        AddSkill(typeof(Skill_CatCatPunch));
        AddSkill(typeof(Skill_FatalStrike));
        AddSkill(typeof(Skill_EatChur));
    }

    private void Start()
    {
        //GameObject skillObj = new GameObject($"Skill_Zoomies");
        //skillObj.transform.parent = transform;
        //Skill addskill = skillObj.AddComponent<Skill_Zoomies>();

        //if (!SkillList.Contains(addskill))
        //{
        //    SkillList.Add(addskill);
        //}

        //SkillList.AddRange(GetComponents<Skill>());
        //foreach (var skill in SkillList)
        //{
        //    // 각 스킬의 게임오브젝트를 활성화
        //    skill.gameObject.SetActive(true);
        //    SkillList.Add(skill);
        //}
        //foreach (Skill skill in SkillList)
        //{
        //    StartCoroutine(skill.StartSkill());
        //}
    }


    private void AddSkill(Type skillType)
    {
        GameObject skillObj = new GameObject($"Skill_{skillType.Name}");
        skillObj.transform.parent = transform;
        Skill skill = (Skill)skillObj.AddComponent(skillType);
        if (!SkillList.Contains(skill))
        {
            SkillList.Add(skill);
        }
    }

    private void Update()
    {
        AutoSkill();
        PlayerInput();
        SetTarget();
    }

    private void FixedUpdate()
    {
        MovePlayer();
        CheckMonsterListAndControlShooting();
    }

    private void AutoSkill()
    {
        if (IsAuto)
        {
            StartAutoSkill();
        }
        else
        {
            StopAutoSkill();
        }
    }

    private void StartAutoSkill()
    {
        foreach (Skill skill in SkillList)
        {
            if (!autoSkillCoroutines.ContainsKey(skill))
            {
                // StartCoroutine의 반환값(Coroutine)을 직접 Dictionary에 저장
                autoSkillCoroutines[skill] = StartCoroutine(AutoSkillActivate(skill));
            }
        }
    }

    private void StopAutoSkill()
    {
        foreach (var coroutine in autoSkillCoroutines.Values)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }
        autoSkillCoroutines.Clear();
    }

    private IEnumerator AutoSkillActivate(Skill skill)
    {
        while(true)
        {
            yield return StartCoroutine(skill.StartSkill());
            yield return new WaitForSeconds(skill.Cooldonwn);

        }
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
        fireDir = (target.transform.position - transform.position).normalized;
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

    public void shootProjectileOnce()
    {
        if (Managers.Instance.Game.MonsterList.Count > 0 && target != null)
        {
            Projectile proj = Instantiate(projectile, transform.position, Quaternion.identity);
            proj.SetInfo(fireDir);
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


    public IEnumerator OnDotHeal(float duration, int healAmount,int tickRate)   
    {
        float endTime = Time.time + duration;
        while (Time.time > endTime)
        {
            int CalcHp = Mathf.Max(MaxHp, Hp + healAmount);
            Hp += healAmount;
            yield return new WaitForSeconds(tickRate);
        }
    }

    public void OnHeal(int amount)
    {
        if (Hp + amount > MaxHp)
        {
            Hp = MaxHp;
        }
        else
        {
            Hp += amount;
        }
    }


    public override void OnDead()
    {
        base.OnDead();
    }

    

}