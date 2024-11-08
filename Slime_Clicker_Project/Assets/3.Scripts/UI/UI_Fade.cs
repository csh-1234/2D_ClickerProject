using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UI_Fade : MonoBehaviour
{
    private static UI_Fade instance;
    public static UI_Fade Instance { get { return instance; } }

    [SerializeField] private Image fadeImage;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        if (fadeImage != null)
            fadeImage.color = new Color(0, 0, 0, 0);
    }

    public IEnumerator FadeOutCoroutine(float duration)
    {
        fadeImage.gameObject.SetActive(true);
        yield return fadeImage.DOFade(1f, duration).WaitForCompletion();
    }

    public IEnumerator FadeInCoroutine(float duration)
    {
        yield return fadeImage.DOFade(0f, duration).WaitForCompletion();
        fadeImage.gameObject.SetActive(false);
    }
}
