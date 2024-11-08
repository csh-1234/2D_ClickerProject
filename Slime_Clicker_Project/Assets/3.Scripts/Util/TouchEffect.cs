using Coffee.UIExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TouchEffect : BaseObject
{
    public GameObject prefab;
    public Transform parentTransform;
    float spawnTime;
    private bool isPressed = false;
    private float releaseTime = 0f;
    public float cooldownTime = 0.5f; // 쿨다운 시간 (초)

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isPressed && Time.time - releaseTime >= cooldownTime)
        {
            isPressed = true;
            StartCreate();
        }

        if (Input.GetMouseButtonUp(0))
        {
            isPressed = false;
            releaseTime = Time.time;
        }

        spawnTime += Time.deltaTime;
    }
    void StartCreate()
    {
        GameObject go = Managers.Instance.Resource.Instantiate("UIParticle", parentTransform, true);
        go.transform.position = Input.mousePosition;

        // 일정 시간 후 오브젝트 풀로 반환
        StartCoroutine(DestroyAfterDelay(go));
    }

    IEnumerator DestroyAfterDelay(GameObject go)
    {
        yield return new WaitForSeconds(1f);
        Managers.Instance.Resource.Destroy(go);
    }
}