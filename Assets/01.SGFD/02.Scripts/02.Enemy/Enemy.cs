using Cinemachine;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Collections;

public class Enemy : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] string Enemytype;
    [SerializeField] public float currentHP;
    [SerializeField] float maxHP;
    [SerializeField] float attackRange;
    [SerializeField] float attackDamage;
    [SerializeField] float maxAttackSpeed;
    [SerializeField] float curAttackSpeed;
    [SerializeField] Slider hpBar;
    [SerializeField] Slider hpBar2;
    [SerializeField] GameObject attackBox;
    [SerializeField] GameObject gold; // �׾����� ������ ���
    [SerializeField] float goldCount;//����

    [SerializeField] LayerMask layerMask;
    [SerializeField] GameObject DangerMarker;
    [SerializeField] GameObject bulletPrefab;

    [SerializeField] int songIndex;

    private float playerDistance;
    private float syncInterval = 0.1f; // ����ȭ ���� (��)
    private float lastSyncTime;

    Animator anim;
    public GameObject playerObj;
    private PhotonView PV;
    NavMeshAgent agent;

    

    void Start()
    {

        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        currentHP = maxHP;
        if (hpBar != null || hpBar != null)
        {
            hpBar.gameObject.SetActive(false);
            hpBar2.gameObject.SetActive(false);
        }

    }

    void Update()
    {
        if (playerObj == null)
        {
            // �߽� ��ġ�� �������� �����Ͽ� ������ ��Ŭ ���
            Collider[] colliders = Physics.OverlapSphere(transform.position, 20f);
            // ������ ��Ŭ���� �÷��̾� �±׸� ���� ������Ʈ�� ã��
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    playerObj = collider.gameObject;
                    break; // �÷��̾ ã�����Ƿ� ����
                }
            }
        }

        if (playerObj != null)
        {
            Vector3 moveVec = (playerObj.transform.position - transform.position).normalized;
            anim.SetBool("isWalk", moveVec != Vector3.zero);

            if (Enemytype != "Box")
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveVec); // ��ǥ ȸ���� �̵� ���� ���ͷ� ����
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 3 * Time.deltaTime); // ���� ȸ������ ��ǥ ȸ������ �ε巴�� ȸ��

                agent.SetDestination(playerObj.transform.position); // �÷��̾� �������� �̵�

            }
            playerDistance = Vector3.Distance(transform.position, playerObj.transform.position); // �Ÿ� ���

            if (curAttackSpeed < 0 && playerDistance < attackRange)
            {
                curAttackSpeed = maxAttackSpeed;
                anim.SetTrigger("isAttack");
                Debug.Log("�÷��̾ ������");
            }
            else
            {
                curAttackSpeed -= Time.deltaTime;
            }

            // ����ȭ ���ݿ� ���� ��ġ ����ȭ
            if (photonView.IsMine && Time.time - lastSyncTime > syncInterval && Enemytype != "Box")
            {
                photonView.RPC("SyncEnemyPosition", RpcTarget.Others, transform.position);
                lastSyncTime = Time.time;
            }
        }

        if (Enemytype != "Box")
        {
            // NavMeshAgent�� �������� �����ߴ��� Ȯ��
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                anim.SetBool("isWalk", false);
            }
            else
            {
                anim.SetBool("isWalk", true);
            }
        }

        if (hpBar != null || hpBar2 != null)
        {
            hpBar.value = Mathf.Lerp(hpBar.value, currentHP / maxHP, Time.deltaTime * 40f);
            hpBar2.value = Mathf.Lerp(hpBar2.value, currentHP / maxHP, Time.deltaTime * 5f);
        }
        Die();
    }

    void Attack()
    {
        photonView.RPC("RPC_AttackBoxActive", RpcTarget.All);
    }

    void Die()
    {
        if (currentHP <= 0)
        {
            for (int i = 0; i < goldCount; i++)
            {
                GameObject enemygold = Instantiate(gold, transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);
                Gold goldComponent = enemygold.GetComponent<Gold>();
                goldComponent.isget = false;

                // ���� �÷��̾� ������Ʈ�� Ÿ������ ����
                goldComponent.target = playerObj.transform;
            }

            Destroy(gameObject);
        }
    }


    [PunRPC]
    void SyncEnemyPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        if (damage > 0)
        {
            PhotonNetwork.Instantiate("debris", transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);

            if (hpBar != null || hpBar2 != null)
            {
                hpBar.gameObject.SetActive(true);
                hpBar2.gameObject.SetActive(true);
            }

            CameraShake.instance.Shake();
            AudioManager.instance.PlaySound(transform.position, songIndex, Random.Range(1.0f, 1.3f), 0.4f);
            anim.SetTrigger("isDamage");
            currentHP -= damage;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentHP);
        }
        else
        {
            currentHP = (float)stream.ReceiveNext();
        }
    }

    [PunRPC]
    void RPC_AttackBoxActive()
    {
        StartCoroutine(AttackBoxActive());
    }

    public void DangerMarkerShoot()
    {
        StartCoroutine(SpellStart());
    }

    IEnumerator SpellStart()
    {
        if (photonView.IsMine)
        {
            float angleStep = 360f / 36;

            for (int i = 0; i < 36; i++)
            {
                Quaternion rotation = Quaternion.Euler(0f, angleStep * i, 0f); // �Ѿ��� ȸ�� ���� ���

                GameObject bullet = Instantiate(bulletPrefab, transform.position, rotation); // �Ѿ� ����
                Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();

                // �Ѿ��� ����� �ӵ� ����
                Vector3 shootDirection = bullet.transform.forward;
                bulletRigidbody.velocity = shootDirection * 10;

                Destroy(bullet, 2f);
                // �Ѿ��� �߰����� ���� (��: ������ ���� ��)

                if (PhotonNetwork.IsConnected)
                {
                    // PUN�� ����Ͽ� �ٸ� �÷��̾�� �Ѿ� �߻� ������ ����ȭ�մϴ�.
                    photonView.RPC("SyncSpellStart", RpcTarget.Others, transform.position, rotation);
                }

                yield return new WaitForSeconds(0.2f); // �߻� ���� ��ٸ�
            }
        }
    }
    [PunRPC]
    void SyncSpellStart(Vector3 firePosition, Quaternion fireRotation)
    {
        // �ٸ� �÷��̾�Լ� ���� ��ġ�� ȸ������ �Ѿ� ����
        GameObject bullet = Instantiate(bulletPrefab, firePosition, fireRotation);
        Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
        Vector3 shootDirection = bullet.transform.forward;
        bulletRigidbody.velocity = shootDirection * 10;

        // �߰����� ���� (��: ������ ���� ��)
    }
    IEnumerator AttackBoxActive()
    {
        attackBox.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        attackBox.SetActive(false);
    }
}
