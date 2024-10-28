using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RootUI : RootInit
{
    public override bool Initialize()
    {
        if(!base.Initialize())
        {
            return true;
        }

        //EventSystem »ý¼º
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
