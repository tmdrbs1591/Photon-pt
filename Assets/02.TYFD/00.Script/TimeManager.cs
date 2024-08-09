using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    [Header("Timer Setting")]
    [SerializeField] private float startTime = 0;
    [SerializeField] private float currentTime = 0;

    [Header("Timer text")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("GameEnd")]
    [SerializeField] private GameObject gameEndPanel;
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private GameObject returnMenuBtn;
    [SerializeField] private bool isGameEnd = false;

    private void Start()
    {
        //if(PhotonNetwork.IsMasterClient)
        SetTimer(startTime);
    }

    private void Update()
    {
        if (currentTime > 0 && StageManager.instance.currentStage % 10 != 5)
        {
            currentTime -= Time.deltaTime;
        }
        else if (currentTime <= 0)
        {
            currentTime = 0;
            if (!isGameEnd)
            {
                isGameEnd = true;
                StartCoroutine(Co_GameEndPanelTrigger());
            }
        }


        SetTimerText();
    }

    [PunRPC]
    private void SetTimer(float time)
    {
        currentTime = time;
    }

    private void SetTimerText()
    {
        if (currentTime >= 60)
        {
            if (currentTime % 60 < 10)
            {
                timerText.text = "남은 시간 " + Mathf.FloorToInt(currentTime / 60).ToString() + " : 0" + Mathf.FloorToInt(currentTime % 60);
            }
            else
            {
                timerText.text = "남은 시간 " + Mathf.FloorToInt(currentTime / 60).ToString() + " : " + Mathf.FloorToInt(currentTime % 60);
            }
        }
        else if (currentTime < 60)
        {
            if (currentTime % 60 < 10)
            {
                timerText.text = "남은 시간 0 : 0" + Mathf.FloorToInt(currentTime % 60);
            }
            else
            {
                timerText.text = "남은 시간 0 : " + Mathf.FloorToInt(currentTime % 60);
            }
        }
    }

    IEnumerator Co_GameEndPanelTrigger()
    {
        gameEndPanel.SetActive(true);

        yield return new WaitForSeconds(2f);

        stageText.gameObject.SetActive(true);
        stageText.text = "최종 스테이지 : " + 0;
        //Debug.Log("스테이지 카운트" + stageCount);
        Debug.Log("현재 스테이지" + StageManager.instance.currentStage);

       for(int i = 0; i < StageManager.instance.currentStage; i++)
        {
            if (i >= StageManager.instance.currentStage)
            {
                i = StageManager.instance.currentStage;
            }
            stageText.text = "최종 스테이지 : " + i.ToString();
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(1f);

        returnMenuBtn.gameObject.SetActive(true);
    }
}
