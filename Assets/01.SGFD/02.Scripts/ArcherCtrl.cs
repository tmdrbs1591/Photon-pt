using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class ArcherCtrl : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] Rigidbody rigid;
    [SerializeField] private float speed; // �̵� �ӵ�
    [SerializeField] private float rotationSpeed = 10f; // ȸ�� �ӵ��� �����ϴ� ����
    [SerializeField] private float jumpPower = 10f;
    [SerializeField] public float attackPower = 1f; // ����� attackPower ���� ���� ������� �ʱ� ���� private �ʵ�� ����

    [SerializeField] Transform cameraPos;
    [SerializeField] TMP_Text nickNameText;

    [SerializeField] private GameObject AttackPtc1;
    [SerializeField] private GameObject AttackPtc2;
    [SerializeField] private GameObject DashPtc;
    [SerializeField] private GameObject SkillPtc;

    [SerializeField] private GameObject SkillPanel;

    [SerializeField] Slider hpBar;

    [SerializeField] private float maxHp;
    [SerializeField] private float curHp;

    [Header("��Ÿ��")]
    [SerializeField] private float attackCoolTime = 0.5f;
    private float attacklCurTime;
    [SerializeField] private float skillCoolTime = 5f; // ��ų ��Ÿ�� ����
    private float skilllCurTime;
    [SerializeField] private float dashCoolTime = 5f; // �뽬 ��Ÿ�� ����
    private float dashCurTime;

    [Header("UI")]
    [SerializeField] private Image skillFilled; // ��ų ��Ÿ���� ǥ���� �̹���
    [SerializeField] private TMP_Text skillText; // ��ų ��Ÿ���� ǥ���� �ؽ�Ʈ
    [SerializeField] private Image dashFilled; // �뽬 ��Ÿ���� ǥ���� �̹���
    [SerializeField] private TMP_Text dashText; // �뽬 ��Ÿ���� ǥ���� �ؽ�Ʈ

    [SerializeField] private GameObject playerCanvas;

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
            ChangeAttackPower(attackPower + 1f); // attackPower ���� �Լ� ȣ��
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
        if (isStop || isSkill) // �����̳� ��ų �߿� �������� �ʰ� ��
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
                Vector3 fireDirection = transform.forward; // ĳ���Ͱ� �ٶ󺸴� ����
                GameObject arrowObj = PhotonNetwork.Instantiate("Arrow", transform.position  + fireDirection * 1.5f, Quaternion.LookRotation(fireDirection));
                Arrow arrow = arrowObj.GetComponent<Arrow>();
                if (arrow != null)
                {
                    arrow.SetDirection(fireDirection); // ȭ���� ���� ����
                    arrow._damage = attackPower; // ȭ���� ���� ����
                }
                AudioManager.instance.PlaySound(transform.position, 4, Random.Range(1f, 0.9f), 0.4f);
                PV.RPC("Damage", RpcTarget.All, attackPower);
                attacklCurTime = attackCoolTime;
                PV.RPC("PlayerAttackAnim", RpcTarget.AllBuffered);
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
    void PlayerAttackAnim()
    {
        anim.SetTrigger("isAttack1");
        StartCoroutine(EffectSetActive(0.5f, AttackPtc1));
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
        attackPower = newAttackPower;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // ����ȭ�� �����͸� �ۼ��մϴ�.
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
        curHp -= damage;
        hpBar.value = curHp / maxHp; // HP �� ������Ʈ
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
                speed *= 4f;

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
        CameraShake.instance.ZoomIn(10f, 1f); // �� ��

        for (int i = 0; i < 6; i++)
        {
            // PV.RPC("Damage", RpcTarget.All, attackPower + 1f);
            AudioManager.instance.PlaySound(transform.position, 2, Random.Range(1.1f, 1.4f), 0.2f);
            yield return new WaitForSeconds(0.1f);
            CameraShake.instance.Shake();
        }
        yield return new WaitForSeconds(0.37f);
        AudioManager.instance.PlaySound(transform.position, 2, Random.Range(1.2f, 1.2f), 0.2f);
        CameraShake.instance.Shake();
        PV.RPC("Damage", RpcTarget.All, attackPower + 10f);

        CameraShake.instance.ZoomOut(57.4f, 0.2f); // �� �ƿ�
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
        yield return new WaitForSeconds(0.12f);

        // �뽬 �ӵ� ����
        speed /= 4f;

        // �뽬 ���� �ʱ�ȭ
        isDash = false;
    }

    void UpdateSkillUI()
    {
        // ��ų UI ������Ʈ: ���� ��ų ��Ÿ�ӿ� ���� UI�� fillAmount ����
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
}
