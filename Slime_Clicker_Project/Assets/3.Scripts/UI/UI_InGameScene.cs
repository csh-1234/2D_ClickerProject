using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_InGameScene : RootUI
{
    public GameObject StatUpgrade;
    public GameObject SkillUpgrade;
    public GameObject WeaponUpgrade;
    public GameObject ArmorUpgrade;

    public Toggle TStatUpgrade;
    public Toggle TSkillUpgrade;
    public Toggle TWeaponUpgrade;
    public Toggle TArmorUpgrade;


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
        TStatUpgrade.isOn = true;
        OnStatUpgradeClick();
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
        BindEvent("MenuToggle/StatUpgrade", OnStatUpgradeClick);
        BindEvent("MenuToggle/SkillUpgrade", OnSkillUpgradeClick);
        BindEvent("MenuToggle/WeaponUpgrade", OnWeaponUpgradeClick);
        BindEvent("MenuToggle/ArmorUpgrade", OnArmorUpgradeClick);
    }
    //StatUpgrade
    //SkillUpgrade
    //WeaponUpgrade
    //ArmorUpgrade

    private void OnStatUpgradeClick()
    {
        TStatUpgrade.isOn = true;
        if (TStatUpgrade != null)
        {
            if(TStatUpgrade.isOn)
            {
                StatUpgrade.gameObject.SetActive(true);
                SkillUpgrade.gameObject.SetActive(false);
                WeaponUpgrade.gameObject.SetActive(false);
                ArmorUpgrade.gameObject.SetActive(false);
            }
        }
    }

    private void OnSkillUpgradeClick()
    {
        TSkillUpgrade.isOn = true;
        if (TSkillUpgrade != null)
        {
            if (TSkillUpgrade.isOn)
            {
                StatUpgrade.gameObject.SetActive(false);
                SkillUpgrade.gameObject.SetActive(true);
                WeaponUpgrade.gameObject.SetActive(false);
                ArmorUpgrade.gameObject.SetActive(false);
            }
        }
    }

    private void OnWeaponUpgradeClick()
    {
        TWeaponUpgrade.isOn = true;
        if (TWeaponUpgrade != null)
        {
            if (TWeaponUpgrade.isOn)
            {
                StatUpgrade.gameObject.SetActive(false);
                SkillUpgrade.gameObject.SetActive(false);
                WeaponUpgrade.gameObject.SetActive(true);
                ArmorUpgrade.gameObject.SetActive(false);
            }
        }
    }

    private void OnArmorUpgradeClick()
    {
        TArmorUpgrade.isOn = true;
        if (TArmorUpgrade != null)
        {
            if (TArmorUpgrade.isOn)
            {
                StatUpgrade.gameObject.SetActive(false);
                SkillUpgrade.gameObject.SetActive(false);
                WeaponUpgrade.gameObject.SetActive(false);
                ArmorUpgrade.gameObject.SetActive(true);
            }
        }
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
