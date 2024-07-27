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
        StageIcon();
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

            // 5, 15, 25, 35 ���������� ���� ShopPosition���� �̵�
            if (currentStage > 0 && currentStage % 10 == 5)
            {
                int shopIndex = Random.Range(0, ShopPosition.Count);
                targetPosition = ShopPosition[shopIndex];
            }
            // 10, 20, 30, 40, 50 ���������� ���� BossPosition���� �̵�
            else if (currentStage > 0 && currentStage % 10 == 0)
            {
                int bossIndex = Random.Range(0, BossPosition.Count);
                targetPosition = BossPosition[bossIndex];
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
                    PhotonNetwork.Instantiate(monsterPrefab.name, t.position, t.rotation);
                }
                lastStage = randomIndex;
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
            photonView.RPC("StageIcon", RpcTarget.All);
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

}
