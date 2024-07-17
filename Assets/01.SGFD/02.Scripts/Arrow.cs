using Photon.Pun;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed = 10f; // ȭ���� �ӵ�
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // ȭ�쿡 �ӵ��� �ο��Ͽ� ������ ������ ��
        rb.velocity = transform.forward * speed;
    }

    // Update is called once per frame
    void Update()
    {
        // ȭ���� ��� ������ �������� ��
        rb.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("ssssssssssssssssssss");
            Destroy(gameObject); // �浹 �� ȭ���� �ı�
        }
    }

    // ȭ���� ������ �÷��̾��� �̵� �������� �����ϴ� �޼���
    public void SetDirection(Vector3 direction)
    {
        transform.forward = direction.normalized;
    }
}
