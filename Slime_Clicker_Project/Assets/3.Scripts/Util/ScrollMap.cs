using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ScrollMap : MonoBehaviour
{
    [SerializeField] private float defaultFlowSpeed = 0.3f;
    private float currentFlowSpeed;

    private MeshRenderer meshRenderer;
    private Material backgroundMaterial;
    private float offset;

    public bool IsScrolling { get; set; }

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        // 새로운 머티리얼 인스턴스 생성
        backgroundMaterial = new Material(meshRenderer.material);
        meshRenderer.material = backgroundMaterial;
    }

    private void Update()
    {
        currentFlowSpeed = IsScrolling ? defaultFlowSpeed : 0f;
        offset += Time.deltaTime * currentFlowSpeed;
        backgroundMaterial.mainTextureOffset = new Vector2(offset, 0);
    }

    private void OnDestroy()
    {
        if (backgroundMaterial != null)
        {
            Destroy(backgroundMaterial);
        }
    }

    // 스크롤 속도를 직접 설정하는 메서드 추가
    public void SetScrollSpeed(float speed)
    {
        defaultFlowSpeed = speed;
    }
}