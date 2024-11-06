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
            //UI_Root안에 짬뽕해서 넣을거임
            GameObject root = GameObject.Find("UI_Root");
            if (root == null)
                root = new GameObject { name = "UI_Root" };
            return root;
        }
    }

   public void CanvasInitialize(GameObject go, bool sort = true, int sortOrder = 0)
    {
        // UI를 쓰기 위해선 바탕이 되는 canvas, canvasScaler, graphicRaycast 컴포넌트가 필요하다.
        // 따라서 UI오브젝트에 해당 컴포넌트들이 없으면 추가해주는 초기화 작업이 필요하다.
        Canvas canvas = go.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
        //캔버스 사이즈를 설정하는 canvasScaler 컴포넌트 추가
        CanvasScaler canvasScaler = go.GetOrAddComponent<CanvasScaler>();
        if (canvasScaler != null)
        {
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1080, 1920);
        }
        //UI 클릭/터치 이벤트를 감지하는 GraphicRaycater 컴포넌트 추가
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
