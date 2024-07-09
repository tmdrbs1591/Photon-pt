using Photon.Pun;  // Photon.Pun ���ӽ����̽� �߰�
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviourPunCallbacks, IPunObservable  // IPunObservable �������̽� �߰�
{
    [SerializeField] float currentHP;
    [SerializeField] float maxHP;

    [SerializeField] Slider hpBar;

    public PhotonView PV;

    void Start()
    {
        currentHP = maxHP;
        // �ʱ� ���� �ʿ� �� �߰�
    }

    void Update()
    {
        PV.RPC("HPbarUpdate", RpcTarget.AllBuffered);
        if (currentHP <= 0)
        {
            Destroy(gameObject);
        }
    }
    [PunRPC]
    public void HPbarUpdate()
    {
        hpBar.value = currentHP / maxHP;
    }
    // Photon RPC �޼���� ����
    [PunRPC]
    public void TakeDamage(float damage)
    {
        currentHP -= damage;
    }

    // IPunObservable �������̽� ����
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // ������ ���� (���� �÷��̾��� �����͸� ����)
            stream.SendNext(currentHP);
        }
        else
        {
            // ������ ���� (���� �÷��̾��� �����͸� ����)
            currentHP = (float)stream.ReceiveNext();
        }
    }
}
