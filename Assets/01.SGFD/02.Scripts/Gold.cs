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
        rb = GetComponent<Rigidbody>();
        Jump();
    }

    void Update()
    {
        Invoke("Get", 0.9f);
    }
    void Get()
    {
        isget = true;
    }
    void Jump()
    {
        float randomJumpForce = Random.Range(4f, 4f);
        Vector2 jumpVelocity = new Vector3(Random.Range(1f, 1f), randomJumpForce);
        rb.AddForce(jumpVelocity, ForceMode.Impulse);
    }
}
