using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    public static bool IsInit = false;
    private static Managers instance;
    public static Managers Instance
    {
        get
        {
            InitializeManager();
            return instance;
        }
    }
    public static void InitializeManager()
    {
        if (instance == null && IsInit == false)
        {
            GameObject go = GameObject.Find("Managers");
            if (go == null)
            {
                go = new GameObject { name = "Managers" };
                go.AddComponent<Managers>();
            }
            DontDestroyOnLoad(go);
            instance = go.GetComponent<Managers>();
        }
    }

    UI_Manager ui = new UI_Manager();
    ResourceManager resource = new ResourceManager();
    PoolManager pool = new PoolManager();
    ObjectManager _object = new ObjectManager();
    GameManager game = new GameManager();
    CurrencyManager currency = new CurrencyManager();
    StatUpgradeManager statUpgrade = new StatUpgradeManager();

    public UI_Manager UI { get { return Instance != null ? Instance.ui : null; } }
    public ResourceManager Resource { get { return Instance != null ? Instance.resource : null; } }
    public PoolManager Pool { get { return Instance != null ? Instance.pool : null; } }
    public ObjectManager Object { get { return Instance != null ? Instance._object : null; } }
    public GameManager Game { get { return Instance != null ? instance.game : null; } }
    public CurrencyManager Currency { get { return Instance != null ? instance.currency : null; } }
    public StatUpgradeManager StatUpgrade { get { return Instance != null ? instance.statUpgrade : null; } }


    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (IsInit) return;

        // 하위 매니저 초기화
        IsInit = true;
    }

    private void OnDestroy()
    {
        Clear();
    }

    public void Clear()
    {
        IsInit = false;
        instance = null;
    }

}
