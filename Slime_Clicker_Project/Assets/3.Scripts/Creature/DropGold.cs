using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DropGold : BaseObject
{
    public override bool Initialize()
    {
        base.Initialize();
        ObjectType = Enums.ObjectType.Gold;

        return true;
    }
}
