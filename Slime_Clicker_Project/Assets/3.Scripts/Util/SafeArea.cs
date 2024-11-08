using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeArea : MonoBehaviour
{
    [SerializeField] private float defaultScreenWidth = 1920f;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        float currentWidth = mainCamera.orthographicSize * 2f * mainCamera.aspect;
        float defaultWidth = defaultScreenWidth / 100f; // 유니티 단위로 변환

        // 현재 화면 비율에 맞춰 위치 조정
        float ratio = currentWidth / defaultWidth;
        Vector3 position = transform.position;
        position.x *= ratio;
        transform.position = position;
    }
}
