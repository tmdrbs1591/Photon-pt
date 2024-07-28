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
        if (other.gameObject.CompareTag("Player") && !isClear) // �������� Ŭ������ ���°� �ƴҶ���
        {
            Ostrichtextanim.textToShow = "<shake>���� �̤�<shake>";
            photonView.RPC("SpawnGold", RpcTarget.All);
            isClear = true;
        }
        if (other.gameObject.CompareTag("Ostrich") && !isClear) // �������� Ŭ������ ���°� �ƴҶ���
        {
            Ostrichtextanim.textToShow = "<wave>���� �̰��~~!<wave>";
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
            // �ٱ��� ��ġ�� ��� ����
            for (int i = 0; i < 20; i++)
            {
                GameObject gold = PhotonNetwork.Instantiate(goldPrefab.name, transform.position + new Vector3(0, 3, 0), Quaternion.identity);
                Gold goldComponent = gold.GetComponent<Gold>();
                if (goldComponent != null)
                {
                    goldComponent.isget = false;

                    // ��� "Player" �±װ� ���� ������Ʈ�� ã��
                    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

                    if (players.Length > 0)
                    {
                        // �������� Ÿ���� ����
                        GameObject randomPlayer = players[Random.Range(0, players.Length)];
                        goldComponent.target = randomPlayer.transform;
                    }
                }
            }
        }
    }
}
