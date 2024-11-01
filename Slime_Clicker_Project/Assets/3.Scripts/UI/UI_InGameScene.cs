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
        //기본 상태 = 스텟 업그레이드
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void LoadResource()
    {
        Managers.Instance.Resource.LoadAllResourceAsync<UnityEngine.Object>("Prefab", (key, count, totalCount) =>
        {
            Debug.Log($"리소스 받아오는중 ... [이름 : {key}, {count} / {totalCount}]");
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
        // 이벤트를 추가할 (오브젝트 이름, 메서드)
        //BindEvent("MenuToggle/StatUpgrade", OnStatUpgradeClick);
       
    }

    //이벤트 정리
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
