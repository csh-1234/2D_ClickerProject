using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_StageInfo : RootUI
{
    [SerializeField]private TextMeshProUGUI StageInfo;
    [SerializeField]private TextMeshProUGUI StageTime;
    public Button startWave;

    private int min = 2;
    private float sec = 0f;

    protected override void Awake()
    {
        base.Awake();
        SetCurrentStageLevel();
    }

    private void Update()
    {
        //calctime();
    }

    void SetCurrentStageLevel()
    {
        StageInfo.text = $"Stage { Managers.Instance.Stage.GetCurrentStageLevel()}";
    }

    Coroutine coStartStage;

    public void OnClickStartStage()
    {
        if (coStartStage != null)
        {
            StopCoroutine(coStartStage);
            coStartStage = null;
        }
        coStartStage = StartCoroutine(StartWave());
    }

    IEnumerator StartWave()
    {
        while (true)
        {
            Managers.Instance.Stage.StartStage();
            min = 2;
            sec = 0f;
            while (true)
            {
                calctime();
                yield return null;

                if (sec <= 0f && min <= 0)  // �ε��Ҽ��� �� ����
                {
                    print("Ÿ�Ӿƿ�");
                    SetCurrentStageLevel();
                    break;
                }
                else if (Managers.Instance.Game.player.Hp == 0)
                {
                    print("�÷��̾� ���");
                    SetCurrentStageLevel();
                    break;
                }
                else if (Managers.Instance.Game.MonsterList.Count == 0)
                {
                    print("�������� Ŭ���� - ���� �����Ϸ�");
                    Managers.Instance.Stage.AddCurrentStageLevel();
                    SetCurrentStageLevel();
                    break;
                }
            }
            print("���ο� ��������");
            yield return new WaitForSeconds(1f);

            //x�� 6.86��ŭ �̵�
            //Managers.Instance.Game.player.moveMiddlePos();

            StartCoroutine(PlayPlayerMoveAnimation());
            yield return new WaitForSeconds(7f);
        }
    }

    private IEnumerator PlayPlayerMoveAnimation()
    {
        var player = Managers.Instance.Game.player;
        Vector3 originalPos = player.transform.position;
        Vector3 targetPos = originalPos + Vector3.right * 6.86f;

        
        // ������ �̵�
        yield return player.transform
            .DOMove(targetPos, 3f)
            .SetEase(Ease.Linear)
            .WaitForCompletion();

        yield return new WaitForSeconds(1f);

        // ����ġ�� �̵�
        yield return player.transform
            .DOMove(originalPos, 3f)
            .SetEase(Ease.InOutQuad)
            .WaitForCompletion();
    }

    //Coroutine coStartNextStage;
    //Coroutine coStartRepeatStage;

    //public void NextStage()
    //{
    //    if (coStartNextStage != null)
    //    {
    //        StopCoroutine(coStartNextStage);
    //        coStartNextStage = null;
    //    }
    //    coStartNextStage = StartCoroutine(StartNextStage());
    //}

    //public void RepeatStage()
    //{
    //    if (coStartRepeatStage != null)
    //    {
    //        StopCoroutine(coStartRepeatStage);
    //        coStartRepeatStage = null;
    //    }
    //    coStartRepeatStage = StartCoroutine(StartRepeatStage());
    //}


    //private IEnumerator StartNextStage()
    //{
    //    yield return null;
    //}

    //private IEnumerator StartRepeatStage()
    //{
    //    yield return null;
    //}



    void calctime()
    {
        sec -= Time.deltaTime;
        if (sec <= 0f)
        {
            sec = 60f;
            min--;
        }
        StageTime.text = string.Format("{0:D2}:{1:D2}", min, (int)sec);
    }

}


