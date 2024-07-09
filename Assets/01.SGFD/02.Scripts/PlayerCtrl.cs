using System.Collections;
using UnityEngine;
using Photon.Pun;
using Cinemachine;
using UnityEngine.UI;
using TMPro;

public class PlayerCtrl : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private float speed; // 이동 속도
    [SerializeField] private float rotationSpeed = 10f; // 회전 속도를 조절하는 변수
    [SerializeField] private float jumpPower = 10f;

    [SerializeField] Transform cameraPos;

    [SerializeField] TMP_Text nickNameText;

    [SerializeField] private GameObject AttackPtc1;
    [SerializeField] private GameObject AttackPtc2;
    [SerializeField] private GameObject AttackPtc3;

    [SerializeField] Vector3 attackBoxSize;
    [SerializeField] Transform attackBoxPos;

    public PhotonView PV;

    float hAxis; // 수평 입력 값
    float vAxis; // 수직 입력 값
    bool jumpDown;

    Vector3 moveVec; // 이동 방향 벡터

    Animator anim; // 애니메이터 컴포넌트

    Rigidbody rigid;

    [Header("Attack")]
    private int maxAttackCount = 3;
    private int curAttackCount = 0;
    private Coroutine attackCoroutine;

    [Header("쿨타임")]
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

            // Cinemachine Transposer 설정
            var transposer = CM.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer != null)
            {
                transposer.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
                transposer.m_FollowOffset = new Vector3(0, 5, -6); // 예시: 카메라를 플레이어 뒤에 배치
            }
        }
    }

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>(); // 애니메이터 컴포넌트 가져오기
    }

    void Update()
    {
        if (!PV.IsMine) return;

        GetInput();
        Move(); // 이동 함수 호출
        Jump();
        Attack();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal"); // 수평 입력 감지
        vAxis = Input.GetAxisRaw("Vertical"); // 수직 입력 감지
        jumpDown = Input.GetButtonDown("Jump");
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized; // 입력 벡터를 정규화하여 이동 방향 벡터 설정

        transform.position += moveVec * speed * Time.deltaTime; // 이동 속도와 시간 간격을 곱하여 위치 업데이트

        anim.SetBool("isWalk", moveVec != Vector3.zero); // 이동 벡터의 크기에 따라 걷는 애니메이션 상태 설정

        if (moveVec != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveVec); // 목표 회전을 이동 방향 벡터로 설정
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime); // 현재 회전에서 목표 회전까지 부드럽게 회전
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
                    PV.RPC("PlayerAttackAnim", RpcTarget.AllBuffered, curAttackCount); // RPC 호출로 모든 클라이언트에게 공격 애니메이션 실행을 동기화합니다.
                    curAttackCount++;
                }
                else if (curAttackCount >= maxAttackCount)
                {
                    curAttackCount = 0;
                    PV.RPC("PlayerAttackAnim", RpcTarget.AllBuffered, curAttackCount); // RPC 호출로 모든 클라이언트에게 공격 애니메이션 실행을 동기화합니다.
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
        Debug.Log("공격 초기화");
    }

    IEnumerator EffectSetActive(float time, GameObject effectObject) // 이펙트 코루틴
    {
        effectObject.SetActive(true);
        yield return new WaitForSeconds(time);
        effectObject.SetActive(false);

        // RPC 호출로 이펙트 활성화 상태를 모든 클라이언트에게 동기화합니다.
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
        // 동기화할 데이터가 있을 경우 여기에 작성합니다.
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(attackBoxPos.position, attackBoxSize);
    }


    [PunRPC]
    void Damage() // 데미지
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
