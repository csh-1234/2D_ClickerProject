using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PausePopup : UI_Popup
{
    [SerializeField] private Button continueButton;
    [SerializeField] private Button exitButton;
    private float checkTimeScale;

    protected override void Awake()
    {
        base.Awake();
        checkTimeScale = Time.timeScale;
        Time.timeScale = 0;
    }
    public void OnContinueButtonClick()
    {
        Time.timeScale = checkTimeScale;
        ClosePopupUI();
    }
    public void OnExitButtonClick()
    {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
            Application.Quit(); // 어플리케이션 종료
    #endif
    }
}

