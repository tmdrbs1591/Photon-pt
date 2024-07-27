using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold : MonoBehaviour
{
    [SerializeField] public Transform target; // ��尡 ���� Ÿ��
    [SerializeField] private float speed = 3.0f; // �̵� �ӵ�
    public bool isget; // ������ �ִ� ��������
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
        // ������ ���� ���� ���� (XZ ���)
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;

        // ������ ���� ũ�� ���� (���� ��� 1���� 3 ������ ������ ��)
        float randomForceMagnitude = Random.Range(3f, 6f);

        // ������ ����� ũ���� ���޽� �� ����
        rb.AddForce(randomDirection * randomForceMagnitude, ForceMode.Impulse);
    }



    void FollowTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        rb.MovePosition(transform.position + direction * speed * Time.deltaTime);
    }
}
