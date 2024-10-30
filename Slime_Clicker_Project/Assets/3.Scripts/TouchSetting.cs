using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchSetting : MonoBehaviour
{
    public GameObject prefab;
    public Transform parentTransform;
    float spawnTime;
    public float defaultTime = 0.05f;
    private bool isPressed = false;
    private float releaseTime = 0f;
    public float cooldownTime = 0.5f; // 쿨다운 시간 (초)


    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isPressed && Time.time - releaseTime >= cooldownTime)
        {
            isPressed = true;
            startcreate();
        }

        if (Input.GetMouseButtonUp(0))
        {
            isPressed = false;
            releaseTime = Time.time;
        }

        spawnTime += Time.deltaTime;
    }

    void startcreate()
    {
        Vector2 mposition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GameObject go = Instantiate(prefab, Input.mousePosition, Quaternion.identity, parentTransform);
        Destroy(go, 2f);
    }
}
