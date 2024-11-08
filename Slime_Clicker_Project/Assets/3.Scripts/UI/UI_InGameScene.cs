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
        Managers.Instance.Game.LoadGame();
        LoadResource();
        Managers.Instance.Sound.Play("InGameBgm", SoundManager.Sound.Bgm);
        Managers.Instance.StatUpgrade.UpdateAllTexts();
    }

    void OnApplicationQuit()
    {
        Managers.Instance.Game.SaveGame();
    }
    private void LoadResource()
    {
        Managers.Instance.Resource.LoadAllResourceAsync<UnityEngine.Object>("Prefab", (key, count, totalCount) =>
        {
            Debug.Log($"리소스 받아오는중 ... [이름 : {key}, {count} / {totalCount}]");
            if (count == totalCount)
            {
                Managers.Instance.Data.Initialize();
            }
        });
    }
}
