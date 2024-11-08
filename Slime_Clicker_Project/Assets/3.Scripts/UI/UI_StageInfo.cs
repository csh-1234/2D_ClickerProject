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

    private const int STAGES_PER_CYCLE = 5; // �� ����Ŭ�� �������� ��
    private readonly Vector3 playerStartPos = new Vector2(0, 8.58f); // �÷��̾� ���� ��ġ ����
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
                    print("�������� �й�");
                    yield return StartCoroutine(DefeatAnimation());
                    Managers.Instance.Sound.Play("Fail", SoundManager.Sound.Effect);
                    StartCoroutine(ShowStageAlarm(StageFailAlarm));
                    SetCurrentStageLevel();
                    stageCounter++;
                    break;
                }
                else if (Managers.Instance.Game.MonsterList.Count == 0)
                {
                    print("�������� Ŭ���� - ���� �����Ϸ�");
                    Managers.Instance.Stage.AddCurrentStageLevel();
                    Managers.Instance.Sound.Play("Clear", SoundManager.Sound.Effect);
                    StartCoroutine(ShowStageAlarm(StageClearAlarm));
                    SetCurrentStageLevel();
                    stageCounter++;
                    break;
                }
            }

            RestorePlayerHp();
            print("���ο� ��������");

            // ���� ���������� 5�� ����� �� ����
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

        // ��� ���͵��� �ڷ� �������� ��
        foreach (var monster in Managers.Instance.Game.MonsterList)
        {
            monster.RetreatFromPlayer(2f); // ī�޶� ���� �ð��� �����ϰ� ����
        }
        // ��� ���
        yield return new WaitForSeconds(2f);
        Managers.Instance.Stage.ClearCurrentStage();
        yield return new WaitForSeconds(1.5f);
    }
   
    private IEnumerator ClearAnimation(bool resetPosition)
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
            Debug.Log($"�÷��̾� ��ġ :{playerStartPos.x}, {playerStartPos.y}");

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


