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
    [SerializeField] private float speed; // �̵� �ӵ�
    [SerializeField] private float rotationSpeed = 10f; // ȸ�� �ӵ��� �����ϴ� ����
    [SerializeField] private float jumpPower = 10f;
    [SerializeField] public float attackPower = 1f; // ����� attackPower ���� ���� ������� �ʱ� ���� private �ʵ�� ����

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

    [Header("��Ÿ��")]
    [SerializeField] private float attackCoolTime = 0.5f;
    private float attacklCurTime;
    [SerializeField] private float skillCoolTime = 5f; // ��ų ��Ÿ�� ����
    private float skilllCurTime;
    [SerializeField] private float dashCoolTime = 5f; // ����� ��Ÿ�� ����
    private float dashCurTime;

    [Header("UI")]
    [SerializeField] private Image skillFilled; // ��ų ��Ÿ���� ǥ���� �̹���
    [SerializeField] private TMP_Text skillText; // ��ų ��Ÿ���� ǥ���� �ؽ�Ʈ
    [SerializeField] private Image dashFiled; // ��ų ��Ÿ���� ǥ���� �̹���
    [SerializeField] private TMP_Text dashText; // ��ų ��Ÿ���� ǥ���� �ؽ�Ʈ

    [SerializeField] private GameObject playerCanvas;
    [SerializeField] private Transform skillPos;

    public PhotonView PV;

    float hAxis; // ���� �Է� ��
    float vAxis; // ���� �Է� ��
    bool jumpDown;
    bool isDash;
    bool isStop;
    bool isSkill;

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
        if (isStop || isSkill)//�����̳� ��ų�߿� �������̰�
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


                StartCoroutine(ObjectSetActive(SkillPanel, 1.8f));// ��ų �г� Ȱ��ȭ
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
        Vector3 fireDirection = transform.forward; // ĳ���Ͱ� �ٶ󺸴� ����
        GameObject firePillarObj = PhotonNetwork.Instantiate("FirePillar", skillPos.transform.position + new Vector3(0, 0.5f, 0) + fireDirection * 1.5f, Quaternion.LookRotation(fireDirection));
        FirePillar firePillar = firePillarObj.GetComponent<FirePillar>();

        if (firePillar != null)
        {
            firePillar._damage = attackPower; // ȭ���� �Ŀ� ����
        }
        AudioManager.instance.PlaySound(transform.position, 4, Random.Range(1f, 0.9f), 0.4f);
        attacklCurTime = attackCoolTime;
        PV.RPC("PlayerAttackAnim", RpcTarget.AllBuffered);


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
    void UpdateDashUI()
    {
        // ��ų UI ���� ������Ʈ: ���� ��ų ��Ÿ�ӿ� ���� UI�� fillAmount ����
        if (dashFiled != null)
        {
            dashFiled.fillAmount = Mathf.Clamp01(dashCurTime / dashCoolTime);
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
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(attackBoxPos.position, attackBoxSize);
    }

}