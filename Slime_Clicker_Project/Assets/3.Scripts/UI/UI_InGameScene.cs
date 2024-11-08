using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_InGameScene : RootUI
{
    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        LoadResource();
        //Managers.Instance.Sound.Play("InGameBgm", SoundManager.Sound.Bgm);
        
    }

    void OnApplicationQuit()
    {
        Managers.Instance.Game.SaveGame();
    }
    private void LoadResource() 
    {
        Managers.Instance.Resource.LoadAllResourceAsync<UnityEngine.Object>("Prefab", (key, count, totalCount) =>
        {
            Debug.Log($"{key}, {count} / {totalCount}]");
            if (count == totalCount)
            {
                Managers.Instance.Data.Initialize();
                Managers.Instance.Game.LoadGame();
                if(Managers.Instance.Currency.GetCurrentGold() == 0)
                {
                    Managers.Instance.Currency.SetGold(1000000000);
                }
                    Managers.Instance.StatUpgrade.UpdateAllTexts();
            }
        });
    }
}
