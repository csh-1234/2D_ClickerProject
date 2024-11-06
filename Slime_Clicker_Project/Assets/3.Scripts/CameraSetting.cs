using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraSetting : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float targetAspect = 1920f / 1080f;  // ��ǥ ��Ⱦ��
    [SerializeField] private float defaultOrthographicSize = 5.4f; // �⺻ ī�޶� ������

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        UpdateCameraSize();
        AddLetterboxing();
    }

    private void UpdateCameraSize()
    {
        float currentAspect = (float)Screen.width / Screen.height;

        if (currentAspect > targetAspect)
        {
            // ȭ���� �� ���η� �� ���
            float scale = currentAspect / targetAspect;
            mainCamera.orthographicSize = defaultOrthographicSize * scale;
        }
    }

    private void AddLetterboxing()
    {
        float currentAspect = (float)Screen.width / Screen.height;

        // ���͹ڽ��� ������ �� ����
        if (currentAspect > targetAspect)
        {
            float normalizedWidth = targetAspect / currentAspect;
            float barWidth = (1f - normalizedWidth) / 2f;

            // ���� ��
            CreateLetterboxBar(new Vector2(barWidth, 1f), Vector2.left);
            // ������ ��
            CreateLetterboxBar(new Vector2(barWidth, 1f), Vector2.right);
        }
    }

    private void CreateLetterboxBar(Vector2 size, Vector2 position)
    {
        GameObject bar = new GameObject("LetterboxBar");
        bar.transform.SetParent(transform);

        RectTransform rectTransform = bar.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(position.x == -1 ? 0 : 1 - size.x, 0);
        rectTransform.anchorMax = new Vector2(position.x == -1 ? size.x : 1, 1);
        rectTransform.sizeDelta = Vector2.zero;

        Image image = bar.AddComponent<Image>();
        image.color = Color.black;
        image.raycastTarget = false;
    }
}
