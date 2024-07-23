using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

[System.Serializable]
public class StageInfo
{
    public Transform spawnPos;
    public Transform[] monsterSpawnPos;
}

public class StageManager : MonoBehaviourPun
{
    public static StageManager instance;
    public List<StageInfo> stageInfos = new List<StageInfo>();
    public List<Transform> ShopPosition = new List<Transform>();
    public List<Transform> BossPosition = new List<Transform>();
    public GameObject monsterPrefab;
    public int currentStage = 0;
    public int lastStage;

    [SerializeField] TMP_Text stageText;

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

    private void Update()
    {
        stageText.text = "STAGE " + currentStage;      
    }
    public void NextStage()
    {
        // 쿨다운 확인
        if (Time.time - lastStageChangeTime < stageCooldown)
            return;

        if (PhotonNetwork.IsMasterClient)
        {
            Transform targetPosition = null;

            // 스테이지 증가
            currentStage++;

            // 5, 15, 25, 35 스테이지일 때는 ShopPosition으로 이동
            if (currentStage > 0 && currentStage % 10 == 5)
            {
                int shopIndex = Random.Range(0, ShopPosition.Count);
                targetPosition = ShopPosition[shopIndex];
            }
            // 10, 20, 30, 40, 50 스테이지일 때는 BossPosition으로 이동
            else if (currentStage > 0 && currentStage % 10 == 0)
            {
                int bossIndex = Random.Range(0, BossPosition.Count);
                targetPosition = BossPosition[bossIndex];
            }
            else
            {
                // 일반 스테이지일 때는 stageInfos에서 랜덤하게 spawnPos 선택
                int randomIndex = Random.Range(1, stageInfos.Count);
                while (randomIndex == lastStage)
                {
                    randomIndex = Random.Range(1, stageInfos.Count);
                }

                targetPosition = stageInfos[randomIndex - 1].spawnPos;

                foreach (Transform t in stageInfos[randomIndex - 1].monsterSpawnPos)
                {
                    PhotonNetwork.Instantiate(monsterPrefab.name, t.position, t.rotation);
                }
                lastStage = randomIndex;
            }

            if (targetPosition != null)
            {
                lastStageChangeTime = Time.time; // 현재 시간을 마지막 변경 시간으로 설정

                // 모든 클라이언트에서 RPC 호출하여 currentStage와 함께 MovePlayer 메서드 실행
                photonView.RPC("MovePlayer", RpcTarget.All, currentStage, targetPosition.position, targetPosition.rotation);
            }
            else
            {
                Debug.LogError("Target position is null.");
            }
        }
    }

    [PunRPC]
    public void MovePlayer(int stage, Vector3 position, Quaternion rotation)
    {
        // 클라이언트에서 stage 값을 받아와서 설정
        currentStage = stage;

        // 플레이어를 랜덤 위치로 이동
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            StartCoroutine(TP(player, position, rotation));
        }
        else
        {
            Debug.LogError("Player not found.");
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
