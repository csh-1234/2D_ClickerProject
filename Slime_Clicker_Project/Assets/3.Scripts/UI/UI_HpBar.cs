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
        //transform.position = Camera.main.WorldToScreenPoint(parent.position - Vector3.up * 1.2f);
        transform.rotation = Camera.main.transform.rotation;
        float ratio = creature.Hp / (float)creature.MaxHp;
        SetHpRatio(ratio);
    }
    public void SetHpRatio(float ratio)
    {
        GetComponent<Slider>().value = ratio;
    }
}
