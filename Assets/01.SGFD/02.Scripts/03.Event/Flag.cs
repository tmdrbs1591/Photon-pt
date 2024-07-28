using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Flag : MonoBehaviourPun
{
    [SerializeField] GameObject clearPtc;
    [SerializeField] GameObject goldPrefab;


    [SerializeField] bool isClear = false;
    [SerializeField] TextAnim Ostrichtextanim;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !isClear) // 누군가가 클리어한 상태가 아닐때만
        {
            Ostrichtextanim.textToShow = "<shake>졌다 ㅜㅜ<shake>";
            photonView.RPC("SpawnGold", RpcTarget.All);
            isClear = true;
        }
        if (other.gameObject.CompareTag("Ostrich") && !isClear) // 누군가가 클리어한 상태가 아닐때만
        {
            Ostrichtextanim.textToShow = "<wave>내가 이겼다~~!<wave>";
            isClear = true;
        }
    }

    [PunRPC]
    void SpawnGold()
    {
        if (clearPtc != null)
        {
            AudioManager.instance.PlaySound(transform.position, 8, Random.Range(1f, 1f), 1f);
            clearPtc.SetActive(false);
            clearPtc.SetActive(true);
        }
       

        if (PhotonNetwork.IsMasterClient)
        {
            // 바구니 위치에 골드 생성
            for (int i = 0; i < 20; i++)
            {
                GameObject gold = PhotonNetwork.Instantiate(goldPrefab.name, transform.position + new Vector3(0, 3, 0), Quaternion.identity);
                Gold goldComponent = gold.GetComponent<Gold>();
                if (goldComponent != null)
                {
                    goldComponent.isget = false;

                    // 모든 "Player" 태그가 붙은 오브젝트를 찾음
                    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

                    if (players.Length > 0)
                    {
                        // 랜덤으로 타겟을 선택
                        GameObject randomPlayer = players[Random.Range(0, players.Length)];
                        goldComponent.target = randomPlayer.transform;
                    }
                }
            }
        }
    }
}
