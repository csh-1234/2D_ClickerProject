using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_InGameScene : RootUI
{
    protected override void Awake()
    {
        base.Awake();
    }
    // Start is called before the first frame update
    void Start()
    {
        LoadResource();
        BindEventToObjects();
        //�⺻ ���� = ���� ���׷��̵�
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void LoadResource()
    {
        Managers.Instance.Resource.LoadAllResourceAsync<UnityEngine.Object>("Prefab", (key, count, totalCount) =>
        {
            Debug.Log($"���ҽ� �޾ƿ����� ... [�̸� : {key}, {count} / {totalCount}]");
            if (count == totalCount)
            {
                Managers.Instance.Data.Initialize();
            }
        });

    }

    #region ObjectEvent
    private List<UI_EventHandler> _boundHandlers = new List<UI_EventHandler>();
    private void BindEvent(string objectName, Action action)
    {
        Transform objectTransform = transform.Find(objectName);
        if (objectTransform != null)
        {
            UI_EventHandler eventHandler = objectTransform.GetOrAddComponent<UI_EventHandler>();
            eventHandler.OnClickHandler += action;
            _boundHandlers.Add(eventHandler);
        }
        else
        {
            Debug.LogWarning($"[{objectName}] Object NotFound");
        }
    }
    private void BindEventToObjects()
    {
        // �̺�Ʈ�� �߰��� (������Ʈ �̸�, �޼���)
        //BindEvent("MenuToggle/StatUpgrade", OnStatUpgradeClick);
       
    }

    //�̺�Ʈ ����
    private void OnDisable()
    {
        foreach (var handler in _boundHandlers)
        {
            if (handler != null)
            {
                handler.OnClickHandler = null;
                handler.OnPressedHandler = null;
                handler.OnPointerDownHandler = null;
                handler.OnPointerUpHandler = null;
                handler.OnDragHandler = null;
                handler.OnBeginDragHandler = null;
                handler.OnEndDragHandler = null;
            }
        }
        _boundHandlers.Clear();
    }
    #endregion
}
