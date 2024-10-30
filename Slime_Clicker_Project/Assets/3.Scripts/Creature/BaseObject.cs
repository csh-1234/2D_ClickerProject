using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class BaseObject : MonoBehaviour
{
    public EObjectType ObjectType { get; protected set; }

    private bool isInnitialized = false;

    protected virtual void Awake()
    {
        Initialize();
    }

    public virtual bool Initialize()
    {
        if (isInnitialized) { return false; }
        isInnitialized = true;
        return true;
    }
}
