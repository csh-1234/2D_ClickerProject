using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_MenuToggle : RootUI
{
    public Toggle TStatUpgrade;
    public Toggle TSkillUpgrade;
    public Toggle TWeaponUpgrade;
    public Toggle TArmorUpgrade;

    public GameObject StatUpgrade;
    public GameObject SkillUpgrade;
    public GameObject WeaponUpgrade;
    public GameObject ArmorUpgrade;

    public ScrollRect Scroll;

    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        BindEventToObjects();
        //�⺻ ���� = ���� ���׷��̵�
        TStatUpgrade.isOn = true;
        //OnStatUpgradeClick();
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
        BindEvent("StatUpgrade", OnStatUpgradeClick);
        BindEvent("SkillUpgrade", OnSkillUpgradeClick);
        BindEvent("WeaponUpgrade", OnWeaponUpgradeClick);
        BindEvent("ArmorUpgrade", OnArmorUpgradeClick);
    }

    private void OnStatUpgradeClick()
    {
        Managers.Instance.Sound.Play("Click", SoundManager.Sound.Effect);
        TStatUpgrade.isOn = true;
        if (TStatUpgrade != null)
        {
            if (TStatUpgrade.isOn)
            {
                OnToggleChanged();
                StatUpgrade.gameObject.SetActive(true);
                SkillUpgrade.gameObject.SetActive(false);
                WeaponUpgrade.gameObject.SetActive(false);
                ArmorUpgrade.gameObject.SetActive(false);
                
            }
        }
    }

    private void OnSkillUpgradeClick()
    {
        Managers.Instance.Sound.Play("Click", SoundManager.Sound.Effect);
        TSkillUpgrade.isOn = true;
        if (TSkillUpgrade != null)
        {
            if (TSkillUpgrade.isOn)
            {
                OnToggleChanged();
                StatUpgrade.gameObject.SetActive(false);
                SkillUpgrade.gameObject.SetActive(true);
                WeaponUpgrade.gameObject.SetActive(false);
                ArmorUpgrade.gameObject.SetActive(false);
            }
        }
    }

    private void OnWeaponUpgradeClick()
    {
        Managers.Instance.Sound.Play("Click", SoundManager.Sound.Effect);
        TWeaponUpgrade.isOn = true;
        if (TWeaponUpgrade != null)
        {
            if (TWeaponUpgrade.isOn)
            {
                OnToggleChanged();
                StatUpgrade.gameObject.SetActive(false);
                SkillUpgrade.gameObject.SetActive(false);
                WeaponUpgrade.gameObject.SetActive(true);
                ArmorUpgrade.gameObject.SetActive(false);
            }
        }
    }

    private void OnArmorUpgradeClick()
    {
        Managers.Instance.Sound.Play("Click", SoundManager.Sound.Effect);
        TArmorUpgrade.isOn = true;
        if (TArmorUpgrade != null)
        {
            if (TArmorUpgrade.isOn)
            {
                OnToggleChanged();
                StatUpgrade.gameObject.SetActive(false);
                SkillUpgrade.gameObject.SetActive(false);
                WeaponUpgrade.gameObject.SetActive(false);
                ArmorUpgrade.gameObject.SetActive(true);
            }
        }
    }

    private IEnumerator ScrollToTop()
    {
        yield return null;  // ���� �����ӱ��� ���
        Scroll.normalizedPosition = new Vector2(0, 1);
    }

    public void OnToggleChanged()
    {
        // ���� ������ ��� ���� ����...

        StartCoroutine(ScrollToTop());
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
