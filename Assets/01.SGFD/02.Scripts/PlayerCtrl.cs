using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerCtrl : MonoBehaviour
{
    [SerializeField] private float speed; // �̵� �ӵ�
    [SerializeField] private float rotationSpeed = 10f; // ȸ�� �ӵ��� �����ϴ� ����
    [SerializeField] private float jumpPower = 10f;

    [SerializeField] Transform cameraPos;

    public PhotonView PV;

    float hAxis; // ���� �Է� ��
    float vAxis; // ���� �Է� ��

    bool jumpDown;

    Vector3 moveVec; // �̵� ���� ����

    Animator anim; // �ִϸ����� ������Ʈ

    Rigidbody rigid;

    private void Awake()
    {
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
            anim.SetTrigger("isAttack1");
        }
    }
}
