using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // Photon Unity Networking

public class AcornEvent : MonoBehaviourPun
{
    [SerializeField] GameObject acornPrefab; // ������ ���� ������
    [SerializeField] GameObject goldPrefab; // ������ ��� ������
    [SerializeField] Transform basket; // �ٱ����� Ʈ������
    [SerializeField] float radius = 10f; // ������ ������ �ֺ� �ݰ�
    [SerializeField] GameObject clearPtc;

    [SerializeField] int acornCount;

    [SerializeField] GameObject eventTextPanel;
    [SerializeField] TextAnim textanim;

    [SerializeField] TextAnim Squireltextanim;

    private bool isCoroutineRunning = false; // Coroutine running flag

    private void OnEnable()
    {
        // �ٱ����� �ڽ� ������Ʈ ���¸� �����ϱ� ���� �ڷ�ƾ ����
        if (!isCoroutineRunning)
        {
            StartCoroutine(CheckAllChildrenActive());
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            EventStart();
        }
    }

    void EventStart()
    {
        // ������ Ŭ���̾�Ʈ�� ��� Ŭ���̾�Ʈ�� ������ �����ϵ��� ���
        SpawnAcorns(11);

        // �ڽ� ������Ʈ ��� ��Ȱ��ȭ
        photonView.RPC("DisableAllChildren", RpcTarget.All);
    }

    [PunRPC]
    void DisableAllChildren()
    {
        eventTextPanel.SetActive(false);
        eventTextPanel.SetActive(true);
        textanim.textToShow = "�ٶ��㸦 ���� ���丮�� �ٱ��Ͽ� ��������!";
        Squireltextanim.textToShow = "<shake>�� ���丮 �̤�\r\n�̰� �� ������Ƥ̤�<shake>";
        foreach (Transform child in basket)
        {
            child.gameObject.SetActive(false);
        }

        // DisableAllChildren�� ȣ���� �� �ٽ� �ڽ� ������Ʈ ���¸� �����ϱ� ���� �ڷ�ƾ ����
        if (!isCoroutineRunning)
        {
            StartCoroutine(CheckAllChildrenActive());
        }
    }

    void SpawnAcorns(int count)
    {
        for (int i = 0; i < count; i++)
        {
            // ������ ��ġ�� �����մϴ�.
            Vector3 randomPosition = transform.position + Random.insideUnitSphere * radius;

            // Y�� ��ġ�� �����Ϸ��� �Ʒ��� ���� ����
            randomPosition.y = transform.position.y + 0.2f;
            // ��Ʈ��ũ�� ���� ������ �����մϴ�.
            PhotonNetwork.Instantiate(acornPrefab.name, randomPosition, Quaternion.identity);
        }
    }

    IEnumerator CheckAllChildrenActive()
    {
        isCoroutineRunning = true; // Set the flag to true

        while (true)
        {
            yield return new WaitForSeconds(0.1f); // 1�ʸ��� üũ

            if (AreAllChildrenActive(basket))
            {
                Squireltextanim.textToShow = "<wave>����!!<wave>";
                AudioManager.instance.PlaySound(transform.position, 8, Random.Range(1f, 1f), 1f);
                clearPtc.SetActive(false);
                clearPtc.SetActive(true);
                Debug.Log("������");
                SpawnGold();
                isCoroutineRunning = false; // Set the flag to false
                yield break; // ��带 ������ �� �� �ڷ�ƾ�� ����
            }
        }
    }

    bool AreAllChildrenActive(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (!child.gameObject.activeSelf)
            {
                return false;
            }
        }
        return true;
    }

    void SpawnGold()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // �ٱ��� ��ġ�� ��� ����
            Vector3 spawnPosition = basket.position;
            for (int i = 0; i < 20; i++)
            {
                GameObject gold = PhotonNetwork.Instantiate(goldPrefab.name, spawnPosition + new Vector3(0, 3, 0), Quaternion.identity);
                Gold goldComponent = gold.GetComponent<Gold>();
                goldComponent.isget = false;

                // ��� "Player" �±װ� ���� ������Ʈ�� ã��
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

                if (players.Length > 0)
                {
                    // �������� Ÿ���� ����
                    GameObject randomPlayer = players[Random.Range(0, players.Length)];
                    goldComponent.target = randomPlayer.transform;
                }
                else
                {
                    Debug.LogWarning("No players found with the 'Player' tag.");
                }
            }
        }
    }
}
