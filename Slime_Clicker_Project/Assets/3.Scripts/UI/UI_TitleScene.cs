using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Enums;

public class UI_TitleScene : RootUI
{
    public Image TitleTextBox;
    public TextMeshProUGUI TitleText;
    public Slider loadingSlider;
    public TextMeshProUGUI loadingValue;
    public TextMeshProUGUI loadingText;
    public Image LoadingCircle;
    public Image startButton;

    
    private DG.Tweening.Sequence _titleAnimation;
    protected override void Awake()
    {
        base.Awake();
        LoadResource();
    }

    void Start()
    {
        BindEventToObjects();
        StartCoroutine(StartLoadingBar());
        Managers.Instance.Sound.Play("TitleBgm", SoundManager.Sound.Bgm);
        
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
        BindEvent("StartButton", OnTitleClickButtonClick);
        
    }

    private void OnTitleClickButtonClick()
    {
        Managers.Instance.Sound.Play("Click", SoundManager.Sound.Effect);
        SceneManager.LoadScene("InGameScene");
        Managers.Instance.Sound.Clear();
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
        startButton.gameObject.SetActive(true);
        DOTween.Init();
        _titleAnimation = DOTween.Sequence();

        _titleAnimation.Join(TitleText.DOFade(0, 1.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutCubic));
        _titleAnimation.Join(TitleTextBox.DOFade(0, 1.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutCubic));
    }

    #endregion

    IEnumerator StartLoadingBar()
    {
        loadingSlider.GetComponent<Slider>().DOValue(1, 5);
        LoadingCircle.GetComponent<RectTransform>().DORotate(new Vector3(0, 0, -360), 5f, RotateMode.FastBeyond360).SetEase(Ease.Linear);

        float timeElapsed = 0f;
        while (timeElapsed < 5f)
        {
            timeElapsed += Time.deltaTime;
            float percent = Mathf.Clamp01(timeElapsed / 5f) * 100f;
            loadingValue.SetText(percent.ToString("F0") + "%");
            yield return null;
        }

        // 10초 후에 로딩 이미지 비활성화
        Managers.Instance.Sound.Play("Clear", SoundManager.Sound.Effect);
        LoadingCircle.gameObject.SetActive(false);
        loadingText.gameObject.SetActive(false);
        TitleTextBox.gameObject.SetActive(true);
        loadingSlider.gameObject.SetActive(false);


        StartButtonAnimation();
    }
}
