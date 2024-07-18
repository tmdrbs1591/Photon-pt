using Photon.Pun;
using UnityEngine;
using TMPro;
using System.Collections;

public class Arrow : MonoBehaviourPunCallbacks
{
    [SerializeField] private float speed = 10f; // 화살의 속도
    public PhotonView PV;
    private Rigidbody rb;
    public float _damage = 15f; // 데미지 값
    public ArcherCtrl archerctrl;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 화살에 속도를 부여하여 앞으로 나가게 함
        rb.velocity = transform.forward * speed;

        // PhotonView 컴포넌트 할당
        PV = GetComponent<PhotonView>();

        StartCoroutine(DestroyArrowDelayed());
    }

    // Update is called once per frame
    void Update()
    {
        // 화살이 계속 앞으로 나가도록 함
        rb.velocity = transform.forward * speed;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            var enemyPhotonView = collision.gameObject.GetComponent<PhotonView>();
            var enemyScript = collision.gameObject.GetComponent<Enemy>();
            if (enemyPhotonView != null && enemyPhotonView.IsMine)
            {
                enemyScript.playerObj = archerctrl.gameObject;
                // 데미지 동기화 RPC 호출
                enemyPhotonView.RPC("TakeDamage", RpcTarget.AllBuffered, _damage);

                // 히트 파티클 생성
                PhotonNetwork.Instantiate("HitPtc", collision.transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);

                // 데미지 텍스트 생성 RPC 호출
                if (PV != null)
                {
                    PV.RPC("SpawnDamageText", RpcTarget.AllBuffered, collision.transform.position, _damage);
                }
                else
                {
                    Debug.LogError("PhotonView is null on Arrow.");
                }

                Debug.Log("Hit the enemy!");
             //  PhotonNetwork.Destroy(gameObject);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
     
    }

    // 화살의 방향을 플레이어의 이동 방향으로 설정하는 메서드
    public void SetDirection(Vector3 direction)
    {
        transform.forward = direction.normalized;
    }

    [PunRPC]
    void SpawnDamageText(Vector3 position, float damage)
    {
        GameObject damageTextObj = Instantiate(Resources.Load<GameObject>("DamageText"), position + new Vector3(1, 2.5f, 0), Quaternion.identity);
        TMP_Text damageText = damageTextObj.GetComponent<TMP_Text>();
        if (damageText != null)
        {
            damageText.text = damage.ToString();
        }
       // Destroy(damageTextObj, 2f);
    }
    private IEnumerator DestroyArrowDelayed()
    {
        yield return new WaitForSeconds(0.5f);

        if (PV != null)
        {
            PV.RPC("DestroyArrow", RpcTarget.AllBuffered);
        }
        else
        {
            Debug.LogError("PhotonView is null on Arrow.");
        }
    }

    [PunRPC]
    void DestroyArrow()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
