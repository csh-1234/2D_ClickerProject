using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_HpBar : RootUI
{
    public Creature creature;
    protected override void Awake()
    {
        base.Awake();
    }
    private void Update()
    {
        Transform parent = transform.parent;
        transform.rotation = Camera.main.transform.rotation;
        float ratio = creature._currentStats.Hp / (float)creature._currentStats.MaxHp;
        SetHpRatio(ratio);
    }
    public void SetHpRatio(float ratio)
    {
        GetComponent<Slider>().value = ratio;
    }
}
