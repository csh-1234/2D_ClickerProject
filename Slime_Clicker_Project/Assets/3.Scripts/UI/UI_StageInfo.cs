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

  
    public Button startWave;

    private const int STAGES_PER_CYCLE = 5; // 한 사이클당 스테이지 수
    private Vector3 playerStartPos; // 플레이어 시작 위치 저장

 

    private int min = 2;
    private float sec = 0f;

    protected override void Awake()
    {
        base.Awake();
        SetCurrentStageLevel();
        // 페이드 패널 초기화
      
        playerStartPos = Managers.Instance.Game.player.transform.position;
    }

    private void Start()
    {
        if (coStartStage != null)
        {
            StopCoroutine(coStartStage);
            coStartStage = null;
        }
        coStartStage = StartCoroutine(StartWave());
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

            Managers.Instance.Game.player.Hp = Managers.Instance.Game.player.MaxHp;
            Managers.Instance.Game.UpdatePlayerStats();

            print("새로운 스테이지");
            // 현재 스테이지가 5의 배수일 때만 원점으로 리셋
            int currentStage = Managers.Instance.Stage.GetCurrentStageLevel();
            bool isEndOfCycle = currentStage % STAGES_PER_CYCLE == 0;

            yield return StartCoroutine(PlayPlayerMoveAnimation(isEndOfCycle));
            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator PlayPlayerMoveAnimation(bool resetPosition)
    {
        var player = Managers.Instance.Game.player;
        Vector3 originalPos = player.transform.position;
        Vector3 targetPos = originalPos + Vector3.right * 6f;

        // 걷기 애니메이션 시작
        player.anim.SetBool("IsWalk", true);

        // 이동 애니메이션
        yield return player.transform
            .DOMove(targetPos, 2.5f)
            .SetEase(Ease.Linear)
            .WaitForCompletion();

        player.anim.SetBool("IsWalk", false);

        if (resetPosition)
        {
            // 5스테이지 완료 후 페이드 아웃
            yield return StartCoroutine(UI_Fade.Instance.FadeOutCoroutine(0.5f));

            // 플레이어 위치 초기화
            player.transform.position = playerStartPos;

            yield return new WaitForSeconds(0.5f);

            // 페이드 인
            yield return StartCoroutine(UI_Fade.Instance.FadeInCoroutine(0.5f));
        }
        else
        {
            // 일반 스테이지 클리어 후 대기
            yield return new WaitForSeconds(1f);
        }
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


