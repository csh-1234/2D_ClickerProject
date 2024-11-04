using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_StageInfo : RootUI
{
    [SerializeField]private TextMeshProUGUI StageInfo;
    [SerializeField]private TextMeshProUGUI StageTime;

    [SerializeField] private Transform background1;  // 첫 번째 배경
    [SerializeField] private Transform background2;  // 두 번째 배경
    [SerializeField] private float backgroundWidth = 17.8f;  // 배경 하나의 너비

    public Button startWave;

    private Vector3 bg1InitialPos;
    private Vector3 bg2InitialPos;

    private int min = 2;
    private float sec = 0f;

    protected override void Awake()
    {
        base.Awake();
        SetCurrentStageLevel();
        backgroundWidth = background1.GetComponent<SpriteRenderer>().bounds.size.x;
        bg1InitialPos = background1.position;
        bg2InitialPos = background2.position;
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

                if (sec <= 0f && min <= 0)  // 부동소수점 비교 수정
                {
                    print("타임아웃");
                    SetCurrentStageLevel();
                    break;
                }
                else if (Managers.Instance.Game.player.Hp == 0)
                {
                    print("플레이어 사망");
                    SetCurrentStageLevel();
                    break;
                }
                else if (Managers.Instance.Game.MonsterList.Count == 0)
                {
                    print("스테이지 클리어 - 몬스터 소탕완료");
                    Managers.Instance.Stage.AddCurrentStageLevel();
                    SetCurrentStageLevel();
                    break;
                }
            }
            print("새로운 스테이지");
            yield return new WaitForSeconds(1f);

            //x로 6.86만큼 이동
            //Managers.Instance.Game.player.moveMiddlePos();

            StartCoroutine(PlayPlayerMoveAnimation());
            StartCoroutine(PlayBackgroundAnimation());
            yield return new WaitForSeconds(7f);
        }
    }

    private IEnumerator PlayPlayerMoveAnimation()
    {
        var player = Managers.Instance.Game.player;
        Vector3 originalPos = player.transform.position;
        Vector3 targetPos = originalPos + Vector3.right * 2f;


        // 앞으로 이동
        yield return player.transform
            .DOMove(targetPos, 3f)
            .SetEase(Ease.InOutQuad)
            .WaitForCompletion();

        yield return new WaitForSeconds(1f);

        // 원위치로 이동
        yield return player.transform
            .DOMove(originalPos, 3f)
            .SetEase(Ease.InOutQuad)
            .WaitForCompletion();
    }

    private IEnumerator PlayBackgroundAnimation()
    {
        Vector3 originalPos = background1.transform.position;
        Vector3 targetPos = originalPos - (Vector3.right * 2f);


        // 로 이동
        yield return background1.transform
            .DOMove(targetPos, 5f)
            .SetEase(Ease.InOutQuad)
            .WaitForCompletion();

        yield return new WaitForSeconds(1f);

        //// 원위치로 이동
        //yield return background1.transform
        //    .DOMove(originalPos, 3f)
        //    .SetEase(Ease.InOutQuad)
        //    .WaitForCompletion();
    }

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


