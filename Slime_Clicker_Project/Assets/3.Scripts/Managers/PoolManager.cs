using System.Collections.Generic;
using UnityEngine.Pool;
using UnityEngine;
using UnityEditor.SceneManagement;
class Pool
{
    private GameObject _prefab;  // 풀링할 원본 프리팹
    private IObjectPool<GameObject> _pool;  // Unity의 내장 오브젝트 풀
    private Transform _root;     // 풀링된 오브젝트들의 부모 Transform

    // Root 프로퍼티: 풀링된 오브젝트들을 담을 부모 오브젝트
    private Transform Root
    {
        get
        {
            if (_root == null)
            {
                // 루트 오브젝트가 없으면 새로 생성
                GameObject go = new GameObject() { name = $"{_prefab.name}Root" };
                _root = go.transform;

                // UIParticle인 경우 특별 처리
                if (_prefab.name == "UIParticle")
                {
                    SetRootPosition(go);
                }
            }
            return _root;
        }
    }

    // 생성자: 프리팹을 받아 새로운 풀 생성
    public Pool(GameObject prefab)
    {
        _prefab = prefab;
        // ObjectPool 생성 시 필요한 콜백 함수들 등록
        _pool = new ObjectPool<GameObject>(OnCreate, OnGet, OnRelease, OnDestroy);
    }

    // UI 파티클용 특별 처리: 캔버스의 자식으로 설정
    private void SetRootPosition(GameObject rootObject)
    {
        Canvas canvas = GameObject.Find("ClickEffectCanvas").GetComponent<Canvas>();
        if (canvas != null)
        {
            rootObject.transform.SetParent(canvas.transform, false);
        }
    }

    // 풀에서 오브젝트 가져오기
    public GameObject Pop()
    {
        return _pool.Get();
    }

    // 풀에 오브젝트 반환
    public void Push(GameObject go)
    {
        _pool.Release(go);
    }

    #region 풀 콜백 함수들
    // 새 오브젝트 생성 시 호출
    private GameObject OnCreate()
    {
        GameObject go = GameObject.Instantiate(_prefab);
        go.transform.SetParent(Root, false);
        go.name = _prefab.name;
        return go;
    }

    // 풀에서 오브젝트를 가져올 때 호출
    void OnGet(GameObject go)
    {
        go.SetActive(true);
    }
    
    // 풀에 오브젝트를 반환할 때 호출
    void OnRelease(GameObject go)
    {
        go.SetActive(false);
    }
    
    // 풀에서 오브젝트를 완전히 제거할 때 호출
    void OnDestroy(GameObject go)
    {
        GameObject.Destroy(go);
    }
    #endregion
}

// 전체 풀을 관리하는 매니저 클래스
public class PoolManager
{
    // 프리팹 이름을 키로 사용하는 풀 딕셔너리
    private Dictionary<string, Pool> _pools = new Dictionary<string, Pool>();

    public GameObject Pop(GameObject prefab)
    {
        //해당 프리팹의 풀이 없으면 생성
        if (_pools.ContainsKey(prefab.name) == false)
        {
            createPool(prefab);
        }
        return _pools[prefab.name].Pop();
    }

    public bool Push(GameObject go)
    {
        //해당 프리펩 이름을 가진 풀이 없으면 false
        if (_pools.ContainsKey(go.name) == false)
        {
            return false;
        }
        _pools[go.name].Push(go);

        return true;
    }

    void createPool(GameObject prefab)
    {
        //새로운 풀 생성
        Pool pool = new Pool(prefab);
        _pools.Add(prefab.name, pool);
    }
}