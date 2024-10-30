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
}
