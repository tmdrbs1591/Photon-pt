using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.Analytics;

public class MageCtrl : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] Rigidbody rigid;
    [SerializeField] private float rotationSpeed = 10f; // 회전 속도를 조절하는 변수
    [SerializeField] private float jumpPower = 10f;

    [SerializeField] Transform cameraPos;
    [SerializeField] TMP_Text nickNameText;

    [SerializeField] private GameObject AttackPtc1;
    [SerializeField] private GameObject AttackPtc2;
    [SerializeField] private GameObject AttackPtc3;
    [SerializeField] private GameObject DashPtc;
    [SerializeField] private GameObject SkillPtc;

    [SerializeField] private GameObject SkillPanel;

    [SerializeField] Slider hpBar;


    [SerializeField] private Transform attackPos;

    [Header("쿨타임")]
    private float attacklCurTime;
    private float skilllCurTime;
    [SerializeField] private float dashCoolTime = 5f; // 대쉬 쿨타임 설정
    private float dashCurTime;

    [Header("UI")]
    [SerializeField] private Image skillFilled; // 스킬 쿨타임을 표시할 이미지
    [SerializeField] private TMP_Text skillText; // 스킬 쿨타임을 표시할 텍스트
    [SerializeField] private Image dashFilled; // 대쉬 쿨타임을 표시할 이미지
    [SerializeField] private TMP_Text dashText; // 대쉬 쿨타임을 표시할 텍스트

    [SerializeField] private GameObject playerCanvas;
    [SerializeField] private GameObject playerUICanvas;


    [Header("사운드")]
    [SerializeField] private AudioSource wakkAudioSource;

    private int curAttackCount = 0;
    private int maxAttackCount = 3;


    public PhotonView PV;

    float hAxis; // 수평 입력 값
    float vAxis; // 수직 입력 값
    bool jumpDown;
    bool isDash;
    bool isStop;
    bool isSkill;

    Animator anim; // 애니메이터 컴포넌트

    private Vector3 networkPosition;
    private Quaternion networkRotation;
    private float interpolationFactor = 30f; // 보간 계수

    public bool isShop = false;


    public PlayerStats playerStats;


    protected void Awake()
    {
        wakkAudioSource = GetComponent<AudioSource>();
        playerStats = GetComponent<PlayerStats>();
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        playerStats.curHp = playerStats.maxHp;

        if (!photonView.IsMine)
        {
            // 다른 플레이어의 캔버스를 비활성화
            playerCanvas.SetActive(false);
            playerUICanvas.SetActive(false);
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
            ChangeAttackPower(playerStats.attackPower + 1f); // attackPower 증가 함수 호출
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            GameObject closestEnemy = FindClosestEnemy();
            if (closestEnemy != null)
            {
                Debug.Log("가장 가까운 적: " + closestEnemy.name + " 위치: " + closestEnemy.transform.position);
            }
            else
            {
                Debug.Log("적을 찾을 수 없습니다.");
            }
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
        if (isStop || isSkill) // 공격이나 스킬 중엔 못 움직이게
            return;

        Vector3 moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        transform.position += moveVec * playerStats.speed * Time.deltaTime;

        anim.SetBool("isWalk", moveVec != Vector3.zero);

        if (moveVec != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveVec);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // 걷는 소리가 재생 중이 아니면 재생
            if (!wakkAudioSource.isPlaying)
            {
                wakkAudioSource.Play();
            }
        }
        else
        {
            // 캐릭터가 멈추면 걷는 소리 중지
            if (wakkAudioSource.isPlaying)
            {
                wakkAudioSource.Stop();
            }
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
                GameObject closestEnemy = FindClosestEnemy();
                if (closestEnemy != null)
                {
                    PV.RPC("RPC_RotateToClosestEnemy", RpcTarget.All, closestEnemy.transform.position);
                }

                StartCoroutine(IsStop(0.2f));
                Vector3 fireDirection = transform.forward; // 캐릭터가 바라보는 방향
                GameObject arrowObj = PhotonNetwork.Instantiate("MageBullet", attackPos.transform.position + new Vector3(0, 0.5f, 0) + fireDirection * 1.5f, Quaternion.LookRotation(fireDirection));
                Arrow arrow = arrowObj.GetComponent<Arrow>();

                if (arrow != null)
                {
                    arrow.SetDirection(fireDirection); // 화살의 방향 설정
                    arrow._damage = playerStats.attackPower; // 화살의 파워 설정
                    arrow.archerctrl = gameObject.GetComponent<ArcherCtrl>();
                }
                AudioManager.instance.PlaySound(transform.position, 10, Random.Range(1f, 0.9f), 0.4f);
                PV.RPC("Damage", RpcTarget.All, playerStats.attackPower);
                attacklCurTime = playerStats.attackCoolTime;
                PV.RPC("PlayerAttackAnim", RpcTarget.AllBuffered, curAttackCount);
                curAttackCount = (curAttackCount + 1) % maxAttackCount;
            }
        }
        else
        {
            attacklCurTime -= Time.deltaTime;
        }
    }

    void ArrowFire()
    {

    }
    [PunRPC]
    void PlayerAttackAnim(int attackIndex)
    {
        switch (attackIndex)
        {
            case 0:
                anim.SetTrigger("isAttack1");
                Debug.Log("asdddddd");
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
        if (effectObject.activeSelf)
        {
            if (effectObject == AttackPtc1)
            {
                // AttackPtc1이 이미 활성화되어 있다면 AttackPtc2를 활성화
                AttackPtc2.SetActive(true);
                yield return new WaitForSeconds(time);
                AttackPtc2.SetActive(false);
                PV.RPC("SyncEffectState", RpcTarget.OthersBuffered, AttackPtc2.name, false);
            }
        }
        else
        {
            effectObject.SetActive(true);
            yield return new WaitForSeconds(time);
            effectObject.SetActive(false);
            PV.RPC("SyncEffectState", RpcTarget.OthersBuffered, effectObject.name, false);
        }
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
                // 다른 이펙트 오브젝트들도 추가할 수 있습니다.
        }
        if (effectObject != null)
        {
            effectObject.SetActive(state);
        }
    }



    [PunRPC]
    void Damage(float damage)
    {
    }

    // attackPower 값을 변경하고 다른 클라이언트에게 RPC를 통해 알립니다.
    public void ChangeAttackPower(float newAttackPower)
    {
        PV.RPC("SetAttackPower", RpcTarget.AllBuffered, newAttackPower);
    }

    [PunRPC]
    void SetAttackPower(float newAttackPower)
    {
        playerStats.attackPower = newAttackPower;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 동기화할 데이터를 작성합니다.
        if (stream.IsWriting)
        {
            // 데이터를 다른 클라이언트에게 보냅니다.
            stream.SendNext(playerStats.attackPower);
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(playerStats.curHp);
        }
        else
        {
            // 데이터를 다른 클라이언트로부터 수신합니다.
            playerStats.attackPower = (float)stream.ReceiveNext();
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
            playerStats.curHp = (float)stream.ReceiveNext();
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
        playerStats.curHp -= damage;
        hpBar.value = playerStats.curHp / playerStats.maxHp; // HP 바 업데이트
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
                playerStats.speed *= 4f;

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

                StartCoroutine(ObjectSetActive(SkillPanel, 1.8f)); // 스킬 패널 활성화
                StartCoroutine(IsSkill(1.3f));
                anim.SetTrigger("isAttack1");
                // PV.RPC("ActivateSkillEffect", RpcTarget.All);
                StartCoroutine(SkillCor());
                skilllCurTime = playerStats.skillCoolTime;
            }
        }
        else
        {
            skilllCurTime -= Time.deltaTime;
        }
    }

    IEnumerator SkillCor()
    {
        GameObject closestEnemy = FindClosestEnemy();
        if (closestEnemy != null)
        {
            PV.RPC("RPC_RotateToClosestEnemy", RpcTarget.All, closestEnemy.transform.position);
        }

        Vector3 fireDirection = transform.forward; // 캐릭터가 바라보는 방향
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < 30; i++)
        {
            // 랜덤한 위치 벡터 생성 (-1부터 1까지의 범위)
            Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), 0.5f, Random.Range(-1f, 1f));

            // 화살을 랜덤한 위치에 생성
            GameObject arrowObj = PhotonNetwork.Instantiate("SkillArrow", attackPos.transform.position + randomOffset + fireDirection * 1.5f, Quaternion.LookRotation(fireDirection));
            SkillArrow arrow = arrowObj.GetComponent<SkillArrow>();
            if (arrow != null)
            {
                //  arrow.SetDirection(fireDirection); // 화살의 방향 설정
                //   arrow._damage = attackPower; // 화살의 파워 설정
            }
            CameraShake.instance.Shake();
            anim.SetTrigger("isAttack1");

            // n초 뒤에 삭제하기
            StartCoroutine(DestroyAfterDelay(arrowObj, 0.5f));

            yield return new WaitForSeconds(0.04f);
        }
    }

    IEnumerator DestroyAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);

        // PhotonNetwork.Destroy를 사용하여 동기화된 방식으로 삭제
        if (obj != null && PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Destroy(obj);
        }
    }



    [PunRPC]
    void ActivateSkillEffect() // 스킬 이펙트 RPC
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
        yield return new WaitForSeconds(0.08f);

        // 대쉬 속도 복구
        playerStats.speed /= 4f;

        // 대쉬 상태 초기화
        isDash = false;
    }

    void UpdateSkillUI()
    {
        // 스킬 UI 업데이트: 남은 스킬 쿨타임에 따라 UI의 fillAmount 설정
        if (skillFilled != null)
        {
            skillFilled.fillAmount = Mathf.Clamp01(skilllCurTime / playerStats.skillCoolTime);
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
        // 대쉬 UI 업데이트: 남은 대쉬 쿨타임에 따라 UI의 fillAmount 설정
        if (dashFilled != null)
        {
            dashFilled.fillAmount = Mathf.Clamp01(dashCurTime / dashCoolTime);
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
    private GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); // "Enemy" 태그가 있는 모든 적을 찾음
        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position); // 플레이어와 적 사이의 거리 계산

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }

    // 가장 가까운 적 방향으로 즉시 회전하는 메서드
    private void RotateToClosestEnemy(GameObject enemy)
    {
        if (enemy != null)
        {
            Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized; // 적 방향 벡터 계산
            Quaternion targetRotation = Quaternion.LookRotation(directionToEnemy); // 적 방향으로의 회전 계산
            transform.rotation = targetRotation; // 즉시 회전
        }
    }

    [PunRPC]
    private void RPC_RotateToClosestEnemy(Vector3 enemyPosition)
    {
        Vector3 directionToEnemy = (enemyPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToEnemy);
        transform.rotation = targetRotation;
    }

}
