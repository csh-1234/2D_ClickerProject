using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Scene : RootUI
{
    public override bool Initialize()
    {
        if (base.Initialize() == false)
            return false;

        Managers.Instance.UI.CanvasInitialize(gameObject, false);
        return true;
    }
}
