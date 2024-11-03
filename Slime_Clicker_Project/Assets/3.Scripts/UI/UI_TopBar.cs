using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_TopBar : RootUI
{
    [SerializeField] protected TextMeshProUGUI CurrentGold;

    [SerializeField] protected Button GameSpeed;
    [SerializeField] protected Button GamePause;


    protected override void Awake()
    {
        base.Awake();
    }
    private void Start()    
    {
        Managers.Instance.Currency.OnGoldChanged += UpdateGoldText;
        CurrentGold.text = $"{CurrentGold.text = $"{string.Format("{0:N0}", Managers.Instance.Currency.GetCurrentGold())}"}";
    }

    private void UpdateGoldText(int goldAmount) { CurrentGold.text = $"{string.Format("{0:N0}", goldAmount)}"; }
}
