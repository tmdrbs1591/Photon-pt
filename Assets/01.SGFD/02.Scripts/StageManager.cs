using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class StageManager : MonoBehaviourPun
{
    public static StageManager instance;
    public List<Transform> StartPosition = new List<Transform>();

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

    public void NextStage()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // ���� ��ġ ����
            int randomIndex = Random.Range(0, StartPosition.Count);
            Transform randomPosition = StartPosition[randomIndex];

            // ��� Ŭ���̾�Ʈ���� RPC ȣ��
            photonView.RPC("MovePlayer", RpcTarget.All, randomPosition.position, randomPosition.rotation);
        }
    }

    [PunRPC]
    public void MovePlayer(Vector3 position, Quaternion rotation)
    {
        // �÷��̾ ���� ��ġ�� �̵�
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            StartCoroutine(TP(player, position, rotation));
        }
    }

    IEnumerator TP(GameObject player,Vector3 position, Quaternion rotation)
    {
        NetworkManager.instance.Fade();
        yield return new WaitForSeconds(1.3f);
        player.transform.position = position;
        player.transform.rotation = rotation;
    }
}
