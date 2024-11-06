using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Tilemaps.TilemapRenderer;
using UnityEngine.UI;
using DG.Tweening;
public class UI_Manager
{
    int _order = 10;

    Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
    UI_Scene _sceneUI = null;
    public UI_Scene SceneUI { get { return _sceneUI; } }
    public GameObject Root
    {
        get
        {
            //UI_Root�ȿ� «���ؼ� ��������
            GameObject root = GameObject.Find("UI_Root");
            if (root == null)
                root = new GameObject { name = "UI_Root" };
            return root;
        }
    }

   public void CanvasInitialize(GameObject go, bool sort = true, int sortOrder = 0)
    {
        // UI�� ���� ���ؼ� ������ �Ǵ� canvas, canvasScaler, graphicRaycast ������Ʈ�� �ʿ��ϴ�.
        // ���� UI������Ʈ�� �ش� ������Ʈ���� ������ �߰����ִ� �ʱ�ȭ �۾��� �ʿ��ϴ�.
        Canvas canvas = go.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
        //ĵ���� ����� �����ϴ� canvasScaler ������Ʈ �߰�
        CanvasScaler canvasScaler = go.GetOrAddComponent<CanvasScaler>();
        if (canvasScaler != null)
        {
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1080, 1920);
        }
        //UI Ŭ��/��ġ �̺�Ʈ�� �����ϴ� GraphicRaycater ������Ʈ �߰�
        go.GetOrAddComponent<GraphicRaycaster>();

        if (sort)
        {
            canvas.sortingOrder = _order;
            _order++;
        }
        else
        {
            canvas.sortingOrder = sortOrder;
        }
    }

    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Instance.Resource.Instantiate($"{name}");
        T sceneUI = go.GetOrAddComponent<T>();
        _sceneUI = sceneUI;

        go.transform.SetParent(Root.transform);

        return sceneUI;
    }

    public T ShowPopupUI<T>(string name = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Instance.Resource.Instantiate($"{name}");
        T popup = go.GetOrAddComponent<T>();
        _popupStack.Push(popup);

        go.transform.SetParent(Root.transform);
        CanvasInitialize(go);

        return popup;
    }

    public void ClosePopupUI(UI_Popup popup)
    {
        if (_popupStack.Count == 0)
            return;

        if (_popupStack.Peek() != popup)
        {
            Debug.Log("Close Popup Failed!");
            return;
        }
        ClosePopupUI();
    }

    public void ClosePopupUI()
    {
        if (_popupStack.Count == 0)
            return;

        UI_Popup popup = _popupStack.Pop();
        Managers.Instance.Resource.Destroy(popup.gameObject);
        popup = null;
        _order--;
    }

    public void CloseALLPopupUI()
    {
        if (_popupStack.Count == 0)
            return;


        UI_Popup popup = _popupStack.Pop();
        Managers.Instance.Resource.Destroy(popup.gameObject);
        popup = null;
        _order--;
    }

}
