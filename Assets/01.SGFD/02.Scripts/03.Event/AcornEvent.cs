using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // Photon Unity Networking

public class AcornEvent : MonoBehaviourPun
{
    [SerializeField] GameObject acornPrefab; // 생성할 아콘 프리팹
    [SerializeField] GameObject goldPrefab; // 생성할 골드 프리팹
    [SerializeField] Transform basket; // 바구니의 트랜스폼
    [SerializeField] float radius = 10f; // 아콘이 생성될 주변 반경
    [SerializeField] GameObject clearPtc;

    [SerializeField] int acornCount;

    [SerializeField] GameObject eventTextPanel;
    [SerializeField] TextAnim textanim;

    [SerializeField] TextAnim Squireltextanim;

    private bool isCoroutineRunning = false; // Coroutine running flag

    private void OnEnable()
    {
        // 바구니의 자식 오브젝트 상태를 감시하기 위한 코루틴 시작
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
        // 마스터 클라이언트가 모든 클라이언트에 아콘을 생성하도록 명령
        SpawnAcorns(11);

        // 자식 오브젝트 모두 비활성화
        photonView.RPC("DisableAllChildren", RpcTarget.All);
    }

    [PunRPC]
    void DisableAllChildren()
    {
        eventTextPanel.SetActive(false);
        eventTextPanel.SetActive(true);
        textanim.textToShow = "다람쥐를 도와 도토리를 바구니에 담으세요!";
        Squireltextanim.textToShow = "<shake>내 도토리 ㅜㅜ\r\n이걸 다 언제담아ㅜㅜ<shake>";
        foreach (Transform child in basket)
        {
            child.gameObject.SetActive(false);
        }

        // DisableAllChildren을 호출한 후 다시 자식 오브젝트 상태를 감시하기 위한 코루틴 시작
        if (!isCoroutineRunning)
        {
            StartCoroutine(CheckAllChildrenActive());
        }
    }

    void SpawnAcorns(int count)
    {
        for (int i = 0; i < count; i++)
        {
            // 무작위 위치를 생성합니다.
            Vector3 randomPosition = transform.position + Random.insideUnitSphere * radius;

            // Y축 위치를 고정하려면 아래와 같이 변경
            randomPosition.y = transform.position.y + 0.2f;
            // 네트워크를 통해 아콘을 생성합니다.
            PhotonNetwork.Instantiate(acornPrefab.name, randomPosition, Quaternion.identity);
        }
    }

    IEnumerator CheckAllChildrenActive()
    {
        isCoroutineRunning = true; // Set the flag to true

        while (true)
        {
            yield return new WaitForSeconds(0.1f); // 1초마다 체크

            if (AreAllChildrenActive(basket))
            {
                Squireltextanim.textToShow = "<wave>고마워!!<wave>";
                AudioManager.instance.PlaySound(transform.position, 8, Random.Range(1f, 1f), 1f);
                clearPtc.SetActive(false);
                clearPtc.SetActive(true);
                Debug.Log("골드생성");
                SpawnGold();
                isCoroutineRunning = false; // Set the flag to false
                yield break; // 골드를 생성한 후 이 코루틴을 중지
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
            // 바구니 위치에 골드 생성
            Vector3 spawnPosition = basket.position;
            for (int i = 0; i < 20; i++)
            {
                GameObject gold = PhotonNetwork.Instantiate(goldPrefab.name, spawnPosition + new Vector3(0, 3, 0), Quaternion.identity);
                Gold goldComponent = gold.GetComponent<Gold>();
                goldComponent.isget = false;

                // 모든 "Player" 태그가 붙은 오브젝트를 찾음
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

                if (players.Length > 0)
                {
                    // 랜덤으로 타겟을 선택
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
