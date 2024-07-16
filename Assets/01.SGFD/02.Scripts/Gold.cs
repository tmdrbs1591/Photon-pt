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
            Invoke("Get", 0.9f);
        }
    }

    void Get()
    {
        isget = true;
    }

    void Jump()
    {
        rb.AddForce(new Vector3(0, 4, 0), ForceMode.Impulse);
    }

    void FollowTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        rb.MovePosition(transform.position + direction * speed * Time.deltaTime);
    }
}
