using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DropGold : BaseObject
{
    Vector2 targetpos = new Vector2(-2.395f, 4.649f);

    public override bool Initialize()
    {
        base.Initialize();
        ObjectType = Enums.ObjectType.Gold;

        return true;
    }

    private void Update()
    {
        //Vector2 dist = ((Vector2)transform.position - targetpos).normalized;
        //transform.Translate(dist * Time.deltaTime * 2f);
    }
}
