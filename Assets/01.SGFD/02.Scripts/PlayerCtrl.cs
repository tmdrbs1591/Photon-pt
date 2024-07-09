using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class PlayerCtrl : MonoBehaviour
{
    [SerializeField] private float speed; // �̵� �ӵ�
    [SerializeField] private float rotationSpeed = 10f; // ȸ�� �ӵ��� �����ϴ� ����
    [SerializeField] private float jumpPower = 10f;

    [SerializeField] Transform cameraPos;

    [SerializeField] TMP_Text nickNameText;

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
        GetInput();
        if (PV.IsMine)
        {
            Move(); // �̵� �Լ� ȣ��
            Jump();
            Attack();
        }
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
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (curAttackCount < maxAttackCount)
            {
                PlayerAttackAnim();
                curAttackCount++;

                if (attackCoroutine != null)
                {
                    StopCoroutine(attackCoroutine);
                }

                attackCoroutine = StartCoroutine(EndAttackCount());
                Debug.Log("���� ���� Ƚ�� : " + curAttackCount);
            }
            else if (curAttackCount >= maxAttackCount)
            {
                curAttackCount = 0;
                PlayerAttackAnim();
                curAttackCount++;

                if (attackCoroutine != null)
                {
                    StopCoroutine(attackCoroutine);
                }

                attackCoroutine = StartCoroutine(EndAttackCount());
                Debug.Log("���� ���� Ƚ�� : " + curAttackCount);
            }
        }
    }

    void PlayerAttackAnim()
    {
        switch (curAttackCount)
        {
            case 0:
                anim.SetTrigger("isAttack1");
                break;
            case 1:
                anim.SetTrigger("isAttack2");
                break;
            case 2:
                anim.SetTrigger("isAttack3");
                break;
        }
    }

    private IEnumerator EndAttackCount()
    {
        yield return new WaitForSeconds(5f);
        curAttackCount = 0;
        Debug.Log("���� �ʱ�ȭ");
    }
}
