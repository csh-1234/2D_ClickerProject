using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyManager
{
    private int CurrentGold = 100000000;
    public event Action<int> OnGoldChanged;

    public void Initialize()
    {
        //TODO : 저장된 재화 데이터 load
        //CurrentGold = 100000;
    }
    public void SetGold(int addAmount)
    {
        CurrentGold = addAmount;
        OnGoldChanged?.Invoke(CurrentGold);
    }

    public void AddGold(int addAmount)
    {
        CurrentGold += addAmount;
        OnGoldChanged?.Invoke(CurrentGold);
    }
    public void RemoveGold(int addAmount)
    {
        //일단 골드가 마이너스가 되는걸 막아놓음. 강화나 구매시 체크하는 로직 필요
        CurrentGold =  Mathf.Max(0, CurrentGold - addAmount);
        OnGoldChanged?.Invoke(CurrentGold);
    }
    public int GetCurrentGold() { return CurrentGold; }
}
