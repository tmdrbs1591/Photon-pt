using Photon.Pun;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed = 10f; // 화살의 속도
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 화살에 속도를 부여하여 앞으로 나가게 함
        rb.velocity = transform.forward * speed;
    }

    // Update is called once per frame
    void Update()
    {
        // 화살이 계속 앞으로 나가도록 함
        rb.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            var enemyScript = other.gameObject.GetComponent<Enemy>();
            enemyScript.TakeDamage(15);

            PhotonNetwork.Instantiate("HitPtc", other.transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity); //히트 파티클


            Debug.Log("ssssssssssssssssssss");
            Destroy(gameObject); // 충돌 후 화살을 파괴
        }
    }

    // 화살의 방향을 플레이어의 이동 방향으로 설정하는 메서드
    public void SetDirection(Vector3 direction)
    {
        transform.forward = direction.normalized;
    }
}
