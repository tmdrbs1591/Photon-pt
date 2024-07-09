using Cinemachine;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] float currentHP;
    [SerializeField] float maxHP;

    [SerializeField] Slider hpBar;

     Animator anim;

    private GameObject playerObj;

    private PhotonView PV;

    void Start()
    {
        anim = GetComponent<Animator>();
        currentHP = maxHP;
        // �ʱ� ���� �ʿ� �� �߰�
    }

    void Update()
    {
       



        // �߽� ��ġ�� �������� �����Ͽ� ������ ��Ŭ ���
        Collider[] colliders = Physics.OverlapSphere(transform.position, 5f);

        // ������ ��Ŭ���� �÷��̾� �±׸� ���� ������Ʈ�� ã��
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                playerObj = collider.gameObject;


                Vector3 moveVec = (playerObj.transform.position - transform.position).normalized;
                anim.SetBool("isWalk", moveVec != Vector3.zero);


                Quaternion targetRotation = Quaternion.LookRotation(moveVec); // ��ǥ ȸ���� �̵� ���� ���ͷ� ����
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 3 * Time.deltaTime); // ���� ȸ������ ��ǥ ȸ������ �ε巴�� ȸ


                break;
            }
        }

        if (playerObj != null)
        {
            // �÷��̾ ������ ���, �÷��̾� �������� �̵�
            transform.position = Vector3.MoveTowards(transform.position, playerObj.transform.position, 2 * Time.deltaTime);

            // ���� ��ġ�� ����ȭ
            if (photonView.IsMine)
            {
                photonView.RPC("SyncEnemyPosition", RpcTarget.Others, transform.position);
            }
        }

        hpBar.value = currentHP / maxHP;
        if (currentHP <= 0)
        {
            Destroy(gameObject);
        }
    }

    [PunRPC]
    void SyncEnemyPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
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
