using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EffectBase : MonoBehaviour
{
    private ParticleSystem _particleSystem;

    public void SetInfo(Vector2 pos)
    {
        _particleSystem = GetComponent<ParticleSystem>();
        transform.position = pos;
        StartCoroutine(coDespawnEffect());
    }

    private IEnumerator coDespawnEffect()
    {
        yield return new WaitForSeconds(2f);
        Managers.Instance.Resource.Destroy(gameObject);

    }

}
