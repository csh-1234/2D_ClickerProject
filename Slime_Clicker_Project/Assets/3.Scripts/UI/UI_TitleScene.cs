using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_TitleScene : RootUI
{
    public Image TitleTextBox;
    public TextMeshProUGUI TitleText;
    
    private DG.Tweening.Sequence _titleAnimation;
    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        StartButtonAnimation();
        BindEventToObjects();
    }

    void Update()
    {
        
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
        BindEvent("TitleImage", OnTitleClickButtonClick);
    }

    private void OnTitleClickButtonClick()
    {
        Debug.Log("Ddd");
        SceneManager.LoadScene("InGameScene");
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

        _titleAnimation?.Kill();
    }
    #endregion

    #region Animation
    void StartButtonAnimation()
    {
        DOTween.Init();
        _titleAnimation = DOTween.Sequence();

        _titleAnimation.Join(TitleText.DOFade(0, 2f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutCubic));
        _titleAnimation.Join(TitleTextBox.DOFade(0, 2f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutCubic));
    }

    #endregion

   
}
