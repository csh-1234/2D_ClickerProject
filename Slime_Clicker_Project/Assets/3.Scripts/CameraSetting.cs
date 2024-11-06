using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraSetting : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float targetAspect = 1920f / 1080f;  // 목표 종횡비
    [SerializeField] private float defaultOrthographicSize = 5.4f; // 기본 카메라 사이즈

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
            // 화면이 더 가로로 긴 경우
            float scale = currentAspect / targetAspect;
            mainCamera.orthographicSize = defaultOrthographicSize * scale;
        }
    }

    private void AddLetterboxing()
    {
        float currentAspect = (float)Screen.width / Screen.height;

        // 레터박스용 검은색 바 생성
        if (currentAspect > targetAspect)
        {
            float normalizedWidth = targetAspect / currentAspect;
            float barWidth = (1f - normalizedWidth) / 2f;

            // 왼쪽 바
            CreateLetterboxBar(new Vector2(barWidth, 1f), Vector2.left);
            // 오른쪽 바
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
