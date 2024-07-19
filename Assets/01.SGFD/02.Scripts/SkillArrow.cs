using Photon.Pun;
using UnityEngine;
using TMPro;
using System.Collections;

public class SkillArrow : MonoBehaviourPunCallbacks
{
    [SerializeField] private float speed = 10f; // 화살의 속도
    public PhotonView PV;
    private Rigidbody rb;
    public float _damage = 15f; // 데미지 값

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 화살에 속도를 부여하여 앞으로 나가게 함
        rb.velocity = transform.forward * speed;

        // PhotonView 컴포넌트 할당
        PV = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        // 화살이 계속 앞으로 나가도록 함
        rb.velocity = transform.forward * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 충돌 처리 (현재는 비워 두었습니다)
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            var enemyPhotonView = other.gameObject.GetComponent<PhotonView>();

            // 소유자만 처리하도록
            if (PV.IsMine && enemyPhotonView != null)
            {
                // 데미지 동기화 RPC 호출
                enemyPhotonView.RPC("TakeDamage", RpcTarget.AllBuffered, _damage);

                // 히트 파티클 생성
                PhotonNetwork.Instantiate("HitPtc", other.transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);

                // 데미지 텍스트 생성 RPC 호출
                PV.RPC("SpawnDamageText", RpcTarget.AllBuffered, other.transform.position, _damage);

                Debug.Log("Hit the enemy!");

                // 화살 파괴 RPC 호출 (1초 뒤에 파괴되도록)
                StartCoroutine(DestroyArrowDelayed());
            }
        }
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
        yield return new WaitForSeconds(1f); // 1초 대기 후 화살 파괴

        if (PV != null && PV.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            Debug.LogError("PhotonView is null or not owned by this client.");
        }
    }
}
