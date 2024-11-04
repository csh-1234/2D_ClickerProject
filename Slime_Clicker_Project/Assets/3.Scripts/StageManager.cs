using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class StageManager
{
    private int CurrentStageLevel { get; set; } = 1;  // �������� ����
    private float StageTime { get; set; } = 120f;      // �� ���������� �ð�(��)
    private float DifficultyByLevel      //����� ���̵�(���� ���ݿ� �ջ�)
    {
        get 
        {
            //1 = 1.0, 2 = 1.0*1.1, 3 = 1.1 *1.1.......
            float baseMultiplier = 1.0f;
            float exponentialGrowth = Mathf.Pow(1.1f, CurrentStageLevel - 1);
            return baseMultiplier * exponentialGrowth;
        }
    }

    public int GetCurrentStageLevel()
    {
        return CurrentStageLevel;
    }

    public void AddCurrentStageLevel()
    {
        CurrentStageLevel++;
    }

    public void StartStage()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 spawnPos = new Vector2(Random.Range(3f, 20f), Random.Range(0.7f, 1.5f));
            Monster mo = Managers.Instance.Object.Spawn<Monster>(spawnPos, (int)EDataId.Slime_Green);
            //���������� ���� ���� ���� ���(�Ҽ��� ����)
            mo._currentStats.Hp = (int)(mo._currentStats.Hp * DifficultyByLevel);
            mo._currentStats.MaxHp = (int)(mo._currentStats.Hp * DifficultyByLevel);
            mo._currentStats.Attack = (int)(mo._currentStats.Hp * DifficultyByLevel);
            mo._currentStats.Defense = (int)(mo._currentStats.Hp * DifficultyByLevel);
            mo._currentStats.AttackSpeed = (int)(mo._currentStats.Hp * DifficultyByLevel);
            Managers.Instance.Game.MonsterList.Add(mo);
        }
    }
}
