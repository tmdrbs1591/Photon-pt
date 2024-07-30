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
    [SerializeField] private float rotationSpeed = 10f; // ȸ�� �ӵ��� �����ϴ� ����
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

    [Header("��Ÿ��")]
    private float attacklCurTime;
    private float skilllCurTime;
    [SerializeField] private float dashCoolTime = 5f; // �뽬 ��Ÿ�� ����
    private float dashCurTime;

    [Header("UI")]
    [SerializeField] private Image skillFilled; // ��ų ��Ÿ���� ǥ���� �̹���
    [SerializeField] private TMP_Text skillText; // ��ų ��Ÿ���� ǥ���� �ؽ�Ʈ
    [SerializeField] private Image dashFilled; // �뽬 ��Ÿ���� ǥ���� �̹���
    [SerializeField] private TMP_Text dashText; // �뽬 ��Ÿ���� ǥ���� �ؽ�Ʈ

    [SerializeField] private GameObject playerCanvas;
    [SerializeField] private GameObject playerUICanvas;


    [Header("����")]
    [SerializeField] private AudioSource wakkAudioSource;

    private int curAttackCount = 0;
    private int maxAttackCount = 3;


    public PhotonView PV;

    float hAxis; // ���� �Է� ��
    float vAxis; // ���� �Է� ��
    bool jumpDown;
    bool isDash;
    bool isStop;
    bool isSkill;

    Animator anim; // �ִϸ����� ������Ʈ

    private Vector3 networkPosition;
    private Quaternion networkRotation;
    private float interpolationFactor = 30f; // ���� ���

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
            // �ٸ� �÷��̾��� ĵ������ ��Ȱ��ȭ
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

    public override void OnDisconnected(DisconnectCause cause) => print("�������");

    protected void Update()
    {
        if (!PV.IsMine) return;

        GetInput();
        Move();
        Jump();
        Attack();
        Dash();
        Skill();
        UpdateSkillUI(); // ��ų UI ������Ʈ �޼��� ȣ��
        UpdateDashUI(); // 

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeAttackPower(playerStats.attackPower + 1f); // attackPower ���� �Լ� ȣ��
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            GameObject closestEnemy = FindClosestEnemy();
            if (closestEnemy != null)
            {
                Debug.Log("���� ����� ��: " + closestEnemy.name + " ��ġ: " + closestEnemy.transform.position);
            }
            else
            {
                Debug.Log("���� ã�� �� �����ϴ�.");
            }
        }

        if (!PV.IsMine)
        {
            // �ٸ� Ŭ���̾�Ʈ���� �����Ͽ� ��ġ�� ȸ���� ����
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
        if (isStop || isSkill) // �����̳� ��ų �߿� �� �����̰�
            return;

        Vector3 moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        transform.position += moveVec * playerStats.speed * Time.deltaTime;

        anim.SetBool("isWalk", moveVec != Vector3.zero);

        if (moveVec != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveVec);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // �ȴ� �Ҹ��� ��� ���� �ƴϸ� ���
            if (!wakkAudioSource.isPlaying)
            {
                wakkAudioSource.Play();
            }
        }
        else
        {
            // ĳ���Ͱ� ���߸� �ȴ� �Ҹ� ����
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
                Vector3 fireDirection = transform.forward; // ĳ���Ͱ� �ٶ󺸴� ����
                GameObject arrowObj = PhotonNetwork.Instantiate("MageBullet", attackPos.transform.position + new Vector3(0, 0.5f, 0) + fireDirection * 1.5f, Quaternion.LookRotation(fireDirection));
                Arrow arrow = arrowObj.GetComponent<Arrow>();

                if (arrow != null)
                {
                    arrow.SetDirection(fireDirection); // ȭ���� ���� ����
                    arrow._damage = playerStats.attackPower; // ȭ���� �Ŀ� ����
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
                // AttackPtc1�� �̹� Ȱ��ȭ�Ǿ� �ִٸ� AttackPtc2�� Ȱ��ȭ
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
                // �ٸ� ����Ʈ ������Ʈ�鵵 �߰��� �� �ֽ��ϴ�.
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

    // attackPower ���� �����ϰ� �ٸ� Ŭ���̾�Ʈ���� RPC�� ���� �˸��ϴ�.
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
        // ����ȭ�� �����͸� �ۼ��մϴ�.
        if (stream.IsWriting)
        {
            // �����͸� �ٸ� Ŭ���̾�Ʈ���� �����ϴ�.
            stream.SendNext(playerStats.attackPower);
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(playerStats.curHp);
        }
        else
        {
            // �����͸� �ٸ� Ŭ���̾�Ʈ�κ��� �����մϴ�.
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
            damageText.text = damage.ToString(); // ����ȭ�� _attackPower ���� �ؽ�Ʈ�� ����
        }
        Destroy(damageTextObj, 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("AttackBox"))
        {
            if (PV.IsMine)
                PV.RPC("PlayerTakeDamage", RpcTarget.AllBuffered, 1f); // ü�� ���� RPC ȣ��
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
        hpBar.value = playerStats.curHp / playerStats.maxHp; // HP �� ������Ʈ
    }

    void Dash()
    {
        if (dashCurTime <= 0)
        {
            // �뽬 �Է��� �����ϰ�, �뽬�� ���� ó���� �����մϴ�.
            if (Input.GetKeyDown(KeyCode.Z))
            {
                dashCurTime = dashCoolTime;

                anim.SetTrigger("isDash");
                // �뽬 ����Ʈ Ȱ��ȭ RPC ȣ��
                PV.RPC("ActivateDashEffect", RpcTarget.All);

                // �뽬 �ӵ� ����
                playerStats.speed *= 4f;

                // �뽬 ����Ʈ ���ӽð� �Ŀ� �뽬 �ӵ� ���� �� ���� �ʱ�ȭ
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

                StartCoroutine(ObjectSetActive(SkillPanel, 1.8f)); // ��ų �г� Ȱ��ȭ
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

        Vector3 fireDirection = transform.forward; // ĳ���Ͱ� �ٶ󺸴� ����
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < 30; i++)
        {
            // ������ ��ġ ���� ���� (-1���� 1������ ����)
            Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), 0.5f, Random.Range(-1f, 1f));

            // ȭ���� ������ ��ġ�� ����
            GameObject arrowObj = PhotonNetwork.Instantiate("SkillArrow", attackPos.transform.position + randomOffset + fireDirection * 1.5f, Quaternion.LookRotation(fireDirection));
            SkillArrow arrow = arrowObj.GetComponent<SkillArrow>();
            if (arrow != null)
            {
                //  arrow.SetDirection(fireDirection); // ȭ���� ���� ����
                //   arrow._damage = attackPower; // ȭ���� �Ŀ� ����
            }
            CameraShake.instance.Shake();
            anim.SetTrigger("isAttack1");

            // n�� �ڿ� �����ϱ�
            StartCoroutine(DestroyAfterDelay(arrowObj, 0.5f));

            yield return new WaitForSeconds(0.04f);
        }
    }

    IEnumerator DestroyAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);

        // PhotonNetwork.Destroy�� ����Ͽ� ����ȭ�� ������� ����
        if (obj != null && PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Destroy(obj);
        }
    }



    [PunRPC]
    void ActivateSkillEffect() // ��ų ����Ʈ RPC
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

        // �뽬 �ӵ� ����
        playerStats.speed /= 4f;

        // �뽬 ���� �ʱ�ȭ
        isDash = false;
    }

    void UpdateSkillUI()
    {
        // ��ų UI ������Ʈ: ���� ��ų ��Ÿ�ӿ� ���� UI�� fillAmount ����
        if (skillFilled != null)
        {
            skillFilled.fillAmount = Mathf.Clamp01(skilllCurTime / playerStats.skillCoolTime);
            if (skilllCurTime > 0)
            {
                skillText.text = skilllCurTime.ToString("F1"); // �Ҽ��� ù° �ڸ����� ǥ��
            }
            else
            {
                skillText.text = ""; // ��Ÿ���� 0 ������ �� ���� ���
            }
        }
    }

    void UpdateDashUI()
    {
        // �뽬 UI ������Ʈ: ���� �뽬 ��Ÿ�ӿ� ���� UI�� fillAmount ����
        if (dashFilled != null)
        {
            dashFilled.fillAmount = Mathf.Clamp01(dashCurTime / dashCoolTime);
            if (dashCurTime > 0)
            {
                dashText.text = dashCurTime.ToString("F1"); // �Ҽ��� ù° �ڸ����� ǥ��
            }
            else
            {
                dashText.text = ""; // ��Ÿ���� 0 ������ �� ���� ���
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
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); // "Enemy" �±װ� �ִ� ��� ���� ã��
        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position); // �÷��̾�� �� ������ �Ÿ� ���

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }

    // ���� ����� �� �������� ��� ȸ���ϴ� �޼���
    private void RotateToClosestEnemy(GameObject enemy)
    {
        if (enemy != null)
        {
            Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized; // �� ���� ���� ���
            Quaternion targetRotation = Quaternion.LookRotation(directionToEnemy); // �� ���������� ȸ�� ���
            transform.rotation = targetRotation; // ��� ȸ��
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
