using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

public class UI_TopBar : RootUI
{
    [SerializeField] protected TextMeshProUGUI CurrentGold;

    [SerializeField] protected Button GameSpeed;
    [SerializeField] protected Button GamePause;
    [SerializeField] protected Image NormalIcon;
    [SerializeField] protected Image FastIcon;
    private Image speedBoard;
    private bool isFast = false;


    protected override void Awake()
    {
        base.Awake();
        speedBoard = GameSpeed.GetComponent<Image>();
    }
    private void Start()    
    {
        Managers.Instance.Currency.OnGoldChanged += UpdateGoldText;
        CurrentGold.text = $"{CurrentGold.text = $"{string.Format("{0:N0}", Managers.Instance.Currency.GetCurrentGold())}"}";
    }
    private void UpdateGoldText(int goldAmount) { CurrentGold.text = $"{string.Format("{0:N0}", goldAmount)}"; }

    public void OnSpeedClick()
    {
        Managers.Instance.Sound.Play("Click", SoundManager.Sound.Effect);
        if (isFast == false)
        {
            Time.timeScale = 2f;
            NormalIcon.gameObject.SetActive(false);
            FastIcon.gameObject.SetActive(true);
            StartRainbowEffect();
            isFast = true;
        }
        else
        {
            Time.timeScale = 1f;
            NormalIcon.gameObject.SetActive(true);
            FastIcon.gameObject.SetActive(false);
            StopRainbowEffect();
            isFast = false;
        }
    }

    public void OnPauseClick()
    {
        Managers.Instance.Sound.Play("Click", SoundManager.Sound.Effect);
        Managers.Instance.UI.ShowPopupUI<UI_PausePopup>();
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
            speedBoard.color = HexToColor("FFEA7C");  // 원래 색상으로 복귀
        }
    }

    private IEnumerator SmoothRainbowColorChange()
    {
        print("레인보우 발동");
        float hue = 0f;
        Color currentColor = speedBoard.color;

        while (true)
        {
            Color targetColor = Color.HSVToRGB(hue, 0.7f, 1f); // 채도와 명도 값 조정
            speedBoard.color = targetColor;
            hue = (hue + Time.deltaTime * 0.5f) % 1f; // 속도 조정
            yield return null;
        }
    }

}
