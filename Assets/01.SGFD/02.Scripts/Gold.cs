using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold : MonoBehaviour
{
    [SerializeField] public Transform target; // 골드가 따라갈 타겟
    [SerializeField] private float speed = 3.0f; // 이동 속도
    public bool isget; // 먹을수 있는 상태인지
    Rigidbody rb;

    void Start()
    {
       // Debug.Log(PhotonNetwork.NickName);
        rb = GetComponent<Rigidbody>();
        Jump();
    }

    void Update()
    {
        if (isget && target != null)
        {
            FollowTarget();
        }
        else
        {
            Invoke("Get", 1f);
        }
    }

    void Get()
    {
        isget = true;
    }
    void Jump()
    {
        // 랜덤한 방향 벡터 생성 (XZ 평면)
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;

        // 랜덤한 힘의 크기 설정 (예를 들어 1에서 3 사이의 랜덤한 값)
        float randomForceMagnitude = Random.Range(3f, 6f);

        // 랜덤한 방향과 크기의 임펄스 힘 적용
        rb.AddForce(randomDirection * randomForceMagnitude, ForceMode.Impulse);
    }



    void FollowTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        rb.MovePosition(transform.position + direction * speed * Time.deltaTime);
    }
}
