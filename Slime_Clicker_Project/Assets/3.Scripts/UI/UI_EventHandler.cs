using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Action OnClickHandler = null;        // ���콺 Ŭ��
    public Action OnPressedHandler = null;      // ���콺 �� ����
    public Action OnPointerDownHandler = null;  // ���콺 �� ���� ����
    public Action OnPointerUpHandler = null;    // ���콺 ���� ����

    public Action<BaseEventData> OnDragHandler = null;      // ���콺 �巡��
    public Action<BaseEventData> OnBeginDragHandler = null; // ���콺 �巡�� ��
    public Action<BaseEventData> OnEndDragHandler = null;   // ���콺 �巡�� ����

    bool isPressed = false;

    private void Update()
    {
        if (isPressed)
        {
            OnPressedHandler?.Invoke();
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("��Ŭ��");
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("��Ŭ��");
        }
        else if (eventData.button == PointerEventData.InputButton.Middle)
        {
            Debug.Log("�߰� ��ư Ŭ��");
        }

        if (OnClickHandler != null)
        {
            OnClickHandler.Invoke();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        OnPointerDownHandler?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = true;
        OnPointerUpHandler?.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        isPressed = true;
        OnDragHandler?.Invoke(eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        OnBeginDragHandler?.Invoke(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnEndDragHandler?.Invoke(eventData);
    }
}