using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
//using static DataManager;
using static Unity.Burst.Intrinsics.X86;
using Random = UnityEngine.Random;

public class ObjectManager
{
    public void ShowDamageFont(Vector2 pos, float damage, float healAmount, Transform parent, bool isCritical = false)
    {
        string prefabName;
        if (isCritical)
        {
            prefabName = "CriticalDamageText";
        }
        else
        {
            prefabName = "DamageText";
        }
        GameObject go = Managers.Instance.Resource.Instantiate(prefabName, pooling: true);
        ShowDamage damageText = go.GetOrAddComponent<ShowDamage>();
        damageText.SetInfo(pos, damage, healAmount, parent, isCritical);
    }
}
