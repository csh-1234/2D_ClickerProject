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

    private const int STAGES_PER_CYCLE = 5; // �� ����Ŭ�� �������� ��
    private Vector3 playerStartPos; // �÷��̾� ���� ��ġ ����

 

    private int min = 2;
    private float sec = 0f;

    protected override void Awake()
    {
        base.Awake();
        SetCurrentStageLevel();
        // ���̵� �г� �ʱ�ȭ
      
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

            Managers.Instance.Game.player.Hp = Managers.Instance.Game.player.MaxHp;
            Managers.Instance.Game.UpdatePlayerStats();

            print("���ο� ��������");
            // ���� ���������� 5�� ����� ���� �������� ����
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


