using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f; // �Ѿ��� �ӵ�

    void Start()
    {
        // ������ �̵��ϴ� �������� �ʱ�ȭ
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
    }

    void Update()
    {
        // ���� ���, �Ѿ��� ���� �Ÿ��� �������� �� �ڵ����� ������ �� �ֽ��ϴ�.
        // �� �ڵ�� �����̸�, �ʿ信 ���� �����ϼž� �մϴ�.
        if (transform.position.magnitude > 100f)
        {
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // �ٸ� ��ü�� �浹�� �� ó���� �ڵ带 �ۼ��մϴ�.
        // ���� ���, �浹 �� ���ظ� �����ų� ȿ���� ������ �� �ֽ��ϴ�.
    }
}
