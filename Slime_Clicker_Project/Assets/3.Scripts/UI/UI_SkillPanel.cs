using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SkillPanel : RootUI
{
    [SerializeField] private List<UI_SkillButton> skillButtons;  // Inspector에서 할당
    public GameObject template;
    public Image rotationIcon;

    protected override void Awake()
    {
        base.Awake();
        
    }
    private void Start()
    {
        InitializeSkillButtons();
        BindEventToObjects();
    }

    private void InitializeSkillButtons()
    {
        Player player = Managers.Instance.Game.player;
        foreach (Skill pSkill in player.SkillList)
        {
            GameObject SkillButton = Instantiate(template, transform);
            //TODO : resource에서 가져오도록(비동기 처리)
            //SkillButton.transform.Find("Icon").GetComponent<Image>().sprite = Managers.Instance.Resource.Load<Sprite>("EatChur");
            //SkillButton.transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>($"Resources/{pSkill.name}.icon");
            SkillButton.transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>($"{pSkill.name}_Icon");

            UI_SkillButton ui_SkillButton = SkillButton.GetComponent<UI_SkillButton>();
            ui_SkillButton.name = pSkill.name;
            ui_SkillButton.SetSkill(pSkill);

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
    private void BindEventToObjects()
    {
        // 이벤트를 추가할 (오브젝트 이름, 메서드)
        BindEvent("AutoSkill", OnAutoButtonClick);
    }

    private void OnAutoButtonClick()
    {
        var player = Managers.Instance.Game.player;
        player.IsAuto = !player.IsAuto;

        // 기존 트윈 정리
        if (rotationTween != null)
        {
            rotationTween.Kill();
            rotationTween = null;
        }

        // 회전 초기화
        rotationIcon.transform.rotation = Quaternion.identity;

        if (player.IsAuto)
        {
            StartLoadingRotation();
        }
    }


    //이벤트 정리, 일단 가지고 있도록함
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
    private Tweener rotationTween;  // Sequence 대신 Tweener 사용
    void StartLoadingRotation()
    {
        // 새로운 회전 트윈 생성
        rotationTween = rotationIcon.transform
            .DORotate(new Vector3(0, 0, -360), 2f, RotateMode.FastBeyond360)
            .SetRelative(true)
            .SetEase(Ease.Linear)
            .SetLoops(-1);
    }
}
