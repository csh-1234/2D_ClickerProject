using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UI_StatUpgrade : RootUI
{
    private TextMeshProUGUI StatLevel;
    private TextMeshProUGUI StatBonus;
    private TextMeshProUGUI StatCost;
    private string statType;
    protected override void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        StatLevel = gameObject.transform.Find("IconBoard/LevelText").GetComponent<TextMeshProUGUI>();
        StatBonus = gameObject.transform.Find("MenuTexts/BonusText").GetComponent<TextMeshProUGUI>();
        StatCost = gameObject.transform.Find("MenuButton/Board/CurrencyIcon/CostText").GetComponent<TextMeshProUGUI>();
        statType = gameObject.name;

        Managers.Instance.StatUpgrade.OnStatChanged += OnStatChanged;
        var initialStats = Managers.Instance.StatUpgrade.GetStatInfo(statType);

        UpdateStat(initialStats.level, initialStats.bonus, initialStats.cost);
        BindEventToObjects();
    }



    private void OnStatChanged(string type, int level, float bonus, int cost)
    {
        // 자신의 스탯 타입에 대한 변경사항만 처리
        if (type == statType)
        {
            //비용체크
            if(Managers.Instance.Currency.GetCurrentGold() >= cost)
            {
                UpdateStat(level, bonus, cost);
                Managers.Instance.Currency.RemoveGold(cost-1);
            }   
            else
            {
                Debug.Log("스텟 레벨업 자액부족");
            }
        }
    }


    private void UpdateStat(int level, float bonus, int cost)
    {
        StatLevel.text = $"LV.{level}";
        StatBonus.text = $"{statType.Replace("Upgrade", "")} +{bonus.ToString("F2")}";
        StatCost.text = $"{cost}";
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
        BindEvent("MenuButton", OnUpgradeButtonClick);
    }

    private void OnUpgradeButtonClick()
    {
        Managers.Instance.Sound.Play("Click", SoundManager.Sound.Effect);
        Managers.Instance.StatUpgrade.statUpgrade(statType);
    }

    //이벤트 정리
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

}
