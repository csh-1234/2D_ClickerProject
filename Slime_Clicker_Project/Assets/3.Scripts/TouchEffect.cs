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
    public float cooldownTime = 0.5f; // ��ٿ� �ð� (��)

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

        // ���� �ð� �� ������Ʈ Ǯ�� ��ȯ
        StartCoroutine(DestroyAfterDelay(go));
    }

    IEnumerator DestroyAfterDelay(GameObject go)
    {
        yield return new WaitForSeconds(1f);
        Managers.Instance.Resource.Destroy(go);
    }
}