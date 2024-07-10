using Cinemachine;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Collections;

public class Enemy : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] float currentHP;
    [SerializeField] float maxHP;
    [SerializeField] float attackRange;
    [SerializeField] float attackDamage;
    [SerializeField] float maxAttackSpeed;
    [SerializeField] float curAttackSpeed;
    [SerializeField] Slider hpBar;
    [SerializeField] GameObject attackBox;


    private float playerDistance;

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
            // �߽� ��ġ�� �������� �����Ͽ� ������ ��Ŭ ���
            Collider[] colliders = Physics.OverlapSphere(transform.position, 5f);

            // ������ ��Ŭ���� �÷��̾� �±׸� ���� ������Ʈ�� ã��
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    playerObj = collider.gameObject;

                    // �÷��̾ ã�����Ƿ� ����
                    break;
                }
            }
        }

        if (playerObj != null)
        {
            Vector3 moveVec = (playerObj.transform.position - transform.position).normalized;
            anim.SetBool("isWalk", moveVec != Vector3.zero);

            Quaternion targetRotation = Quaternion.LookRotation(moveVec); // ��ǥ ȸ���� �̵� ���� ���ͷ� ����
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 3 * Time.deltaTime); // ���� ȸ������ ��ǥ ȸ������ �ε巴�� ȸ��

            // �÷��̾ ������ ���, �÷��̾� �������� �̵�
            agent.SetDestination(playerObj.transform.position);

            playerDistance = Vector3.Distance(gameObject.transform.position, playerObj.transform.position); // �÷��̾�� ������ �Ÿ� ���

            // ���ݼӵ� Ȯ��
            if (curAttackSpeed < 0)
            {
                // �Ÿ��� ���� ��Ÿ� �ȿ� �ִ��� Ȯ��
                if (playerDistance < attackRange)
                {
                    curAttackSpeed = maxAttackSpeed;
                    anim.SetTrigger("isAttack");
                    Debug.Log("�÷��̾ ������");
                }
            }
            else
            {
                curAttackSpeed -= Time.deltaTime;
            }

            // ���� ��ġ�� ����ȭ
            if (photonView.IsMine)
            {
                photonView.RPC("SyncEnemyPosition", RpcTarget.Others, transform.position);
            }
        }

        // NavMeshAgent�� �������� �����ߴ��� Ȯ��
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                // �������� �������� ��
                anim.SetBool("isWalk", false);

                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    // �߰����� ������ ������ �� (���� ����)
                    anim.SetBool("isWalk", false);
                }
            }
            else
            {
                // �̵� ���� ��
                anim.SetBool("isWalk", true);
            }
        }

        hpBar.value = currentHP / maxHP;
        if (currentHP <= 0)
        {
            Destroy(gameObject);
        }
    }

    void Attack()
    {
        photonView.RPC("RPC_AttackBoxActive", RpcTarget.All);
       
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
        AudioManager.instance.PlaySound(transform.position, 1, Random.Range(1.0f, 1.3f), 0.4f);
        anim.SetTrigger("isDamage");
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
    [PunRPC]
    void RPC_AttackBoxActive()
    {
        StartCoroutine(AttackBoxActive());
    }
    IEnumerator AttackBoxActive()
    {
        attackBox.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        attackBox.SetActive(false);

    }
}
