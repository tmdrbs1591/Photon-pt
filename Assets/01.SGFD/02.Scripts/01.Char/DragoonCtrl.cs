using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Cinemachine;
using TMPro;
using Photon.Realtime;
using System.Net.Http.Headers;

public class DragoonCtrl : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] Rigidbody rigid;
    [SerializeField] private float speed; // 이동 속도
    [SerializeField] private float rotationSpeed = 10f; // 회전 속도를 조절하는 변수
    [SerializeField] private float jumpPower = 10f;
    [SerializeField] public float attackPower = 1f; // 변경된 attackPower 값을 직접 사용하지 않기 위해 private 필드로 변경

    [SerializeField] Transform cameraPos;
    [SerializeField] TMP_Text nickNameText;

    [SerializeField] private GameObject AttackPtc1;
    [SerializeField] private GameObject AttackPtc2;
    [SerializeField] private GameObject AttackPtc3;
    [SerializeField] private GameObject DashPtc;
    [SerializeField] private GameObject SkillPtc;


    [SerializeField] private GameObject SkillPanel;

    [SerializeField] Vector3 attackBoxSize;
    [SerializeField] Transform attackBoxPos;
    [SerializeField] Slider hpBar;

    [SerializeField] private float maxHp;
    [SerializeField] private float curHp;

    [Header("쿨타임")]
    [SerializeField] private float attackCoolTime = 0.5f;
    private float attacklCurTime;
    [SerializeField] private float skillCoolTime = 5f; // 스킬 쿨타임 설정
    private float skilllCurTime;
    [SerializeField] private float dashCoolTime = 5f; // 대수ㅣ 쿨타임 설정
    private float dashCurTime;

    [Header("UI")]
    [SerializeField] private Image skillFilled; // 스킬 쿨타임을 표시할 이미지
    [SerializeField] private TMP_Text skillText; // 스킬 쿨타임을 표시할 텍스트
    [SerializeField] private Image dashFiled; // 스킬 쿨타임을 표시할 이미지
    [SerializeField] private TMP_Text dashText; // 스킬 쿨타임을 표시할 텍스트

    [SerializeField] private GameObject playerCanvas;
    [SerializeField] private Transform skillPos;

    public PhotonView PV;

    float hAxis; // 수평 입력 값
    float vAxis; // 수직 입력 값
    bool jumpDown;
    bool isDash;
    bool isStop;
    bool isSkill;

    Animator anim; // 애니메이터 컴포넌트

    private int maxAttackCount = 3;
    private int curAttackCount = 0;
    private Coroutine attackCoroutine;

    private Vector3 networkPosition;
    private Quaternion networkRotation;
    private float interpolationFactor = 30f; // 보간 계수



    protected void Awake()
    {
        curHp = maxHp;
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();


        if (!photonView.IsMine)
        {
            // 다른 플레이어의 캔버스를 비활성화
            playerCanvas.SetActive(false);
        }
        if (PV.IsMine)
        {
            nickNameText.text = PhotonNetwork.NickName;
            nickNameText.color = Color.green;

            var CM = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
            CM.target = transform;

        }
        else
        {
            nickNameText.text = PV.Owner.NickName;
            nickNameText.color = Color.white;
        }
    }
    public override void OnDisconnected(DisconnectCause cause) => print("연결끊김");

    protected void Update()
    {

        if (!PV.IsMine) return;

        GetInput();
        Move();
        Jump();
        Attack();
        Dash();
        Skill();
        UpdateSkillUI(); // 스킬 UI 업데이트 메서드 호출
        UpdateDashUI(); // 

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeAttackPower(attackPower + 1f); // attackPower 증가 함수 호출
        }
        if (!PV.IsMine)
        {
            // 다른 클라이언트에서 보간하여 위치와 회전을 조정
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 25);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * 25);
        }
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        jumpDown = Input.GetButtonDown("Jump");
    }

    void Move()
    {
        if (isStop || isSkill)//공격이나 스킬중엔 못움직이게
            return;
        Vector3 moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        transform.position += moveVec * speed * Time.deltaTime;

        anim.SetBool("isWalk", moveVec != Vector3.zero);

        if (moveVec != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveVec);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void Jump()
    {
        if (jumpDown)
        {
            anim.SetTrigger("isJump");
        }
    }

    void Attack()
    {
        if (attacklCurTime <= 0)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                StartCoroutine(IsStop(0.2f));
                AudioManager.instance.PlaySound(transform.position, 0, Random.Range(1f, 0.9f), 0.1f);
                AudioManager.instance.PlaySound(transform.position, 5, Random.Range(1f, 0.9f), 0.5f);
                PV.RPC("Damage", RpcTarget.All, attackPower);
                attacklCurTime = attackCoolTime;
                PV.RPC("PlayerAttackAnim", RpcTarget.AllBuffered, curAttackCount);
                curAttackCount = (curAttackCount + 1) % maxAttackCount;
            }
        }
        else
        {
            attacklCurTime -= Time.deltaTime;
        }

    }

    [PunRPC]
    void PlayerAttackAnim(int attackIndex)
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

    IEnumerator EffectSetActive(float time, GameObject effectObject)
    {
        effectObject.SetActive(true);
        yield return new WaitForSeconds(time);
        effectObject.SetActive(false);
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

    [PunRPC]
    void Damage(float damage)
    {
        Collider[] colliders = Physics.OverlapBox(attackBoxPos.position, attackBoxSize / 2f);
        foreach (Collider collider in colliders)
        {
            if (collider != null && collider.CompareTag("Enemy"))
            {
                var enemyScript = collider.gameObject.GetComponent<Enemy>();
                PhotonView enemyPhotonView = collider.gameObject.GetComponent<PhotonView>();
                if (enemyPhotonView != null && enemyPhotonView.IsMine)
                {
                    enemyPhotonView.RPC("TakeDamage", RpcTarget.AllBuffered, damage);
                    enemyScript.playerObj = this.gameObject;
                    PV.RPC("SpawnDamageText", RpcTarget.AllBuffered, collider.transform.position, damage);
                    PhotonNetwork.Instantiate("HitPtc", collider.transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);
                    if (enemyScript.currentHP - damage <= 0)
                    {
                        Debug.Log(PhotonNetwork.NickName);
                    }
                }
            }
        }
    }

    // attackPower 값을 변경하고 다른 클라이언트에게 RPC를 통해 알립니다.
    public void ChangeAttackPower(float newAttackPower)
    {
        Debug.Log("d");
        PV.RPC("SetAttackPower", RpcTarget.AllBuffered, newAttackPower);
    }

    [PunRPC]
    void SetAttackPower(float newAttackPower)
    {
        attackPower = newAttackPower;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 동기화할 데이터가 있을 경우 여기에 작성합니다.
        if (stream.IsWriting)
        {
            // 데이터를 다른 클라이언트에게 보냅니다.
            stream.SendNext(attackPower);
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(curHp);

        }
        else
        {
            // 데이터를 다른 클라이언트로부터 수신합니다.
            attackPower = (float)stream.ReceiveNext();
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
            curHp = (float)stream.ReceiveNext();
        }
    }

    [PunRPC]
    void SpawnDamageText(Vector3 position, float damage)
    {
        GameObject damageTextObj = Instantiate(Resources.Load<GameObject>("DamageText"), position + new Vector3(1, 2.5f, 0), Quaternion.identity);
        TMP_Text damageText = damageTextObj.GetComponent<TMP_Text>();
        if (damageText != null)
        {
            damageText.text = damage.ToString(); // 동기화된 _attackPower 값을 텍스트로 설정
        }
        Destroy(damageTextObj, 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("AttackBox"))
        {
            if (PV.IsMine)
                PV.RPC("PlayerTakeDamage", RpcTarget.AllBuffered, 1f); // 체력 감소 RPC 호출
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag("Portal") && Input.GetKey(KeyCode.C))
        {
            Debug.Log("next room");
            PV.RPC("MoveToNextStage", RpcTarget.All);
        }
    }

    [PunRPC]
    void MoveToNextStage()
    {
        StageManager.instance.NextStage();
    }


    [PunRPC]
    void PlayerTakeDamage(float damage)
    {
        curHp -= damage;
        hpBar.value = curHp / maxHp; // HP 바 업데이트
    }

    void Dash()
    {
        if (dashCurTime <= 0)
        {
            // 대쉬 입력을 감지하고, 대쉬할 때의 처리를 구현합니다.
            if (Input.GetKeyDown(KeyCode.Z))
            {
                dashCurTime = dashCoolTime;

                anim.SetTrigger("isDash");
                // 대쉬 이펙트 활성화 RPC 호출
                PV.RPC("ActivateDashEffect", RpcTarget.All);

                // 대쉬 속도 증가
                speed *= 4f;

                // 대쉬 이펙트 지속시간 후에 대쉬 속도 복구 및 상태 초기화
                StartCoroutine(DashOut());
            }
        }
        else
        {
            dashCurTime -= Time.deltaTime;
        }
    }

    void Skill()
    {
        if (skilllCurTime <= 0)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                AudioManager.instance.PlaySound(transform.position, 3, Random.Range(1f, 1f), 1f);


                StartCoroutine(ObjectSetActive(SkillPanel, 1.8f));// 스킬 패널 활성화
                StartCoroutine(IsSkill(1.3f));
                anim.SetTrigger("isAttack2");
                StartCoroutine(SkillCor());
                skilllCurTime = skillCoolTime;
            }
        }
        else
        {
            skilllCurTime -= Time.deltaTime;
        }
    }

    IEnumerator SkillCor()
    {
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(IsStop(0.2f));
        Vector3 fireDirection = transform.forward; // 캐릭터가 바라보는 방향
        GameObject firePillarObj = PhotonNetwork.Instantiate("FirePillar", skillPos.transform.position + new Vector3(0, 0.5f, 0) + fireDirection * 1.5f, Quaternion.LookRotation(fireDirection));
        FirePillar firePillar = firePillarObj.GetComponent<FirePillar>();

        if (firePillar != null)
        {
            firePillar._damage = attackPower; // 화살의 파워 설정
        }
        AudioManager.instance.PlaySound(transform.position, 4, Random.Range(1f, 0.9f), 0.4f);
        attacklCurTime = attackCoolTime;
        PV.RPC("PlayerAttackAnim", RpcTarget.AllBuffered);


    }

    [PunRPC]
    void ActivateSkillEffect() // 스킬이펙트 rpc
    {
        StartCoroutine(EffectSetActive(1.5f, SkillPtc));
    }


    [PunRPC]
    void ActivateDashEffect()
    {
        StartCoroutine(EffectSetActive(0.3f, DashPtc));
    }

    IEnumerator DashOut()
    {
        yield return new WaitForSeconds(0.12f);

        // 대쉬 속도 복구
        speed /= 4f;

        // 대쉬 상태 초기화
        isDash = false;
    }

    void UpdateSkillUI()
    {
        // 스킬 UI 벨류 업데이트: 남은 스킬 쿨타임에 따라 UI의 fillAmount 설정
        if (skillFilled != null)
        {
            skillFilled.fillAmount = Mathf.Clamp01(skilllCurTime / skillCoolTime);
            if (skilllCurTime > 0)
            {
                skillText.text = skilllCurTime.ToString("F1"); // 소수점 첫째 자리까지 표시
            }
            else
            {
                skillText.text = ""; // 쿨타임이 0 이하일 때 공백 출력
            }
        }
    }
    void UpdateDashUI()
    {
        // 스킬 UI 벨류 업데이트: 남은 스킬 쿨타임에 따라 UI의 fillAmount 설정
        if (dashFiled != null)
        {
            dashFiled.fillAmount = Mathf.Clamp01(dashCurTime / dashCoolTime);
            if (dashCurTime > 0)
            {
                dashText.text = dashCurTime.ToString("F1"); // 소수점 첫째 자리까지 표시
            }
            else
            {
                dashText.text = ""; // 쿨타임이 0 이하일 때 공백 출력
            }
        }
    }
    IEnumerator IsStop(float time)
    {
        isStop = true;
        yield return new WaitForSeconds(time);
        isStop = false;

    }
    IEnumerator IsSkill(float time)
    {
        isSkill = true;
        yield return new WaitForSeconds(time);
        isSkill = false;

    }
    IEnumerator ObjectSetActive(GameObject go, float time)
    {
        go.SetActive(true);
        yield return new WaitForSeconds(time);
        go.SetActive(false);

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(attackBoxPos.position, attackBoxSize);
    }

}