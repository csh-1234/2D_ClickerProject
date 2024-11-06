using Cinemachine;
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

    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin virtualCameraNoise;
    private float originalOrthoSize;
    private CinemachineFramingTransposer framingTransposer;
    public Button startWave;

    private const int STAGES_PER_CYCLE = 5; // 한 사이클당 스테이지 수
    private Vector3 playerStartPos; // 플레이어 시작 위치 저장

 

    private int min = 0;
    private float sec = 30f;

    protected override void Awake()
    {
        base.Awake();
        SetCurrentStageLevel();
        // 페이드 패널 초기화
      
        if(Managers.Instance.Game.player!=null)
        {
            playerStartPos = Managers.Instance.Game.player.transform.position;

        }

        // 카메라 컴포넌트 초기화
        if (virtualCamera != null)
        {
            virtualCameraNoise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            originalOrthoSize = virtualCamera.m_Lens.OrthographicSize;
        }
    }

    private void Start()
    {
        if (coStartStage != null)
        {
            StopCoroutine(coStartStage);
            coStartStage = null;
        }
        coStartStage = StartCoroutine(StartWave());
        SetCurrentStageLevel();
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
    private int stageCounter = 1;
    IEnumerator StartWave()
    {
        while (true)
        {
            Managers.Instance.Stage.StartStage();
            min = 0;
            sec = 30f;
            while (true)
            {
                calctime();
                yield return null;

                if (min < 0) 
                {
                    print("타임아웃");
                    StageTime.text = "00:30";
                    yield return StartCoroutine(PlayPlayerDeathAnimation());
                    SetCurrentStageLevel();
                    stageCounter++;
                    break;
                }
                else if (Managers.Instance.Game.player.Hp <= 0)
                {
                    print("플레이어 사망");
                    yield return StartCoroutine(PlayPlayerDeathAnimation());
                    stageCounter++;
                    SetCurrentStageLevel();
                    break;
                }
                else if (Managers.Instance.Game.MonsterList.Count == 0)
                {
                    print("스테이지 클리어 - 몬스터 소탕완료");
                    Managers.Instance.Stage.AddCurrentStageLevel();
                    SetCurrentStageLevel();
                    stageCounter++;
                    break;
                }
            }

            Managers.Instance.Game.player.Hp = Managers.Instance.Game.player.MaxHp;
            Managers.Instance.Game.UpdatePlayerStats();

            print("새로운 스테이지");
            // 현재 스테이지가 5의 배수일 때만 원점으로 리셋
            int currentStage = Managers.Instance.Stage.GetCurrentStageLevel();
            bool isEndOfCycle = stageCounter % STAGES_PER_CYCLE == 0;

            yield return StartCoroutine(PlayPlayerMoveAnimation(isEndOfCycle));
            yield return new WaitForSeconds(2f);
        }
    }


    private IEnumerator PlayPlayerDeathAnimation()
    {
        var player = Managers.Instance.Game.player;

        // 원래 카메라 설정 저장
        float originalOrthoSize = virtualCamera.m_Lens.OrthographicSize;
        Vector3 originalTrackedObjectOffset = framingTransposer.m_TrackedObjectOffset;
        float originalDeadZoneDepth = framingTransposer.m_DeadZoneDepth;
        float originalDeadZoneWidth = framingTransposer.m_DeadZoneWidth;
        float originalDeadZoneHeight = framingTransposer.m_DeadZoneHeight;

        // 플레이어 애니메이션 정지
        player.anim.SetBool("IsWalk", false);

        // 모든 몬스터들을 뒤로 물러나게 함
        foreach (var monster in Managers.Instance.Game.MonsterList)
        {
            monster.RetreatFromPlayer(1.5f); // 카메라 줌인 시간과 동일하게 설정
        }

        // 플레이어를 화면 중앙으로 배치하기 위한 설정
        framingTransposer.m_DeadZoneDepth = 0;
        framingTransposer.m_DeadZoneWidth = 0;
        framingTransposer.m_DeadZoneHeight = 0;
        framingTransposer.m_ScreenX = 0.5f;
        framingTransposer.m_ScreenY = 0.5f;
        
        // 천천히 줌인
        DOTween.To(() => virtualCamera.m_Lens.OrthographicSize,
            value => virtualCamera.m_Lens.OrthographicSize = value,
            originalOrthoSize * 0.5f, 1.5f)
            .SetEase(Ease.InOutQuad);
        
        // TrackedObjectOffset을 조정하여 카메라가 비추는 위치를 낮춤
        DOTween.To(() => framingTransposer.m_TrackedObjectOffset,
            value => framingTransposer.m_TrackedObjectOffset = value,
            new Vector3(0, -1f, 0), 1.5f)
            .SetEase(Ease.InOutQuad);
        
        // 줌인이 완료될 때까지 대기
        yield return new WaitForSeconds(1.5f);
    
        // 잠시 대기
        yield return new WaitForSeconds(1f);

        // 천천히 줌아웃하며 원래 설정으로 복귀
        DOTween.To(() => virtualCamera.m_Lens.OrthographicSize,
            value => virtualCamera.m_Lens.OrthographicSize = value,
            originalOrthoSize, 1f)
            .SetEase(Ease.InOutQuad);

        DOTween.To(() => framingTransposer.m_TrackedObjectOffset,
            value => framingTransposer.m_TrackedObjectOffset = value,
            originalTrackedObjectOffset, 1f)
            .SetEase(Ease.InOutQuad);

        Managers.Instance.Stage.ClearCurrentStage();
        
        yield return new WaitForSeconds(1f);

        // 모든 카메라 설정 복구
        framingTransposer.m_DeadZoneDepth = originalDeadZoneDepth;
        framingTransposer.m_DeadZoneWidth = originalDeadZoneWidth;
        framingTransposer.m_DeadZoneHeight = originalDeadZoneHeight;

  
        // 페이드 아웃
        //yield return StartCoroutine(UI_Fade.Instance.FadeOutCoroutine(0.5f));

        // 플레이어 초기화
        //player.transform.position = playerStartPos;
        //Managers.Instance.Game.player.Hp = Managers.Instance.Game.player.MaxHp;
        //Managers.Instance.Game.UpdatePlayerStats();

        yield return new WaitForSeconds(0.5f);

        // 페이드 인
        //yield return StartCoroutine(UI_Fade.Instance.FadeInCoroutine(0.5f));
        
        // 스테이지 재시작

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


