using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootInit : MonoBehaviour
{
    protected bool isInit = false;
    public virtual bool Initialize()
    {
        if (isInit)
        {
            return false;
        }
        isInit = true;
        return true;
    }

    protected virtual void Awake()
    {
        Initialize();
    }


}
