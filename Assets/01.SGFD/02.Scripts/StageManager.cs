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

    private float stageCooldown = 2f; // ���� ���������� �̵��� �� �ִ� ��ٿ� �ð� (��)
    private float lastStageChangeTime = 0f; // ������ �������� ���� �ð�

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

}
