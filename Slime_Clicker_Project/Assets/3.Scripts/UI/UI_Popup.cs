using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Popup : RootUI
{
    public override bool Initialize()
    {
        if (!base.Initialize())
        {
            return true;
        }

        //Managers.Instance.UI.CanvasInitialize(gameObject, true);
        return true;
    }

    public virtual void ClosePopupUI()
    {
        //Managers.Instance.UI.ClosePopupUI(this);
    }
}
