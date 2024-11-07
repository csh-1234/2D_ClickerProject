using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
using static BuffManagement;
using static Coffee.UIExtensions.UIParticleAttractor;
using static DataManager;
using static Enums;

[SerializeField]
public class Player : Creature
{
    public Animator anim;
    [SerializeField] private float _fireRate = 1f;  // 발사 간격 (초)
    private Vector2 _input;
    public Projectile projectile;
    private Monster target;
    private Coroutine shootingCoroutine;
    public List<Skill> SkillList = new List<Skill>();
    private Dictionary<Skill, Coroutine> autoSkillCoroutines = new Dictionary<Skill, Coroutine>();
    private Dictionary<string, BuffInfo> _activeBuffs = new Dictionary<string, BuffInfo>();
    public event Action OnStatsChanged;
    [SerializeField] private Transform shootPos;


    public IEnumerable<BuffInfo> GetActiveBuffs()
    {
        return _activeBuffs.Values;
    }

    public override void ApplyStatModifier(Stats modifier, bool isOn)
    {
        if (modifier == null) return;

        // 버프 적용/제거 시 UpdatePlayerStats를 호출하지 않음
        if (isOn)
        {
            _currentStats.AddStats(modifier);
        }
        else
        {
            _currentStats.SubStats(modifier);
        }
    }

    public void ApplyBuff(string buffId, Stats buffStats, float duration)
    {
        // 기존 버프가 있다면 제거
        if (_activeBuffs.ContainsKey(buffId))
        {
            RemoveBuff(buffId);
        }

        // 새 버프 추가
        _activeBuffs[buffId] = new BuffInfo
        {
            BuffId = buffId,
            BuffStats = buffStats,
            EndTime = Time.time + duration
        };

        // 스탯에 버프 적용
        ApplyStatModifier(buffStats, true);

        // 전체 스탯 업데이트
        Managers.Instance.Game.UpdatePlayerStats();
    }

    public void RemoveBuff(string buffId)
    {
        if (_activeBuffs.TryGetValue(buffId, out BuffInfo buff))
        {
            ApplyStatModifier(buff.BuffStats, false);
            _activeBuffs.Remove(buffId);

            // 전체 스탯 업데이트
            Managers.Instance.Game.UpdatePlayerStats();
        }
    }


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
        if (Managers.Instance.Data.SkillDic == null || Managers.Instance.Data.SkillDic.Count == 0)
        {
            Debug.LogError("Skill data not loaded. Initializing Data Manager...");
            Managers.Instance.Data.Initialize();
        }
        Addskills();
        _currentStats.CopyStats(_baseStats);

        SetInfo(100000);
        anim = GetComponent<Animator>();
    }

    public override void SetInfo(int dataId)
    {
        base.SetInfo(dataId);
        ObjectType = ObjectType.Player;
        _currentStats.CopyStats(_baseStats);
    }

    private void Addskills()
    {
        if (Managers.Instance.Data.SkillDic == null || Managers.Instance.Data.SkillDic.Count == 0)
        {
            Debug.LogError("Skill data not loaded!");
            return;
        }

        try
        {
            AddSkill(typeof(Skill_Zoomies));
        AddSkill(typeof(Skill_BakeBread));
        AddSkill(typeof(Skill_EatChur));
        AddSkill(typeof(Skill_BeastEyes));
        AddSkill(typeof(Skill_FatalStrike));
            //AddSkill(typeof(Skill_CatCatPunch));
        }
        catch (Exception e)
        {
            Debug.LogError($"Error adding skills: {e.Message}\n{e.StackTrace}");
        }
    }

    private void AddSkill(Type skillType)
    {
        GameObject skillObj = new GameObject($"{skillType.Name}");
        skillObj.transform.parent = transform;
        Skill skill = (Skill)skillObj.AddComponent(skillType);

        // 스킬 데이터 초기화
        if (skill is Skill_Zoomies zoomiesSkill)
        {
            SkillData skillData;
            if (Managers.Instance.Data.SkillDic.TryGetValue(200000, out skillData))
            {
                zoomiesSkill.InitializeWithData(skillData);
            }
        }
        if (skill is Skill_BakeBread bakeBread)
        {
            SkillData skillData2;
            if (Managers.Instance.Data.SkillDic.TryGetValue(200001, out skillData2))
            {
                bakeBread.InitializeWithData(skillData2);
            }
        }
        if (skill is Skill_EatChur eatChur)
        {
            SkillData skillData5;
            if (Managers.Instance.Data.SkillDic.TryGetValue(200002, out skillData5))
            {
                eatChur.InitializeWithData(skillData5);
            }
        }
        if (skill is Skill_BeastEyes beastEyes)
        {
            SkillData skillData3;
            if (Managers.Instance.Data.SkillDic.TryGetValue(200003, out skillData3))
            {
                beastEyes.InitializeWithData(skillData3);
            }
        }
        if (skill is Skill_FatalStrike fatalStrike)
        {
            SkillData skillData4;
            if (Managers.Instance.Data.SkillDic.TryGetValue(200004, out skillData4))
            {
                fatalStrike.InitializeWithData(skillData4);
            }
        }

        if (!SkillList.Contains(skill))
        {
            SkillList.Add(skill);
        }
    }


    public void moveMiddlePos()
    {
        
    }
    
    private void Update()
    {
        AutoSkill();
        SetTarget();
    }

    private void FixedUpdate()
    {
        
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
        while (true)
        {
            if (Time.time >= skill._cooldownEndTime)
            {
                yield return StartCoroutine(skill.StartSkill());
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private Vector2 fireDir;
    private float EnemyDist;


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
            EnemyDist = (target.transform.position - transform.position).magnitude;
            fireDir = (target.transform.position - transform.position).normalized;
        }
        
    }

    IEnumerator ShootProjectile()
    {
        while (true)    
        {
            yield return new WaitForSeconds(1f / _currentStats.AttackSpeed);  // 1초 대기
            if (Managers.Instance.Game.MonsterList.Count > 0 && target != null && EnemyDist <= 5)
            {
                //EffectBase Effect = Managers.Instance.Object.Spawn<EffectBase>(transform.position, 0 ,"GunEffect");
                Managers.Instance.Object.ShowEffect(shootPos.transform.position, "GunEffect");
                Managers.Instance.Sound.Play("Fire", SoundManager.Sound.Effect);
                Projectile proj = Managers.Instance.Object.Spawn<Projectile>(CenterPosition, 0, "Projectile");
                bool isForcedCritical = _isCritical;
                if (_isCritical)
                {
                    _isCritical = false;  // 한 번 사용 후 리셋
                }

                proj.SetInfo(this, fireDir, isForcedCritical);  // SetInfo 메서드에 크리티컬 정보 전달
            }
        }
    }

    public void shootProjectileOnce()
    {
        if (Managers.Instance.Game.MonsterList.Count > 0 && target != null)
        {
            Projectile proj = Instantiate(projectile, transform.position, Quaternion.identity);
            proj.SetInfo(this, fireDir);
        }
    }

    private void CheckMonsterListAndControlShooting()
    {
        bool hasMonsters = Managers.Instance.Game.MonsterList.Count > 0;
        bool isAlive = Hp > 0;
        if (hasMonsters && isAlive && shootingCoroutine == null)
        {
            // 몬스터가 있고, 플레이어가 살아있고, 발사 중이 아니면 시작
            shootingCoroutine = StartCoroutine(ShootProjectile());
        }
        else if ((!hasMonsters || !isAlive) && shootingCoroutine != null)
        {
            // 몬스터가 없거나 플레이어가 죽었고, 발사 중이면 중지
            StopCoroutine(shootingCoroutine);
            shootingCoroutine = null;
        }
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
    public enum EPlayerLook
    {
        Right,
        Left,
        Down,
        Up
    }
    public EPlayerLook look { get; set; } = EPlayerLook.Right;
    void UpdateLookDirection()
    {
        if (Mathf.Abs(_input.x) > Mathf.Abs(_input.y))
        {
            look = _input.x > 0 ? EPlayerLook.Right : EPlayerLook.Left;
            SpriteRender.flipX = _input.x > 0;
        }
        else
        {
            look = _input.y > 0 ? EPlayerLook.Up : EPlayerLook.Down;
        }
    }

    void UpdateAni()
    {
        bool isMoving = _input.magnitude > 0.01f;
        anim.SetBool("IsWalk", isMoving);

        if (isMoving)
        {
            UpdateLookDirection();
        }
    }

}