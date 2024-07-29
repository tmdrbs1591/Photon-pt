using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Flag : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject clearPtc;
    [SerializeField] GameObject goldPrefab;
    [SerializeField] TextAnim Ostrichtextanim;

    public bool isClear = false;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("isClear"))
            {
                isClear = (bool)PhotonNetwork.CurrentRoom.CustomProperties["isClear"];
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !isClear)
        {
            Ostrichtextanim.textToShow = "<shake>졌다 ㅜㅜ<shake>";
            SetClearState(true);
            photonView.RPC("SpawnGold", RpcTarget.All);
        }
        if (other.gameObject.CompareTag("Ostrich") && !isClear)
        {
            Ostrichtextanim.textToShow = "<wave>내가 이겼다~~!<wave>";
            SetClearState(true);
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
            for (int i = 0; i < 20; i++)
            {
                GameObject gold = PhotonNetwork.Instantiate(goldPrefab.name, transform.position + new Vector3(0, 3, 0), Quaternion.identity);
                Gold goldComponent = gold.GetComponent<Gold>();
                if (goldComponent != null)
                {
                    goldComponent.isget = false;

                    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

                    if (players.Length > 0)
                    {
                        GameObject randomPlayer = players[Random.Range(0, players.Length)];
                        goldComponent.target = randomPlayer.transform;
                    }
                }
            }
        }
    }

    void SetClearState(bool state)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
            properties["isClear"] = state;
            PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("isClear"))
        {
            isClear = (bool)propertiesThatChanged["isClear"];
        }
    }
}
