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
    [SerializeField] private Transform shootPos;

    public Animator anim;

    public Projectile projectile;
    private Monster target;
    private Coroutine shootingCoroutine;
    public List<Skill> SkillList = new List<Skill>();

    private Dictionary<Skill, Coroutine> autoSkillCoroutines = new Dictionary<Skill, Coroutine>();
    private Dictionary<string, BuffInfo> _activeBuffs = new Dictionary<string, BuffInfo>();

    private Vector2 fireDir;
    private float EnemyDist;
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
    public bool _isAuto = false;     // ���� ��ų ����
    public bool _isCritical = false; // [��ų]�ϰ��ʻ� Ʈ���� => �������� Ȯ��ġ��

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

    private void Update()
    {
        AutoSkill();
        SetTarget();
        CheckMonsterListAndControlShooting();
    }

    public override void SetInfo(int dataId)
    {
        base.SetInfo(dataId);
        ObjectType = ObjectType.Player;
        _currentStats.CopyStats(_baseStats);
    }

    #region Skill&Buff
    public override void ApplyStatModifier(Stats modifier, bool isOn)
    {
        if (modifier == null) return;

        // ���� ����/���� �� UpdatePlayerStats�� ȣ������ ����
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
        // ���� ������ �ִٸ� ����
        if (_activeBuffs.ContainsKey(buffId))
        {
            RemoveBuff(buffId);
        }

        // �� ���� �߰�
        _activeBuffs[buffId] = new BuffInfo
        {
            BuffId = buffId,
            BuffStats = buffStats,
            EndTime = Time.time + duration
        };

        // ���ȿ� ���� ����
        ApplyStatModifier(buffStats, true);

        // ��ü ���� ������Ʈ
        Managers.Instance.Game.UpdatePlayerStats();
    }

    public void RemoveBuff(string buffId)
    {
        if (_activeBuffs.TryGetValue(buffId, out BuffInfo buff))
        {
            ApplyStatModifier(buff.BuffStats, false);
            _activeBuffs.Remove(buffId);

            // ��ü ���� ������Ʈ
            Managers.Instance.Game.UpdatePlayerStats();
        }
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

        // ��ų ������ �ʱ�ȭ
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

    public IEnumerable<BuffInfo> GetActiveBuffs()
    {
        return _activeBuffs.Values;
    }
    #endregion

    #region AutoSkill
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
                // StartCoroutine�� ��ȯ��(Coroutine)�� ���� Dictionary�� ����
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
    #endregion

    #region Projectile
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
    private void CheckMonsterListAndControlShooting()
    {
        bool hasMonsters = Managers.Instance.Game.MonsterList.Count > 0;
        bool isAlive = Hp > 0;
        if (hasMonsters && isAlive && shootingCoroutine == null)
        {
            // ���Ͱ� �ְ�, �÷��̾ ����ְ�, �߻� ���� �ƴϸ� ����
            shootingCoroutine = StartCoroutine(ShootProjectile());
        }
        else if ((!hasMonsters || !isAlive) && shootingCoroutine != null)
        {
            // ���Ͱ� ���ų� �÷��̾ �׾���, �߻� ���̸� ����
            StopCoroutine(shootingCoroutine);
            shootingCoroutine = null;
        }
    }
    IEnumerator ShootProjectile()
    {
        while (true)    
        {
            yield return new WaitForSeconds(1f / _currentStats.AttackSpeed);  // 1�� ���
            if (Managers.Instance.Game.MonsterList.Count > 0 && target != null && EnemyDist <= 5)
            {
                //EffectBase Effect = Managers.Instance.Object.Spawn<EffectBase>(transform.position, 0 ,"GunEffect");
                Managers.Instance.Object.ShowEffect(shootPos.transform.position, "GunEffect");
                Managers.Instance.Sound.Play("Fire", SoundManager.Sound.Effect);
                Projectile proj = Managers.Instance.Object.Spawn<Projectile>(CenterPosition, 0, "Projectile");
                bool isForcedCritical = _isCritical;
                if (_isCritical)
                {
                    _isCritical = false;  // �� �� ��� �� ����
                }

                proj.SetInfo(this, fireDir, isForcedCritical);  // SetInfo �޼��忡 ũ��Ƽ�� ���� ����
            }
        }
    }
    #endregion

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