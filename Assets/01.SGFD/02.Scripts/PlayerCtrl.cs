using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerCtrl : MonoBehaviour
{
    [SerializeField] private float speed; // 이동 속도
    [SerializeField] private float rotationSpeed = 10f; // 회전 속도를 조절하는 변수
    [SerializeField] private float jumpPower = 10f;

    [SerializeField] Transform cameraPos;

    public PhotonView PV;

    float hAxis; // 수평 입력 값
    float vAxis; // 수직 입력 값

    bool jumpDown;

    Vector3 moveVec; // 이동 방향 벡터

    Animator anim; // 애니메이터 컴포넌트

    Rigidbody rigid;

    private void Awake()
    {
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
        GetInput();
        if (PV.IsMine)
        {
            Move(); // 이동 함수 호출
            Jump();
            Attack();
        }
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
        if (Input.GetKeyDown(KeyCode.X))
        {
            anim.SetTrigger("isAttack1");
        }
    }
}
