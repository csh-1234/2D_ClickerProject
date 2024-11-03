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
using static Enums;
public class UI_SkillPanel : RootUI
{
    [SerializeField] private List<UI_SkillButton> skillButtons;  // Inspector���� �Ҵ�
    public GameObject template;
    public Image rotationIcon;
    public Image IconBoard;

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
            //TODO : resource���� ����������(�񵿱� ó��)
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
        // �̺�Ʈ�� �߰��� (������Ʈ �̸�, �޼���)
        BindEvent("AutoSkill", OnAutoButtonClick);
    }

    private void OnAutoButtonClick()
    {
        var player = Managers.Instance.Game.player;
        player.IsAuto = !player.IsAuto;

        // ���� Ʈ�� ����
        if (rotationTween != null)
        {
            rotationTween.Kill();
            rotationTween = null;
        }

        // ȸ�� �ʱ�ȭ
        rotationIcon.transform.rotation = Quaternion.identity;

        if (player.IsAuto)
        {
            StartLoadingRotation();
            StartRainbowEffect();
        }
        else
        {
            StopRainbowEffect();
        }
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
    private Tweener rotationTween;  // Sequence ��� Tweener ���
    private Tweener rainbowTween;  // Sequence ��� Tweener ���
    void StartLoadingRotation()
    {
        // ���ο� ȸ�� Ʈ�� ����
        rotationTween = rotationIcon.transform
            .DORotate(new Vector3(0, 0, 360), 2f, RotateMode.FastBeyond360)
            .SetRelative(true)
            .SetEase(Ease.Linear)
            .SetLoops(-1);
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
}
