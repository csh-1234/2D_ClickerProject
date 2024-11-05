using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using static Enums;

public class MonsterSpawner : MonoBehaviour
{
    public Monster monster;

    private void Start()
    {
        //StartCoroutine(spawnMonster()); //3 20 2.8 1.1
    }

    //IEnumerator spawnMonster()
    //{
    //    while (true)
    //    {
    //        for (int i = 0; i < 5   ; i++)
    //        {
    //            Vector2 spawnPos = new Vector2(Random.Range(3f, 20f), Random.Range(0.7f, 1.5f));
    //            //Monster mo = Instantiate(monster, spawnPos, Quaternion.identity);
    //            //Monster mo = Managers.Instance.Object.Spawn<Monster>(spawnPos, (int)EDataId.Slime_Green);
    //            Managers.Instance.Game.MonsterList.Add(mo);
    //        }
    //        yield return new WaitForSeconds(5f);
    //    }
    //}

    void Update()
    {
        
    }
}
