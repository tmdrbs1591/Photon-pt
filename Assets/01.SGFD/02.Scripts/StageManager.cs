using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class StageManager : MonoBehaviourPun
{
    public static StageManager instance;
    public List<Transform> StartPosition = new List<Transform>();
    public List<Transform> ShopPosition = new List<Transform>();
    public int currentStage = 0;

    private float stageCooldown = 2f; // 다음 스테이지로 이동할 수 있는 쿨다운 시간 (초)
    private float lastStageChangeTime = 0f; // 마지막 스테이지 변경 시간

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 전환되어도 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void NextStage()
    {
        // 쿨다운 확인
        if (Time.time - lastStageChangeTime < stageCooldown)
            return;

        if (PhotonNetwork.IsMasterClient)
        {
            Transform targetPosition;

            // 스테이지 증가
            currentStage++;
            // 현재 스테이지가 5, 10, 15, 20, 25인지 체크
            if (currentStage > 0 && currentStage % 5 == 0) // 5, 10, 15, 20, 25 스테이지에서 ShopPosition으로 이동
            {
                int shopIndex = Random.Range(0, ShopPosition.Count);
                targetPosition = ShopPosition[shopIndex];
            }
            else
            {
                int randomIndex = Random.Range(0, StartPosition.Count);
                targetPosition = StartPosition[randomIndex];
            }

            lastStageChangeTime = Time.time; // 현재 시간을 마지막 변경 시간으로 설정

            // 모든 클라이언트에서 RPC 호출
            photonView.RPC("MovePlayer", RpcTarget.All, targetPosition.position, targetPosition.rotation);
        }
    }



    [PunRPC]
    public void MovePlayer(Vector3 position, Quaternion rotation)
    {
        // 플레이어를 랜덤 위치로 이동
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            StartCoroutine(TP(player, position, rotation));
        }
    }

    IEnumerator TP(GameObject player, Vector3 position, Quaternion rotation)
    {
        NetworkManager.instance.Fade();
        yield return new WaitForSeconds(1.3f);
        player.transform.position = position;
        player.transform.rotation = rotation;
    }
}
