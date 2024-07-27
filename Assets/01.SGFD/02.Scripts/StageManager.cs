using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;

[System.Serializable]
public class StageInfo
{
    public Transform spawnPos;
    public Transform[] monsterSpawnPos;
}

[System.Serializable]
public class StageIcon
{
    public string iconName;
    public GameObject icon;
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

    [Header("StageBar")]
    [SerializeField] public List<StageIcon> stageIcons = new List<StageIcon>();
    [SerializeField] public List<Transform> stagePoss = new List<Transform>();
    [SerializeField] private Slider stageBar;

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

    private void Start()
    {
        StageIcon();
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

            // 스테이지 아이콘 업데이트
            photonView.RPC("StageIcon", RpcTarget.All);
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

    [PunRPC]
    private void StageIcon()
    {
        foreach (var icon in stageIcons)
        {
            icon.icon.SetActive(false);
        }

        int defaulIconCount = 0;
        int stagePosUpandDown = 0;
        bool isShop = false;
        bool isBoss = false;

        for (int i = 0; i < stagePoss.Count; i++)
        {
            if (i == 0)
            {
                stagePosUpandDown = -1;
            }
            else if (i > 0)
            {
                stagePosUpandDown++;
            }
            Debug.Log("현재 표시할 스테이지 : " + (currentStage + stagePosUpandDown));

            if ((currentStage + stagePosUpandDown) % 10 == 0 && i != 4 && (currentStage + stagePosUpandDown) > 0) // 만약 10의 배수 (보스 스테이지)
            {
                stageIcons[5].icon.transform.position = stagePoss[i].position;
                stageIcons[5].icon.SetActive(true);
                isBoss = true;
                Debug.Log("보스 스테이지 칸번호 : " + i);
            }
            else if ((currentStage + stagePosUpandDown) % 5 == 0 && i != 4 && (currentStage + stagePosUpandDown) > 0) // 만약 5의 배수 (상점 스테이지)
            {
                if (!isBoss) // 이미 보스 스테이지가 설정되지 않은 경우에만 설정
                {
                    stageIcons[4].icon.transform.position = stagePoss[i].position;
                    stageIcons[4].icon.SetActive(true);
                    isShop = true;
                    Debug.Log("상점 스테이지 칸번호 : " + i);
                }
            }
            else if (i == 1)
            {
                stageIcons[6].icon.transform.position = stagePoss[i].position;
                stageIcons[6].icon.SetActive(true);
            }
            else if (i != 4)
            {
                stageIcons[defaulIconCount].icon.transform.position = stagePoss[i].position;
                stageIcons[defaulIconCount].icon.SetActive(true);
                defaulIconCount++;
            }

            if (i == 4)
            {
                Debug.Log("현재상점 : " + isShop);
                Debug.Log("현재보스 : " + isBoss);

                if (isShop)
                {
                    stageIcons[5].icon.transform.position = stagePoss[i].position;
                    stageIcons[5].icon.SetActive(true);
                }
                else if (isBoss)
                {
                    stageIcons[4].icon.transform.position = stagePoss[i].position;
                    stageIcons[4].icon.SetActive(true);
                }
                else
                {
                    if (currentStage - 10 > 0)
                    {
                        if (currentStage - 10 > 5)
                        {
                            stageIcons[5].icon.transform.position = stagePoss[i].position;
                            stageIcons[5].icon.SetActive(true);
                        }
                        else
                        {
                            stageIcons[4].icon.transform.position = stagePoss[i].position;
                            stageIcons[4].icon.SetActive(true);
                        }
                    }
                    else
                    {
                        if (currentStage > 5)
                        {
                            stageIcons[5].icon.transform.position = stagePoss[i].position;
                            stageIcons[5].icon.SetActive(true);
                        }
                        else
                        {
                            stageIcons[4].icon.transform.position = stagePoss[i].position;
                            stageIcons[4].icon.SetActive(true);
                        }
                    }
                }
            }
        }
    }

}
