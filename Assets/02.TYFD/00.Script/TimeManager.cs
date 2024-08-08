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

    private void Start()
    {
        //if(PhotonNetwork.IsMasterClient)
        SetTimer(startTime);
    }

    private void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
        }
        else if (currentTime <= 0)
        {
            currentTime = 0;
        }

        timerText.text = "남은 시간 : " + Mathf.FloorToInt(currentTime).ToString();
    }

    [PunRPC]
    private void SetTimer(float time)
    {
        currentTime = time;
    }
}
