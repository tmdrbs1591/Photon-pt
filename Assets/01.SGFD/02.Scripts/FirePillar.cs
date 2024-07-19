using Photon.Pun;
using UnityEngine;
using TMPro;
using System.Collections;

public class FirePillar : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    private Rigidbody rb;
    public float _damage = 15f; // 데미지 값

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // PhotonView 컴포넌트 할당
        PV = GetComponent<PhotonView>();

        if (PV == null)
        {
            Debug.LogError("PhotonView component is missing from the FirePillar.");
            return;
        }

        //StartCoroutine(DestroyFirePillarDelayed());
    }

    // Update is called once per frame
    void Update()
    {
        // 화살이 계속 앞으로 나가도록 하는 코드가 여기에 올 수 있습니다.
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 충돌 시의 처리가 필요하다면 이곳에 코드를 추가합니다.
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            StartCoroutine(DamageCor(other));
            // DestroyArrowDelayed() 메서드는 필요하지 않은 경우 주석 처리
        }
    }

    IEnumerator DamageCor(Collider _other)
    {
        var enemyPhotonView = _other.gameObject.GetComponent<PhotonView>();
        if (enemyPhotonView != null && PV.IsMine)
        {
            for (int i = 0; i < 15; i++)
            {
                enemyPhotonView.RPC("TakeDamage", RpcTarget.AllBuffered, _damage);

                // 데미지 텍스트 생성 RPC 호출
                PV.RPC("SpawnDamageText", RpcTarget.AllBuffered, _other.transform.position, _damage);

                Debug.Log("Hit the enemy!");

                yield return new WaitForSeconds(0.1f);
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
        // Destroy(damageTextObj, 2f); // 주석 처리: 필요 시 활성화
    }

    private IEnumerator DestroyFirePillarDelayed()
    {
        yield return new WaitForSeconds(2.5f);

        // 모든 클라이언트에서 FirePillar를 제거하도록 RPC 호출
        PV.RPC("DestroyFirePillar", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void DestroyFirePillar()
    {
        // 모든 클라이언트에서 FirePillar를 제거합니다.
        if (PhotonNetwork.IsMasterClient || PV.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
