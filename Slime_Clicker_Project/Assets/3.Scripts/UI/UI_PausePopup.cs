using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PausePopup : UI_Popup   
{
    [SerializeField] private Button continueButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject popup;
    private float checkTimeScale;
    private void OnEnable()
    {
        PopupOpenAnimation(popup);
    }
    protected override void Awake()
    {
        base.Awake();
        checkTimeScale = Time.timeScale;
        Time.timeScale = 0;
    }
    public void OnContinueButtonClick()
    {
        Managers.Instance.Sound.Play("Click", SoundManager.Sound.Effect);
        Time.timeScale = checkTimeScale;
        ClosePopupUI();
    }
    public void OnExitButtonClick()
    {
        Managers.Instance.Sound.Play("Click", SoundManager.Sound.Effect);
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
            Application.Quit(); // 어플리케이션 종료
    #endif
    }
}

