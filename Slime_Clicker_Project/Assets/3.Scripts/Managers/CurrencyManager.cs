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
        //TODO : ����� ��ȭ ������ load
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
        //�ϴ� ��尡 ���̳ʽ��� �Ǵ°� ���Ƴ���. ��ȭ�� ���Ž� üũ�ϴ� ���� �ʿ�
        CurrentGold =  Mathf.Max(0, CurrentGold - addAmount);
        OnGoldChanged?.Invoke(CurrentGold);
    }
    public int GetCurrentGold() { return CurrentGold; }
}
