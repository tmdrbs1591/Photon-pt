using System.Collections;
using UnityEngine;
using Photon.Pun;
using Cinemachine;
using UnityEngine.UI;
using TMPro;

public class PlayerCtrl : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private float speed; // �̵� �ӵ�
    [SerializeField] private float rotationSpeed = 10f; // ȸ�� �ӵ��� �����ϴ� ����
    [SerializeField] private float jumpPower = 10f;

    [SerializeField] Transform cameraPos;

    [SerializeField] TMP_Text nickNameText;

    [SerializeField] private GameObject AttackPtc1;
    [SerializeField] private GameObject AttackPtc2;
    [SerializeField] private GameObject AttackPtc3;

    [SerializeField] Vector3 attackBoxSize;
    [SerializeField] Transform attackBoxPos;

    public PhotonView PV;

    float hAxis; // ���� �Է� ��
    float vAxis; // ���� �Է� ��
    bool jumpDown;

    Vector3 moveVec; // �̵� ���� ����

    Animator anim; // �ִϸ����� ������Ʈ

    Rigidbody rigid;

    [Header("Attack")]
    private int maxAttackCount = 3;
    private int curAttackCount = 0;
    private Coroutine attackCoroutine;

    [Header("��Ÿ��")]
    [SerializeField] private float attackCoolTime = 0.5f;
    private float attacklCurTime;

    private void Awake()
    {
        nickNameText.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
        nickNameText.color = PV.IsMine ? Color.green : Color.red;
        if (PV.IsMine)
        {
            var CM = GameObject.Find("CMCamera").GetComponent<CinemachineVirtualCamera>();
            CM.Follow = transform;
            CM.LookAt = null;

            // Cinemachine Transposer ����
            var transposer = CM.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer != null)
            {
                transposer.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
                transposer.m_FollowOffset = new Vector3(0, 5, -6); // ����: ī�޶� �÷��̾� �ڿ� ��ġ
            }
        }
    }

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>(); // �ִϸ����� ������Ʈ ��������
    }

    void Update()
    {
        if (!PV.IsMine) return;

        GetInput();
        Move(); // �̵� �Լ� ȣ��
        Jump();
        Attack();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal"); // ���� �Է� ����
        vAxis = Input.GetAxisRaw("Vertical"); // ���� �Է� ����
        jumpDown = Input.GetButtonDown("Jump");
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized; // �Է� ���͸� ����ȭ�Ͽ� �̵� ���� ���� ����

        transform.position += moveVec * speed * Time.deltaTime; // �̵� �ӵ��� �ð� ������ ���Ͽ� ��ġ ������Ʈ

        anim.SetBool("isWalk", moveVec != Vector3.zero); // �̵� ������ ũ�⿡ ���� �ȴ� �ִϸ��̼� ���� ����

        if (moveVec != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveVec); // ��ǥ ȸ���� �̵� ���� ���ͷ� ����
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime); // ���� ȸ������ ��ǥ ȸ������ �ε巴�� ȸ��
        }
    }

    void Jump()
    {
        if (jumpDown)
        {
            anim.SetTrigger("isJump");
            //rigid.AddForce(Vector3.up * jumpPower,ForceMode.Impulse);
        }
    }

    void Attack()
    {
        if (attacklCurTime <= 0)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                if (photonView.IsMine)
                {
                    photonView.RPC("Damage", RpcTarget.All);
                }

                attacklCurTime = attackCoolTime;
                if (curAttackCount < maxAttackCount)
                {
                    PV.RPC("PlayerAttackAnim", RpcTarget.AllBuffered, curAttackCount); // RPC ȣ��� ��� Ŭ���̾�Ʈ���� ���� �ִϸ��̼� ������ ����ȭ�մϴ�.
                    curAttackCount++;
                }
                else if (curAttackCount >= maxAttackCount)
                {
                    curAttackCount = 0;
                    PV.RPC("PlayerAttackAnim", RpcTarget.AllBuffered, curAttackCount); // RPC ȣ��� ��� Ŭ���̾�Ʈ���� ���� �ִϸ��̼� ������ ����ȭ�մϴ�.
                    curAttackCount++;
                }
            }
        }
        else
        {
            attacklCurTime -= Time.deltaTime;
        }
    }

    [PunRPC]
    public void PlayerAttackAnim(int attackIndex)
    {
        switch (attackIndex)
        {
            case 0:
                anim.SetTrigger("isAttack1");
                StartCoroutine(EffectSetActive(0.5f, AttackPtc1));

                break;
            case 1:
                anim.SetTrigger("isAttack2");
                StartCoroutine(EffectSetActive(0.5f, AttackPtc2));
                break;
            case 2:
                anim.SetTrigger("isAttack3");
                StartCoroutine(EffectSetActive(0.5f, AttackPtc3));
                break;
        }
    }

    private IEnumerator EndAttackCount()
    {
        yield return new WaitForSeconds(5f);
        curAttackCount = 0;
        Debug.Log("���� �ʱ�ȭ");
    }

    IEnumerator EffectSetActive(float time, GameObject effectObject) // ����Ʈ �ڷ�ƾ
    {
        effectObject.SetActive(true);
        yield return new WaitForSeconds(time);
        effectObject.SetActive(false);

        // RPC ȣ��� ����Ʈ Ȱ��ȭ ���¸� ��� Ŭ���̾�Ʈ���� ����ȭ�մϴ�.
        PV.RPC("SyncEffectState", RpcTarget.OthersBuffered, effectObject.name, false);
    }

    [PunRPC]
    private void SyncEffectState(string effectName, bool state)
    {
        GameObject effectObject = null;

        switch (effectName)
        {
            case "AttackPtc1":
                effectObject = AttackPtc1;
                break;
            case "AttackPtc2":
                effectObject = AttackPtc2;
                break;
            case "AttackPtc3":
                effectObject = AttackPtc3;
                break;
        }

        if (effectObject != null)
        {
            effectObject.SetActive(state);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // ����ȭ�� �����Ͱ� ���� ��� ���⿡ �ۼ��մϴ�.
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(attackBoxPos.position, attackBoxSize);
    }


    [PunRPC]
    void Damage() // ������
    {
        Collider[] colliders = Physics.OverlapBox(attackBoxPos.position, attackBoxSize / 2f);

        foreach (Collider collider in colliders)
        {
            if (collider != null && collider.CompareTag("Enemy"))
            {
                PhotonView enemyPhotonView = collider.gameObject.GetComponent<PhotonView>();
                if (enemyPhotonView != null && enemyPhotonView.IsMine)
                {
                    enemyPhotonView.RPC("TakeDamage", RpcTarget.AllBuffered, 1f);
                }
            }
        }
    }


}
