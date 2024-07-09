using Photon.Pun;  // Photon.Pun 네임스페이스 추가
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviourPunCallbacks, IPunObservable  // IPunObservable 인터페이스 추가
{
    [SerializeField] float currentHP;
    [SerializeField] float maxHP;

    [SerializeField] Slider hpBar;

    public PhotonView PV;

    void Start()
    {
        currentHP = maxHP;
        // 초기 설정 필요 시 추가
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
    // Photon RPC 메서드로 변경
    [PunRPC]
    public void TakeDamage(float damage)
    {
        currentHP -= damage;
    }

    // IPunObservable 인터페이스 구현
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 데이터 전송 (로컬 플레이어의 데이터를 전송)
            stream.SendNext(currentHP);
        }
        else
        {
            // 데이터 수신 (원격 플레이어의 데이터를 수신)
            currentHP = (float)stream.ReceiveNext();
        }
    }
}
