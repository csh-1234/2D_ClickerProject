using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GoldEffect : BaseObject
{
    private static UI_GoldEffect instance;
    public static UI_GoldEffect Instance { get { return instance; } }

    [SerializeField] private GameObject goldCoinPrefab;
    [SerializeField] private RectTransform goldIconTarget;
    [SerializeField] private Transform effectParent;

    private Canvas parentCanvas;

    protected override void  Awake()
    {
        if (instance == null)
            instance = this;
        parentCanvas = GetComponentInParent<Canvas>();
    }

    public void PlayGoldEffect(Vector3 startWorldPosition, int goldAmount)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(startWorldPosition);
        int coinCount = Mathf.Min(goldAmount / 10, 5);

        for (int i = 0; i < coinCount; i++)
        {
            CreateCoinEffect(screenPos);
        }
    }

    private void CreateCoinEffect(Vector3 startPos)
    {
        GameObject coin = Instantiate(goldCoinPrefab, effectParent);
        coin.transform.position = startPos;

        Vector3 direction = (goldIconTarget.position - startPos).normalized;
        float distance = Vector3.Distance(startPos, goldIconTarget.position);
        Vector3 targetPos = startPos + direction * distance;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(coin.transform
            .DOMove(targetPos, 0.5f).SetDelay(0.5f)
            .SetEase(Ease.OutQuad));

        
        sequence.OnComplete(() => {
            Managers.Instance.Sound.Play("Coin", SoundManager.Sound.Effect);
            Destroy(coin);
            });
    }
}
