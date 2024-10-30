using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UI_SkillUpgrade : RootUI
{
    private TextMeshProUGUI SkillLevel;
    private TextMeshProUGUI SkillCost;

    private string statType;
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        SkillLevel = gameObject.transform.Find("IconBoard/LevelText").GetComponent<TextMeshProUGUI>();
        SkillCost = gameObject.transform.Find("MenuButton/Board/CurrencyIcon/CostText").GetComponent<TextMeshProUGUI>();
        statType = gameObject.name;

        Managers.Instance.StatUpgrade.OnStatChanged += OnStatChanged;
        var initialStats = Managers.Instance.StatUpgrade.GetStatInfo(statType);

        UpdateStat(initialStats.level, initialStats.bonus, initialStats.cost);
        BindEventToObjects();
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
        BindEvent("MenuButton", OnUpgradeButtonClick);
    }

    private void OnUpgradeButtonClick()
    {
        Managers.Instance.StatUpgrade.statUpgrade(statType);
    }

    //�̺�Ʈ ����, �ϴ� ������ �ֵ�����
    //private void OnDisable()
    //{
    //    foreach (var handler in _boundHandlers)
    //    {
    //        if (handler != null)
    //        {
    //            handler.OnClickHandler = null;
    //            handler.OnPressedHandler = null;
    //            handler.OnPointerDownHandler = null;
    //            handler.OnPointerUpHandler = null;
    //            handler.OnDragHandler = null;
    //            handler.OnBeginDragHandler = null;
    //            handler.OnEndDragHandler = null;
    //        }
    //    }
    //    //if (Managers.Instance != null && Managers.Instance.StatUpgrade != null)
    //    //{
    //    Managers.Instance.StatUpgrade.OnStatChanged -= OnStatChanged;
    //    //}
    //    _boundHandlers.Clear();
    //}
    #endregion

    private void OnStatChanged(string type, int level, int bonus, int cost)
    {
        // �ڽ��� ���� Ÿ�Կ� ���� ������׸� ó��
        if (type == statType)
        {
            UpdateStat(level, bonus, cost);
        }
    }

    private void UpdateStat(int level, int bonus, int cost)
    {
        SkillLevel.text = $"LV.{level}";
        SkillCost.text = $"{cost}";
    }

}
