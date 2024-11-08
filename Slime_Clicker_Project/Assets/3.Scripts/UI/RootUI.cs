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

    public void PopupOpenAnimation(GameObject contentObject) // �˾� ���� ����
    {
        // ó������ �� �۰� ����
        contentObject.transform.localScale = new Vector3(0.4f, 0.4f, 1);

        // �������� ����Ͽ� ���� �ܰ��� �ִϸ��̼� ����
        Sequence sequence = DOTween.Sequence();

        // ù ��°: ũ�� �þ��
        sequence.Append(contentObject.transform
            .DOScale(1.1f, 0.2f)
            .SetEase(Ease.OutQuad));

        // �� ��°: ��¦ �پ���
        sequence.Append(contentObject.transform
            .DOScale(0.9f, 0.15f)
            .SetEase(Ease.InQuad));

        // �� ��°: ���� ũ��� ���ƿ���
        sequence.Append(contentObject.transform
            .DOScale(1f, 0.15f)
            .SetEase(Ease.OutBack, 3f));

        // �ð� ������ ���� ���� �ʵ��� ����
        sequence.SetUpdate(true);
    }
}
