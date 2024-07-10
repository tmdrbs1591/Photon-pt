using Cinemachine;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Enemy : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] float currentHP;
    [SerializeField] float maxHP;
    [SerializeField] Slider hpBar;

    Animator anim;
    private GameObject playerObj;
    private PhotonView PV;
    NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        currentHP = maxHP;
    }

    void Update()
    {
        if (playerObj == null)
        {
            // 중심 위치와 반지름을 설정하여 오버랩 서클 사용
            Collider[] colliders = Physics.OverlapSphere(transform.position, 5f);

            // 오버랩 서클에서 플레이어 태그를 가진 오브젝트를 찾기
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    playerObj = collider.gameObject;

                    // 플레이어를 찾았으므로 중지
                    break;
                }
            }
        }

        if (playerObj != null)
        {
            Vector3 moveVec = (playerObj.transform.position - transform.position).normalized;
            anim.SetBool("isWalk", moveVec != Vector3.zero);

            Quaternion targetRotation = Quaternion.LookRotation(moveVec); // 목표 회전을 이동 방향 벡터로 설정
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 3 * Time.deltaTime); // 현재 회전에서 목표 회전까지 부드럽게 회전

            // 플레이어가 존재할 경우, 플레이어 방향으로 이동
            agent.SetDestination(playerObj.transform.position);

            // 적의 위치를 동기화
            if (photonView.IsMine)
            {
                photonView.RPC("SyncEnemyPosition", RpcTarget.Others, transform.position);
            }
        }

        // NavMeshAgent가 목적지에 도달했는지 확인
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                // 목적지에 도달했을 때
                anim.SetBool("isWalk", false);

                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    // 추가적인 조건을 충족할 때 (정지 상태)
                    anim.SetBool("isWalk", false);
                }
            }
            else
            {
                // 이동 중일 때
                anim.SetBool("isWalk", true);
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

    // Photon RPC 메서드로 변경
    [PunRPC]
    public void TakeDamage(float damage)
    {
        anim.SetTrigger("isDamage");
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
