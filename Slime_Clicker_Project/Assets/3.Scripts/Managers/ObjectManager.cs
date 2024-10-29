using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
//using static DataManager;
using static Unity.Burst.Intrinsics.X86;
using Random = UnityEngine.Random;

public class ObjectManager
{
    //public Player _player { get; private set; }
    //public HashSet<Monster> Monsters { get; } = new HashSet<Monster>();
    //public HashSet<ProjectileController> Projectiles { get; } = new HashSet<ProjectileController>();
    //public HashSet<GameObject> ClickEffect { get; } = new HashSet<GameObject>();
    //public HashSet<Gem> Gems { get; } = new HashSet<Gem>();

    //Dictionary<int, CreatureData> CreatureDataDic;

    //private List<SpawnPointManager> spawnPoints = new List<SpawnPointManager>();

    //public void AddSpawnPoint(Vector2 position)
    //{
    //    spawnPoints.Add(new SpawnPointManager(position));
    //}

    //public Monster SpawnMonsterAtAvailablePoint(int dataId)
    //{
    //    foreach (var spawnPoint in spawnPoints)
    //    {
    //        if (!spawnPoint.IsOccupied)
    //        {
    //            Monster monster = Spawn<Monster>(spawnPoint.Position, dataId);
    //            if (monster != null)
    //            {
    //                spawnPoint.CurrentMonster = monster;
    //                monster.OnDead1 += () => spawnPoint.CurrentMonster = null;
    //                return monster;
    //            }
    //        }
    //    }
    //    return null; // ��� ������ ���� ����Ʈ�� ����
    //}

    //public List<Monster> SpawnMonstersAtAllAvailablePoints(int dataId)
    //{
    //    List<Monster> spawnedMonsters = new List<Monster>();
    //    foreach (var spawnPoint in spawnPoints)
    //    {
    //        if (!spawnPoint.IsOccupied)
    //        {
    //            Monster monster = Spawn<Monster>(spawnPoint.Position, dataId);
    //            if (monster != null)
    //            {
    //                spawnPoint.CurrentMonster = monster;
    //                monster.OnDead1 += () => spawnPoint.CurrentMonster = null;
    //                spawnedMonsters.Add(monster);
    //            }
    //        }
    //    }
    //    return spawnedMonsters;
    //}


    //public void ShowDamageFont(Vector2 pos, float damage, float healAmount, Transform parent, bool isCritical = false)
    //{
    //    string prefabName;
    //    if (isCritical)
    //    {
    //        prefabName = "CriticalDamageText";
    //    }
    //    else
    //    {
    //        prefabName = "DamageText";
    //    }
    //    pos.x += Random.Range(0, 1.1f);
    //    pos.y += Random.Range(0, 1.1f);
    //    GameObject go = Managers.Instance.Resource.Instantiate(prefabName, pooling: true);
    //    ShowDamage damageText = go.GetOrAddComponent<ShowDamage>();
    //    damageText.SetInfo(pos, damage, healAmount, parent, isCritical);
    //}

    // ��Ģ : ��� ������Ʈ�� ������ spawn���� �����ؾ� �Ѵ�.
    //todo ��� ��� dataid �߰�
    //public T Spawn<T>(Vector3 position, int DataId = 0, string prefabName = "") where T : BaseObject
    //{
    //    Type type = typeof(T);

    //    //�ѹ��� ������ ��Ʈ �����ؼ� �б�
    //    if (CreatureDataDic == null)
    //    {
    //        CreatureDataDic = Managers.Instance.Data.CreatureDic;
    //    }



    //    if (type == typeof(Player)) // Player ����
    //    {
    //        CreatureData playerData;
    //        if (CreatureDataDic.TryGetValue(DataId, out playerData))
    //        {
    //            GameObject go = Managers.Instance.Resource.Instantiate("Player");
    //            Player player = go.GetOrAddComponent<Player>();
    //            player.transform.position = position;
    //            player.SetInfo(DataId);
    //            //player.name = playerData.DataName;
    //            //player.Hp = playerData.Hp;
    //            //player.MaxHp = playerData.MaxHp;
    //            //player.Atk = playerData.Atk;
    //            //player.CriRate = 20; // TODO : ������ ��Ʈ�� ũ��Ȯ�� �߰�
    //            _player = player;
    //            player.Initialize();
    //            Managers.Instance.Game.player = player;
    //            return player as T;
    //        }
    //        else
    //        {
    //            Debug.LogError("�÷��̾� ������ ���� ���߽��ϴ�. Wrong DatId or Not Exist in DataTable");
    //            return null;
    //        }
    //    }

    //    else if (type == typeof(Monster)) // Monster ����
    //    {
    //        CreatureData mosnterData;
    //        if (CreatureDataDic.TryGetValue(DataId, out mosnterData))
    //        {
    //            // ���ʹ� ��� ���� Mosnter �������� ���. ���⼭ ���Ҵ��ؼ� ����Ѵ�!
    //            GameObject go = Managers.Instance.Resource.Instantiate("Monster", pooling: true);
    //            Monster monster = go.GetOrAddComponent<Monster>();
    //            monster.transform.position = position;
    //            monster.Hp = mosnterData.Hp;
    //            monster.MaxHp = mosnterData.MaxHp;
    //            monster.Atk = mosnterData.Atk;
    //            monster.MoveSpeed = mosnterData.MoveSpeed;
    //            monster.GetComponent<SpriteRenderer>().sprite = Managers.Instance.Resource.Load<Sprite>(mosnterData.SpriteName);
    //            monster.Initialize();
    //            Monsters.Add(monster);
    //            return monster as T;
    //        }
    //        else
    //        {
    //            Debug.LogError("���� ������ ���� ���߽��ϴ�. Wrong DatId or Not Exist in DataTable");
    //            return null;
    //        }
    //    }
    //    else if (type == typeof(Projectile))
    //    {
    //        GameObject go = Managers.Instance.Resource.Instantiate(prefabName, pooling: true);
    //        Projectile projectile = go.GetOrAddComponent<Projectile>();
    //        go.transform.position = position;
    //        Projectiles.Add(projectile);
    //        projectile.Initialize();
    //        return projectile as T;
    //    }
    //    else if (type == typeof(Gem))
    //    {
    //        GameObject go = Managers.Instance.Resource.Instantiate(prefabName, pooling: true);
    //        Gem gem = go.GetOrAddComponent<Gem>();
    //        go.transform.position = position;
    //        return gem as T;
    //    }
    //    else if (type == typeof(BaseObject))
    //    {
    //        GameObject go = Managers.Instance.Resource.Instantiate(prefabName, pooling: true);
    //        go.transform.position = position;
    //        return go as T;
    //    }

    //    Debug.LogError("Wrong DatId or Not Exist in DataTable");
    //    return null;
    //}

    //public void Despawn<T>(T obj) where T : BaseObject
    //{
    //    if (obj.IsValid() == false)
    //    {
    //    }

    //    System.Type type = typeof(T);

    //    if (type == typeof(Player))
    //    {
    //    }
    //    else if (type == typeof(Monster))
    //    {
    //        //Monster.Remove(obj as Monster);
    //        Managers.Instance.Resource.Destroy(obj.gameObject);
    //    }
    //    else if (type == typeof(Gem))
    //    {
    //        //Gem.Remove(obj as Gem);
    //        Managers.Instance.Resource.Destroy(obj.gameObject);

    //    }
    //    else if (type == typeof(ProjectileController) || type == typeof(Projectile))
    //    {
    //        //Projectiles.Remove(obj as ProjectileController);
    //        Managers.Instance.Resource.Destroy(obj.gameObject);
    //    }
    //}
}
