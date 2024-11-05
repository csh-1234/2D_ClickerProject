using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RootUI : MonoBehaviour
{
    private bool isInnitialized = false;
    protected virtual void Awake()
    {
        Initialize();
    }

    public virtual bool Initialize()
    {
        if (isInnitialized) { return false; }
        isInnitialized = true;

        Object obj = FindObjectOfType(typeof(EventSystem));
        if (obj == null)
        {
            GameObject go = new GameObject() { name = "EventSystem" };
            go.AddComponent<EventSystem>();
            go.AddComponent<StandaloneInputModule>();
        }
        return true;
    }

    public void PopupOpenAnimation(GameObject contentObject) // 팝업 오픈 연출
    {
        // 처음에는 더 작게 시작
        contentObject.transform.localScale = new Vector3(0.4f, 0.4f, 1);

        // 시퀀스를 사용하여 여러 단계의 애니메이션 구현
        Sequence sequence = DOTween.Sequence();

        // 첫 번째: 크게 늘어나기
        sequence.Append(contentObject.transform
            .DOScale(1.1f, 0.2f)
            .SetEase(Ease.OutQuad));

        // 두 번째: 살짝 줄어들기
        sequence.Append(contentObject.transform
            .DOScale(0.9f, 0.15f)
            .SetEase(Ease.InQuad));

        // 세 번째: 최종 크기로 돌아오기
        sequence.Append(contentObject.transform
            .DOScale(1f, 0.15f)
            .SetEase(Ease.OutBack, 3f));

        // 시간 스케일 영향 받지 않도록 설정
        sequence.SetUpdate(true);
    }
}
