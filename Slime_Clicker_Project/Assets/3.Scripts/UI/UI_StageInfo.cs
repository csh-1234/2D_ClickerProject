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

    private const int STAGES_PER_CYCLE = 5; // �� ����Ŭ�� �������� ��
    private Vector3 playerStartPos; // �÷��̾� ���� ��ġ ����

 

    private int min = 0;
    private float sec = 30f;

    protected override void Awake()
    {
        base.Awake();
        SetCurrentStageLevel();
        // ���̵� �г� �ʱ�ȭ
      
        if(Managers.Instance.Game.player!=null)
        {
            playerStartPos = Managers.Instance.Game.player.transform.position;

        }

        // ī�޶� ������Ʈ �ʱ�ȭ
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
                    print("Ÿ�Ӿƿ�");
                    StageTime.text = "00:30";
                    yield return StartCoroutine(PlayPlayerDeathAnimation());
                    SetCurrentStageLevel();
                    stageCounter++;
                    break;
                }
                else if (Managers.Instance.Game.player.Hp <= 0)
                {
                    print("�÷��̾� ���");
                    yield return StartCoroutine(PlayPlayerDeathAnimation());
                    stageCounter++;
                    SetCurrentStageLevel();
                    break;
                }
                else if (Managers.Instance.Game.MonsterList.Count == 0)
                {
                    print("�������� Ŭ���� - ���� �����Ϸ�");
                    Managers.Instance.Stage.AddCurrentStageLevel();
                    SetCurrentStageLevel();
                    stageCounter++;
                    break;
                }
            }

            Managers.Instance.Game.player.Hp = Managers.Instance.Game.player.MaxHp;
            Managers.Instance.Game.UpdatePlayerStats();

            print("���ο� ��������");
            // ���� ���������� 5�� ����� ���� �������� ����
            int currentStage = Managers.Instance.Stage.GetCurrentStageLevel();
            bool isEndOfCycle = stageCounter % STAGES_PER_CYCLE == 0;

            yield return StartCoroutine(PlayPlayerMoveAnimation(isEndOfCycle));
            yield return new WaitForSeconds(2f);
        }
    }


    private IEnumerator PlayPlayerDeathAnimation()
    {
        var player = Managers.Instance.Game.player;

        // ���� ī�޶� ���� ����
        float originalOrthoSize = virtualCamera.m_Lens.OrthographicSize;
        Vector3 originalTrackedObjectOffset = framingTransposer.m_TrackedObjectOffset;
        float originalDeadZoneDepth = framingTransposer.m_DeadZoneDepth;
        float originalDeadZoneWidth = framingTransposer.m_DeadZoneWidth;
        float originalDeadZoneHeight = framingTransposer.m_DeadZoneHeight;

        // �÷��̾� �ִϸ��̼� ����
        player.anim.SetBool("IsWalk", false);

        // ��� ���͵��� �ڷ� �������� ��
        foreach (var monster in Managers.Instance.Game.MonsterList)
        {
            monster.RetreatFromPlayer(1.5f); // ī�޶� ���� �ð��� �����ϰ� ����
        }

        // �÷��̾ ȭ�� �߾����� ��ġ�ϱ� ���� ����
        framingTransposer.m_DeadZoneDepth = 0;
        framingTransposer.m_DeadZoneWidth = 0;
        framingTransposer.m_DeadZoneHeight = 0;
        framingTransposer.m_ScreenX = 0.5f;
        framingTransposer.m_ScreenY = 0.5f;
        
        // õõ�� ����
        DOTween.To(() => virtualCamera.m_Lens.OrthographicSize,
            value => virtualCamera.m_Lens.OrthographicSize = value,
            originalOrthoSize * 0.5f, 1.5f)
            .SetEase(Ease.InOutQuad);
        
        // TrackedObjectOffset�� �����Ͽ� ī�޶� ���ߴ� ��ġ�� ����
        DOTween.To(() => framingTransposer.m_TrackedObjectOffset,
            value => framingTransposer.m_TrackedObjectOffset = value,
            new Vector3(0, -1f, 0), 1.5f)
            .SetEase(Ease.InOutQuad);
        
        // ������ �Ϸ�� ������ ���
        yield return new WaitForSeconds(1.5f);
    
        // ��� ���
        yield return new WaitForSeconds(1f);

        // õõ�� �ܾƿ��ϸ� ���� �������� ����
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

        // ��� ī�޶� ���� ����
        framingTransposer.m_DeadZoneDepth = originalDeadZoneDepth;
        framingTransposer.m_DeadZoneWidth = originalDeadZoneWidth;
        framingTransposer.m_DeadZoneHeight = originalDeadZoneHeight;

  
        // ���̵� �ƿ�
        //yield return StartCoroutine(UI_Fade.Instance.FadeOutCoroutine(0.5f));

        // �÷��̾� �ʱ�ȭ
        //player.transform.position = playerStartPos;
        //Managers.Instance.Game.player.Hp = Managers.Instance.Game.player.MaxHp;
        //Managers.Instance.Game.UpdatePlayerStats();

        yield return new WaitForSeconds(0.5f);

        // ���̵� ��
        //yield return StartCoroutine(UI_Fade.Instance.FadeInCoroutine(0.5f));
        
        // �������� �����

    }

    private IEnumerator PlayPlayerMoveAnimation(bool resetPosition)
    {
        var player = Managers.Instance.Game.player;
        Vector3 originalPos = player.transform.position;
        Vector3 targetPos = originalPos + Vector3.right * 6f;

        // �ȱ� �ִϸ��̼� ����
        player.anim.SetBool("IsWalk", true);

        // �̵� �ִϸ��̼�
        yield return player.transform
            .DOMove(targetPos, 2.5f)
            .SetEase(Ease.Linear)
            .WaitForCompletion();

        player.anim.SetBool("IsWalk", false);

        if (resetPosition)
        {
            // 5�������� �Ϸ� �� ���̵� �ƿ�
            yield return StartCoroutine(UI_Fade.Instance.FadeOutCoroutine(0.5f));

            // �÷��̾� ��ġ �ʱ�ȭ
            player.transform.position = playerStartPos;

            yield return new WaitForSeconds(0.5f);

            // ���̵� ��
            yield return StartCoroutine(UI_Fade.Instance.FadeInCoroutine(0.5f));
        }
        else
        {
            // �Ϲ� �������� Ŭ���� �� ���
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


