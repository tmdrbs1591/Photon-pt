using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class StageManager : MonoBehaviourPun
{
    public static StageManager instance;
    public List<Transform> StartPosition = new List<Transform>();

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
        if (PhotonNetwork.IsMasterClient)
        {
            // 랜덤 위치 선택
            int randomIndex = Random.Range(0, StartPosition.Count);
            Transform randomPosition = StartPosition[randomIndex];

            // 모든 클라이언트에서 RPC 호출
            photonView.RPC("MovePlayer", RpcTarget.All, randomPosition.position, randomPosition.rotation);
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

    IEnumerator TP(GameObject player,Vector3 position, Quaternion rotation)
    {
        NetworkManager.instance.Fade();
        yield return new WaitForSeconds(1.3f);
        player.transform.position = position;
        player.transform.rotation = rotation;
    }
}
