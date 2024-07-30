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
    public Transform portalPos;
}

[System.Serializable]
public class BossMonster
{
    public string BossName;
    public GameObject bossObj;
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

    [Header("StageInfo")]
    public List<StageInfo> stageInfos = new List<StageInfo>();

    [Header("Shop")]
    public List<Transform> shopPosition = new List<Transform>();

    [Header("Boss")]
    public List<StageInfo> bossPosition = new List<StageInfo>();
    public List<BossMonster> bossMonsters = new List<BossMonster>();

    [Header("EventStage")]
    public List<Transform> eventStage = new List<Transform>();
    public int stagePercentage = 5;

    [Header("Monster")]
    public GameObject monsterPrefab;
    private List<GameObject> currentSpawnMonsters = new List<GameObject>();
    private int killCount = 0;
    private int totalMonsters = 0;

    [Header("StageCount")]
    public int currentStage = 0;
    public int lastStage;

    [Header("Portal")]
    public GameObject portalObj;

    [SerializeField] TMP_Text stageText;

    private float stageCooldown = 2f; // ���� ���������� �̵��� �� �ִ� ��ٿ� �ð� (��)
    private float lastStageChangeTime = 0f; // ������ �������� ���� �ð�

    [Header("StageBar")]
    [SerializeField] public List<StageIcon> stageIcons = new List<StageIcon>();
    [SerializeField] public List<Transform> stagePoss = new List<Transform>();
    [SerializeField] private Slider stageBar;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // ���� ��ȯ�Ǿ �ı����� �ʵ��� ����
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateStageIcons();
    }

    private void Update()
    {
        stageText.text = "STAGE " + currentStage;
    }

    public void NextStage()
    {
        // ��ٿ� Ȯ��
        if (Time.time - lastStageChangeTime < stageCooldown)
            return;

        if (PhotonNetwork.IsMasterClient)
        {
            Transform targetPosition = null;

            // �������� ����
            currentStage++;

            currentSpawnMonsters.Clear();
            killCount = 0;
            totalMonsters = 0; // totalMonsters �ʱ�ȭ

            // 5, 15, 25, 35 ���������� ���� ShopPosition���� �̵�
            if (currentStage > 0 && currentStage % 10 == 5)
            {
                int shopIndex = Random.Range(0, shopPosition.Count);
                targetPosition = shopPosition[shopIndex];
            }
            // 10, 20, 30, 40, 50 ���������� ���� BossPosition���� �̵�
            else if (currentStage > 0 && currentStage % 10 == 0)
            {
                int bossIndex = Random.Range(0, bossPosition.Count);
                targetPosition = bossPosition[bossIndex].spawnPos;
            }
            else
            {
                int isEventStage = Random.Range(0, stagePercentage);

                if (isEventStage == 0)
                {
                    int randomIndex = Random.Range(0, eventStage.Count);
                    targetPosition = eventStage[randomIndex];
                }
                else
                {
                    // �Ϲ� ���������� ���� stageInfos���� �����ϰ� spawnPos ����
                    int randomIndex = Random.Range(1, stageInfos.Count);
                    while (randomIndex == lastStage)
                    {
                        randomIndex = Random.Range(1, stageInfos.Count);
                    }

                    targetPosition = stageInfos[randomIndex - 1].spawnPos;

                    foreach (Transform t in stageInfos[randomIndex - 1].monsterSpawnPos)
                    {
                        GameObject monster = PhotonNetwork.Instantiate(monsterPrefab.name, t.position, t.rotation);
                        currentSpawnMonsters.Add(monster);
                        totalMonsters++; // ���� �� ����
                    }

                    // ��Ż ��ġ ����
                    photonView.RPC("SetPortalPosition", RpcTarget.All, stageInfos[randomIndex - 1].portalPos.position);
                    // ��Ż ��Ȱ��ȭ
                    photonView.RPC("SetPortalState", RpcTarget.All, false);

                    lastStage = randomIndex;
                }
            }

            if (targetPosition != null)
            {
                lastStageChangeTime = Time.time; // ���� �ð��� ������ ���� �ð����� ����

                // ��� Ŭ���̾�Ʈ���� RPC ȣ���Ͽ� currentStage�� �Բ� MovePlayer �޼��� ����
                photonView.RPC("MovePlayer", RpcTarget.All, currentStage, targetPosition.position, targetPosition.rotation);
            }
            else
            {
                Debug.LogError("Target position is null.");
            }

            // �������� ������ ������Ʈ
            photonView.RPC("UpdateStageIcons", RpcTarget.All);
        }
    }

    [PunRPC]
    public void MovePlayer(int stage, Vector3 position, Quaternion rotation)
    {
        // Ŭ���̾�Ʈ���� stage ���� �޾ƿͼ� ����
        currentStage = stage;

        // �÷��̾ ���� ��ġ�� �̵�
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
    private void UpdateStageIcons()
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
            Debug.Log("���� ǥ���� �������� : " + (currentStage + stagePosUpandDown));

            if ((currentStage + stagePosUpandDown) % 10 == 0 && i != 4 && (currentStage + stagePosUpandDown) > 0) // ���� 10�� ��� (���� ��������)
            {
                stageIcons[5].icon.transform.position = stagePoss[i].position;
                stageIcons[5].icon.SetActive(true);
                isBoss = true;
                Debug.Log("���� �������� ĭ��ȣ : " + i);
            }
            else if ((currentStage + stagePosUpandDown) % 5 == 0 && i != 4 && (currentStage + stagePosUpandDown) > 0) // ���� 5�� ��� (���� ��������)
            {
                if (!isBoss) // �̹� ���� ���������� �������� ���� ��쿡�� ����
                {
                    stageIcons[4].icon.transform.position = stagePoss[i].position;
                    stageIcons[4].icon.SetActive(true);
                    isShop = true;
                    Debug.Log("���� �������� ĭ��ȣ : " + i);
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
                Debug.Log("������� : " + isShop);
                Debug.Log("���纸�� : " + isBoss);

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

    [PunRPC]
    public void IncrementTotalMonsters()
    {
        totalMonsters++;
    }

    [PunRPC]
    public void MonsterDied()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            killCount++;
            if (killCount >= totalMonsters)
            {
                photonView.RPC("SetPortalState", RpcTarget.All, true);
            }
        }
    }

    [PunRPC]
    public void SetPortalState(bool state)
    {
        portalObj.SetActive(state);
    }

    [PunRPC]
    public void SetPortalPosition(Vector3 position)
    {
        portalObj.transform.position = position;
    }
}
