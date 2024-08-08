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

[System.Serializable]
public class EventStageInfo
{
    public Transform SpawnPos;
    public Transform PortalPos;
}

public class StageManager : MonoBehaviourPunCallbacks, IPunObservable
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
    public List<EventStageInfo> eventStage = new List<EventStageInfo>();
    public int stagePercentage = 5;
    public AcornEvent arconEvent;
    public OstrichEvent ostrichEvent;
    public Flag flag;

    [Header("Monster")]
    public GameObject[] monsterPrefab;
    private List<GameObject> currentSpawnMonsters = new List<GameObject>();
    private int killCount = 0;
    private int totalMonsters = 0;
    private int currentStageMonsterListLength = 1;
    [SerializeField] private TextMeshProUGUI totalMonstersText;

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
    [SerializeField] private Image stageBar;

    [Header("StatUp")]
    [SerializeField] private float hpUp = 0;
    [SerializeField] private float attackUp = 0;
    [SerializeField] private float bossHpUp = 0;
    [SerializeField] private float bossAttackUp = 0;

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

    public void NextStage()
    {
        // ��ٿ� Ȯ��
        if (Time.time - lastStageChangeTime < stageCooldown)
            return;

        if (PhotonNetwork.IsMasterClient)
        {
            // ������ Ŭ���̾�Ʈ������ �������� ���� ������ ����
            if (currentStage == 10)
            {
                currentStageMonsterListLength++;

                if (currentStageMonsterListLength > monsterPrefab.Length)
                {
                    currentStageMonsterListLength = monsterPrefab.Length;
                }
            }

            // �������� ����
            currentStage++;

            stageBar.fillAmount = 0;

            // stageBar�� �ε巴�� 0.25���� ������Ű�� �ڷ�ƾ ȣ��
            StartCoroutine(FillStageBar(0.28f, 1.5f)); // 1.5�� ���� 0.25���� ����

            Transform targetPosition = null;

            currentSpawnMonsters.Clear();
            killCount = 0;
            totalMonsters = 0; // totalMonsters �ʱ�ȭ

            // 5, 15, 25, 35 ���������� ���� ShopPosition���� �̵�
            if (currentStage > 0 && currentStage % 10 == 5)
            {
                Debug.Log("���� �������� �����մϴ�. ���� �������� : " + currentStage);
                int shopIndex = Random.Range(0, shopPosition.Count);
                targetPosition = shopPosition[shopIndex];
            }
            // 10, 20, 30, 40, 50 ���������� ���� BossPosition���� �̵�
            else if (currentStage > 0 && currentStage % 10 == 0)
            {
                Debug.Log("���� �������� �����մϴ�. ���� �������� : " + currentStage);
                int bossIndex = Random.Range(0, bossPosition.Count);
                int bossMonsterIndex = Random.Range(0, bossMonsters.Count);
                targetPosition = bossPosition[bossIndex].spawnPos;

                // ��Ż ��ġ ����
                photonView.RPC("SetPortalPosition", RpcTarget.All, bossPosition[bossIndex].portalPos.position);
                // ��Ż ��Ȱ��ȭ
                photonView.RPC("SetPortalState", RpcTarget.All, false);

                // spawnPos ��� monsterSpawnPos�� ����Ͽ� �ùٸ��� �ݺ����� ������ ����
                foreach (Transform t in bossPosition[bossIndex].monsterSpawnPos)
                {
                    GameObject boss = PhotonNetwork.Instantiate(bossMonsters[bossMonsterIndex].bossObj.name, t.position, t.rotation);
                    currentSpawnMonsters.Add(boss);
                    totalMonsters++;
                }
            }
            else
            {
                int isEventStage = Random.Range(0, stagePercentage);

                if (isEventStage == 0)
                {
                    int randomIndex = Random.Range(0, eventStage.Count);
                    targetPosition = eventStage[randomIndex].SpawnPos;

                    // ��Ż ��ġ ����
                    photonView.RPC("SetPortalPosition", RpcTarget.All, eventStage[randomIndex].PortalPos.position);
                    // ��Ż ��Ȱ��ȭ
                    photonView.RPC("SetPortalState", RpcTarget.All, false);

                    StartCoroutine(Co_StartEventStage(randomIndex));
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

                    int monsterIndex;

                    foreach (Transform t in stageInfos[randomIndex - 1].monsterSpawnPos)
                    {
                        monsterIndex = Random.Range(0, currentStageMonsterListLength);
                        GameObject monster = PhotonNetwork.Instantiate(monsterPrefab[monsterIndex].name, t.position, t.rotation);
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

            // totalMonsters �� ����ȭ
            photonView.RPC("SyncTotalMonsters", RpcTarget.All, totalMonsters);

            // �������� ������ ���� �� �߰� ����ȭ ȣ��
            photonView.RPC("SyncStageData", RpcTarget.All, currentStage, killCount, totalMonsters);
        }
        photonView.RPC("UpdateText", RpcTarget.All);
    }

    [PunRPC]
    private void SyncTotalMonsters(int total)
    {
        totalMonsters = total;
        totalMonstersText.text = "���� ���� �� : " + (totalMonsters - killCount);
    }

    [PunRPC]
    private void SyncStageData(int stage, int kills, int total)
    {
        currentStage = stage;
        killCount = kills;
        totalMonsters = total;
        totalMonstersText.text = "���� ���� �� : " + (totalMonsters - killCount);
    }

    [PunRPC]
    public void UpdateText()
    {
        stageText.text = $"{currentStage} ��������";
        totalMonstersText.text = "���� ���� �� : " + (totalMonsters - killCount);
    }

    IEnumerator FillStageBar(float targetValue, float duration)
    {
        float startValue = stageBar.fillAmount;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            stageBar.fillAmount = Mathf.Lerp(startValue, targetValue, time / duration);
            yield return null;
        }

        stageBar.fillAmount = targetValue;
    }

    [PunRPC]
    public void MovePlayer(int stage, Vector3 position, Quaternion rotation)
    {
        currentStage = stage;

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

            if ((currentStage + stagePosUpandDown) % 10 == 0 && i != 4 && (currentStage + stagePosUpandDown) > 0)
            {
                stageIcons[5].icon.transform.position = stagePoss[i].position;
                stageIcons[5].icon.SetActive(true);
                isBoss = true;
            }
            else if ((currentStage + stagePosUpandDown) % 10 == 5 && i != 4 && (currentStage + stagePosUpandDown) > 0)
            {
                stageIcons[4].icon.transform.position = stagePoss[i].position;
                stageIcons[4].icon.SetActive(true);
                isShop = true;
            }
            else
            {
                if (i == 4)
                {
                    if (isShop == true)
                    {
                        stageIcons[5].icon.transform.position = stagePoss[i].position;
                        stageIcons[5].icon.SetActive(true);
                    }
                    else if (isBoss == true)
                    {
                        stageIcons[4].icon.transform.position = stagePoss[i].position;
                        stageIcons[4].icon.SetActive(true);
                    }
                    else if (!isShop && !isBoss)
                    {
                        int nearest5 = FindNearestMultiple(currentStage, 5);
                        int nearest10 = FindNearestMultiple(currentStage, 10);

                        if (Mathf.Abs(currentStage - nearest5) < Mathf.Abs(currentStage - nearest10))
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
                else
                {
                    stageIcons[defaulIconCount].icon.transform.position = stagePoss[i].position;
                    stageIcons[defaulIconCount].icon.SetActive(true);
                    defaulIconCount++;
                }
            }
        }
    }

    int FindNearestMultiple(int num, int multiple)
    {
        int lower = (num / multiple) * multiple;
        int upper = lower + multiple;

        if (Mathf.Abs(num - lower) < Mathf.Abs(num - upper))
        {
            return lower;
        }
        else
        {
            return upper;
        }
    }

    public void KillMonster()
    {
        killCount++;
        stageBar.fillAmount = (float)killCount / totalMonsters;

        if (killCount >= totalMonsters)
        {
            portalObj.SetActive(true);
        }

        photonView.RPC("UpdateKillCount", RpcTarget.Others, killCount);
    }

    [PunRPC]
    private void SetPortalState(bool state)
    {
        portalObj.SetActive(state);
    }

    [PunRPC]
    private void SetPortalPosition(Vector3 position)
    {
        portalObj.transform.position = position;
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
            photonView.RPC("UpdateKillCount", RpcTarget.All, killCount); // killCount ����ȭ
            photonView.RPC("UpdateText", RpcTarget.All);
        }
    }

    [PunRPC]
    public void EventCheck()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (flag.isClear || arconEvent.isEventEnd)
            {
                photonView.RPC("SetPortalState", RpcTarget.All, true);
                flag.isClear = false;
                arconEvent.isEventEnd = false;
            }
        }
    }

    IEnumerator Co_StartEventStage(int eventStage)
    {
        yield return new WaitForSeconds(2);
        if (eventStage == 0)
        {
            arconEvent.EventStart();
        }
        else if (eventStage == 1)
        {
            ostrichEvent.photonView.RPC("EventStart", RpcTarget.All);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentStage);
            stream.SendNext(killCount);
            stream.SendNext(totalMonsters);
        }
        else
        {
            currentStage = (int)stream.ReceiveNext();
            killCount = (int)stream.ReceiveNext();
            totalMonsters = (int)stream.ReceiveNext();
        }
    }

    [PunRPC]
    private void UpdateKillCount(int newKillCount)
    {
        killCount = newKillCount;
        totalMonstersText.text = "���� ���� �� : " + (totalMonsters - killCount);
    }
}
