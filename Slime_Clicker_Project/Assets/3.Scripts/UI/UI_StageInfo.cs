using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Enums;

public class UI_StageInfo : RootUI
{
    [SerializeField] private TextMeshProUGUI StageInfo;
    [SerializeField] private TextMeshProUGUI StageTime;


    [SerializeField] private Image StageClearAlarm;
    [SerializeField] private Image StageFailAlarm;

    public Animator CanAnim;
    public static bool isCutSceneOn;

    private const int STAGES_PER_CYCLE = 5; // 한 사이클당 스테이지 수
    private readonly Vector3 playerStartPos = new Vector2(0, 8.58f); // 플레이어 시작 위치 저장
    private int min = 1;
    private float sec = 0f;

    private Coroutine coStartStage;
    private int stageCounter = 1;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        SetCurrentStageLevel();
        if (coStartStage != null)
        {
            StopCoroutine(coStartStage);
            coStartStage = null;
        }
        coStartStage = StartCoroutine(StartWave());
    }

    private void Update()
    {
    }

    private IEnumerator StartWave()
    {
        
        while (true)
        {
            Managers.Instance.Stage.StartStage();
            StageTimeInit();

            while (true)
            {
                calctime();
                yield return null;

                if (min < 0 || Managers.Instance.Game.player.Hp <= 0)
                {
                    print("스테이지 패배");
                    yield return StartCoroutine(DefeatAnimation());
                    Managers.Instance.Sound.Play("Fail", SoundManager.Sound.Effect);
                    StartCoroutine(ShowStageAlarm(StageFailAlarm));
                    SetCurrentStageLevel();
                    stageCounter++;
                    break;
                }
                else if (Managers.Instance.Game.MonsterList.Count == 0)
                {
                    print("스테이지 클리어 - 몬스터 소탕완료");
                    Managers.Instance.Stage.AddCurrentStageLevel();
                    Managers.Instance.Sound.Play("Clear", SoundManager.Sound.Effect);
                    StartCoroutine(ShowStageAlarm(StageClearAlarm));
                    SetCurrentStageLevel();
                    stageCounter++;
                    break;
                }
            }

            RestorePlayerHp();
            print("새로운 스테이지");

            // 현재 스테이지가 5의 배수일 때 마다
            int currentStage = Managers.Instance.Stage.GetCurrentStageLevel();
            bool isEndOfCycle = stageCounter % STAGES_PER_CYCLE == 0;
            yield return StartCoroutine(ClearAnimation(isEndOfCycle));
            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator DefeatAnimation()
    {
        var player = Managers.Instance.Game.player;

        player.anim.SetBool("IsWalk", false);
        CanAnim.SetBool("cutscene1", true);
        Invoke(nameof(StopCutScene), 3f);

        // 모든 몬스터들을 뒤로 물러나게 함
        foreach (var monster in Managers.Instance.Game.MonsterList)
        {
            monster.RetreatFromPlayer(2f); // 카메라 줌인 시간과 동일하게 설정
        }
        // 잠시 대기
        yield return new WaitForSeconds(2f);
        Managers.Instance.Stage.ClearCurrentStage();
        yield return new WaitForSeconds(1.5f);
    }
   
    private IEnumerator ClearAnimation(bool resetPosition)
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
            Debug.Log($"플레이어 위치 :{playerStartPos.x}, {playerStartPos.y}");

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

    private IEnumerator ShowStageAlarm(Image image)
    {
        image.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        image.gameObject.SetActive(false);
    }

    public void RestorePlayerHp()
    {
        Managers.Instance.Game.player.Hp = Managers.Instance.Game.player.MaxHp;
        Managers.Instance.Game.UpdatePlayerStats();
    }
    
    public void StopCutScene()
    {
        isCutSceneOn = false;
        CanAnim.SetBool("cutscene1", false);
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

    private void StageTimeInit()
    {
        min = 1;
        sec = 0f;
    }

    private void SetCurrentStageLevel()
    {
        StageInfo.text = $"Stage {Managers.Instance.Stage.GetCurrentStageLevel()}";
    }

}


