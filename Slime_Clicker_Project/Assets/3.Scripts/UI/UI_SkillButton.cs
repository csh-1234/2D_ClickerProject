using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Enums;

public class UI_SkillButton : RootUI, IPointerClickHandler
{
    public Image cooldownFill;
    public Image IconBoard;
    public Skill skill;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        if (skill != null)
        {
            skill.OnCooldownUpdate += UpdateCooldownUI;
            skill.OnBuffStart += StartRainbowEffect;
            skill.OnBuffEnd += StopRainbowEffect;
        }
        
        //BindEventToObjects();
    }

    private void OnDisable()
    {
        if (skill != null)
        {
            skill.OnCooldownUpdate -= UpdateCooldownUI;
            skill.OnBuffStart -= StartRainbowEffect;
            skill.OnBuffEnd -= StopRainbowEffect;
        }
    }

    private void UpdateCooldownUI(float ratio)
    {
        if (cooldownFill != null)
        {
            cooldownFill.fillAmount = 1f - ratio;
        }
    }

    // ��ų ���� ������ ���� public �޼���
    public void SetSkill(Skill newSkill)
    {
        if (skill != null)
        {
            skill.OnCooldownUpdate -= UpdateCooldownUI;
        }

        skill = newSkill;

        if (skill != null)
        {
            skill.OnCooldownUpdate += UpdateCooldownUI;
        }
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

    public void OnPointerClick(PointerEventData eventData)
    {
        StartCoroutine(skill.StartSkill());
        
    }

 
    private Coroutine rainbowEffect;

    public void StartRainbowEffect()
    {
        if (rainbowEffect != null)
            StopCoroutine(rainbowEffect);
            
        rainbowEffect = StartCoroutine(SmoothRainbowColorChange());
    }

    public void StopRainbowEffect()
    {
        if (rainbowEffect != null)
        {
            StopCoroutine(rainbowEffect);
            rainbowEffect = null;
            IconBoard.color = HexToColor("FFEA7C");  // ���� �������� ����
        }
    }
        
    private IEnumerator SmoothRainbowColorChange()
    {
        print("���κ��� �ߵ�");
        float hue = 0f;
        Color currentColor = IconBoard.color;

        while (true)
        {
            Color targetColor = Color.HSVToRGB(hue, 0.7f, 1f); // ä���� �� �� ����
            IconBoard.color = targetColor;
            hue = (hue + Time.deltaTime * 0.5f) % 1f; // �ӵ� ����
            yield return null;
        }
    }
    #endregion

    //private void BindEventToObjects()
    //{
    //    // �̺�Ʈ�� �߰��� (������Ʈ �̸�, �޼���)
    //    BindEvent("CooldownFill", OnSkillButtonClick);
    //}
    //private void OnSkillButtonClick()
    //{
    //    print("��ų��ưŬ��");
    //    skill.StartSkill();
    //}


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

}
