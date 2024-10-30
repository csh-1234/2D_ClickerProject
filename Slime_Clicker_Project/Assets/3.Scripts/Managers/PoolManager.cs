using System.Collections.Generic;
using UnityEngine.Pool;
using UnityEngine;
using UnityEditor.SceneManagement;
class Pool
{
    private GameObject _prefab;  // Ǯ���� ���� ������
    private IObjectPool<GameObject> _pool;  // Unity�� ���� ������Ʈ Ǯ
    private Transform _root;     // Ǯ���� ������Ʈ���� �θ� Transform

    // Root ������Ƽ: Ǯ���� ������Ʈ���� ���� �θ� ������Ʈ
    private Transform Root
    {
        get
        {
            if (_root == null)
            {
                // ��Ʈ ������Ʈ�� ������ ���� ����
                GameObject go = new GameObject() { name = $"{_prefab.name}Root" };
                _root = go.transform;

                // UIParticle�� ��� Ư�� ó��
                if (_prefab.name == "UIParticle")
                {
                    SetRootPosition(go);
                }
            }
            return _root;
        }
    }

    // ������: �������� �޾� ���ο� Ǯ ����
    public Pool(GameObject prefab)
    {
        _prefab = prefab;
        // ObjectPool ���� �� �ʿ��� �ݹ� �Լ��� ���
        _pool = new ObjectPool<GameObject>(OnCreate, OnGet, OnRelease, OnDestroy);
    }

    // UI ��ƼŬ�� Ư�� ó��: ĵ������ �ڽ����� ����
    private void SetRootPosition(GameObject rootObject)
    {
        Canvas canvas = GameObject.Find("ClickEffectCanvas").GetComponent<Canvas>();
        if (canvas != null)
        {
            rootObject.transform.SetParent(canvas.transform, false);
        }
    }

    // Ǯ���� ������Ʈ ��������
    public GameObject Pop()
    {
        return _pool.Get();
    }

    // Ǯ�� ������Ʈ ��ȯ
    public void Push(GameObject go)
    {
        _pool.Release(go);
    }

    #region Ǯ �ݹ� �Լ���
    // �� ������Ʈ ���� �� ȣ��
    private GameObject OnCreate()
    {
        GameObject go = GameObject.Instantiate(_prefab);
        go.transform.SetParent(Root, false);
        go.name = _prefab.name;
        return go;
    }

    // Ǯ���� ������Ʈ�� ������ �� ȣ��
    void OnGet(GameObject go)
    {
        go.SetActive(true);
    }
    
    // Ǯ�� ������Ʈ�� ��ȯ�� �� ȣ��
    void OnRelease(GameObject go)
    {
        go.SetActive(false);
    }
    
    // Ǯ���� ������Ʈ�� ������ ������ �� ȣ��
    void OnDestroy(GameObject go)
    {
        GameObject.Destroy(go);
    }
    #endregion
}

// ��ü Ǯ�� �����ϴ� �Ŵ��� Ŭ����
public class PoolManager
{
    // ������ �̸��� Ű�� ����ϴ� Ǯ ��ųʸ�
    private Dictionary<string, Pool> _pools = new Dictionary<string, Pool>();

    public GameObject Pop(GameObject prefab)
    {
        //�ش� �������� Ǯ�� ������ ����
        if (_pools.ContainsKey(prefab.name) == false)
        {
            createPool(prefab);
        }
        return _pools[prefab.name].Pop();
    }

    public bool Push(GameObject go)
    {
        //�ش� ������ �̸��� ���� Ǯ�� ������ false
        if (_pools.ContainsKey(go.name) == false)
        {
            return false;
        }
        _pools[go.name].Push(go);

        return true;
    }

    void createPool(GameObject prefab)
    {
        //���ο� Ǯ ����
        Pool pool = new Pool(prefab);
        _pools.Add(prefab.name, pool);
    }
}