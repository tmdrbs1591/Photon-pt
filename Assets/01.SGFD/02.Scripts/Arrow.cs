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
            var enemyScript = other.gameObject.GetComponent<Enemy>();
            enemyScript.TakeDamage(15);

            PhotonNetwork.Instantiate("HitPtc", other.transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity); //��Ʈ ��ƼŬ


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
