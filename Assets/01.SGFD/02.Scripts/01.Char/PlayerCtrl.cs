using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Cinemachine;
using TMPro;

public class PlayerCtrl : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] Rigidbody rigid;
    [SerializeField] private float speed; // �̵� �ӵ�
    [SerializeField] private float rotationSpeed = 10f; // ȸ�� �ӵ��� �����ϴ� ����
    [SerializeField] private float jumpPower = 10f;
    [SerializeField] protected float attackPower = 1f; // ����� attackPower ���� ���� ������� �ʱ� ���� private �ʵ�� ����

    [SerializeField] Transform cameraPos;
    [SerializeField] TMP_Text nickNameText;

    [SerializeField] private GameObject AttackPtc1;
    [SerializeField] private GameObject AttackPtc2;
    [SerializeField] private GameObject AttackPtc3;
    [SerializeField] private GameObject DashPtc;
    [SerializeField] private GameObject SkillPtc;

    [SerializeField] Vector3 attackBoxSize;
    [SerializeField] Transform attackBoxPos;
    [SerializeField] Slider hpBar;

    [SerializeField] private float maxHp;
    [SerializeField] private float curHp;

    [Header("��Ÿ��")]
    [SerializeField] private float attackCoolTime = 0.5f;
    private float attacklCurTime;
    [SerializeField] private float skillCoolTime = 5f; // ��ų ��Ÿ�� ����
    private float skilllCurTime;

    [Header("UI")]
    [SerializeField] private Image skillFilled; // ��ų ��Ÿ���� ǥ���� �̹���
    [SerializeField] private TMP_Text skillText; // ��ų ��Ÿ���� ǥ���� �ؽ�Ʈ
    [SerializeField] private GameObject playerCanvas;

    public PhotonView PV;

    float hAxis; // ���� �Է� ��
    float vAxis; // ���� �Է� ��
    bool jumpDown;
    bool isDash;

    Animator anim; // �ִϸ����� ������Ʈ

    private int maxAttackCount = 3;
    private int curAttackCount = 0;
    private Coroutine attackCoroutine;

    private Vector3 networkPosition;
    private Quaternion networkRotation;
    private float interpolationFactor = 30f; // ���� ���

    protected void Awake()
    {
        curHp = maxHp;
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();


        if (!photonView.IsMine)
        {
            // �ٸ� �÷��̾��� ĵ������ ��Ȱ��ȭ
            playerCanvas.SetActive(false);
        }
        if (PV.IsMine)
        {
            nickNameText.text = PhotonNetwork.NickName;
            nickNameText.color = Color.green;

            var CM = GameObject.Find("CMCamera").GetComponent<CinemachineVirtualCamera>();
            CM.Follow = transform;
            CM.LookAt = null;

            var transposer = CM.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer != null)
            {
                transposer.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
                transposer.m_FollowOffset = new Vector3(0, 5, -6); // ī�޶� ��ġ ���� ����
            }
        }
        else
        {
            nickNameText.text = PV.Owner.NickName;
            nickNameText.color = Color.white;
        }
    }

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
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeAttackPower(attackPower + 1f); // attackPower ���� �Լ� ȣ��
        }
        if (!PV.IsMine)
        {
            // �ٸ� Ŭ���̾�Ʈ���� �����Ͽ� ��ġ�� ȸ���� ����
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * interpolationFactor);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * interpolationFactor);
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
                AudioManager.instance.PlaySound(transform.position, 0, Random.Range(1.2f, 1.2f), 0.4f);
                PV.RPC("Damage", RpcTarget.All);
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
    void Damage()
    {
        Collider[] colliders = Physics.OverlapBox(attackBoxPos.position, attackBoxSize / 2f);
        foreach (Collider collider in colliders)
        {
            if (collider != null && collider.CompareTag("Enemy"))
            {
                PhotonView enemyPhotonView = collider.gameObject.GetComponent<PhotonView>();
                if (enemyPhotonView != null && enemyPhotonView.IsMine)
                {
                    enemyPhotonView.RPC("TakeDamage", RpcTarget.AllBuffered, attackPower);
                    PV.RPC("SpawnDamageText", RpcTarget.AllBuffered, collider.transform.position);
                    Destroy(PhotonNetwork.Instantiate("HitPtc", collider.transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity), 2f);
                }
            }
        }
    }

    // attackPower ���� �����ϰ� �ٸ� Ŭ���̾�Ʈ���� RPC�� ���� �˸��ϴ�.
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
        // ����ȭ�� �����Ͱ� ���� ��� ���⿡ �ۼ��մϴ�.
        if (stream.IsWriting)
        {
            // �����͸� �ٸ� Ŭ���̾�Ʈ���� �����ϴ�.
            stream.SendNext(attackPower);
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(curHp);

        }
        else
        {
            // �����͸� �ٸ� Ŭ���̾�Ʈ�κ��� �����մϴ�.
            attackPower = (float)stream.ReceiveNext();
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
            curHp = (float)stream.ReceiveNext();
        }
    }

    [PunRPC]
    void SpawnDamageText(Vector3 position)
    {
        GameObject damageTextObj = Instantiate(Resources.Load<GameObject>("DamageText"), position + new Vector3(1, 2.5f, 0), Quaternion.identity);
        TMP_Text damageText = damageTextObj.GetComponent<TMP_Text>();
        if (damageText != null)
        {
            damageText.text = attackPower.ToString(); // ����ȭ�� _attackPower ���� �ؽ�Ʈ�� ����
        }
        Destroy(damageTextObj, 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("AttackBox"))
        {
            Debug.Log("ddddddddd");
            if (PV.IsMine)
                PV.RPC("PlayerTakeDamage", RpcTarget.AllBuffered, 1f); // ü�� ���� RPC ȣ��
        }
    }

    [PunRPC]
    void PlayerTakeDamage(float damage)
    {
        curHp -= damage;
        hpBar.value = curHp / maxHp; // HP �� ������Ʈ
    }

    void Dash()
    {
        // �뽬 �Է��� �����ϰ�, �뽬�� ���� ó���� �����մϴ�.
        if (Input.GetKeyDown(KeyCode.Z))
        {
            // �뽬 ����Ʈ Ȱ��ȭ RPC ȣ��
            PV.RPC("ActivateDashEffect", RpcTarget.All);

            // �뽬 �ӵ� ����
            speed *= 4f;

            // �뽬 ����Ʈ ���ӽð� �Ŀ� �뽬 �ӵ� ���� �� ���� �ʱ�ȭ
            StartCoroutine(DashOut());
        }
    }

    void Skill()
    {
        if (skilllCurTime <= 0)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                anim.SetTrigger("isAttack2");
                PV.RPC("ActivateSkillEffect", RpcTarget.All);
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

        for (int i = 0; i < 6; i++)
        {
            PV.RPC("Damage", RpcTarget.All);
            yield return new WaitForSeconds(0.1f);
        }
    }

    [PunRPC]
    void ActivateSkillEffect() // ��ų����Ʈ rpc
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

        // �뽬 �ӵ� ����
        speed /= 4f;

        // �뽬 ���� �ʱ�ȭ
        isDash = false;
    }

    void UpdateSkillUI()
    {
        // ��ų UI ���� ������Ʈ: ���� ��ų ��Ÿ�ӿ� ���� UI�� fillAmount ����
        if (skillFilled != null)
        {
            skillFilled.fillAmount = Mathf.Clamp01(skilllCurTime / skillCoolTime);
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

}