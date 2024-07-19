using Photon.Pun;
using UnityEngine;
using TMPro;
using System.Collections;

public class FirePillar : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    private Rigidbody rb;
    public float _damage = 15f; // ������ ��

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // PhotonView ������Ʈ �Ҵ�
        PV = GetComponent<PhotonView>();

        if (PV == null)
        {
            Debug.LogError("PhotonView component is missing from the FirePillar.");
            return;
        }

        StartCoroutine(DestroyFirePillarDelayed());
    }

    // Update is called once per frame
    void Update()
    {
        // ȭ���� ��� ������ �������� �ϴ� �ڵ尡 ���⿡ �� �� �ֽ��ϴ�.
    }

    private void OnCollisionEnter(Collision collision)
    {
        // �浹 ���� ó���� �ʿ��ϴٸ� �̰��� �ڵ带 �߰��մϴ�.
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            StartCoroutine(DamageCor(other));
            // DestroyArrowDelayed() �޼���� �ʿ����� ���� ��� �ּ� ó��
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

                // ������ �ؽ�Ʈ ���� RPC ȣ��
                PV.RPC("SpawnDamageText", RpcTarget.AllBuffered, _other.transform.position, _damage);

                Debug.Log("Hit the enemy!");

                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    // ȭ���� ������ �÷��̾��� �̵� �������� �����ϴ� �޼���
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
        // Destroy(damageTextObj, 2f); // �ּ� ó��: �ʿ� �� Ȱ��ȭ
    }

    private IEnumerator DestroyFirePillarDelayed()
    {
        yield return new WaitForSeconds(2.5f);

        if (PV != null)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // MasterClient�� ��ü�� �����մϴ�.
                PhotonNetwork.Destroy(gameObject);
            }
            else
            {
                // MasterClient�� �ƴ� Ŭ���̾�Ʈ�� �������� ��û�մϴ�.
                if (PV.IsMine)
                {
                    PV.RPC("DestroyFirePillar", RpcTarget.MasterClient);
                }
                else
                {
                    PV.RequestOwnership();
                    yield return new WaitUntil(() => PV.IsMine || PhotonNetwork.IsMasterClient);
                    if (PV.IsMine || PhotonNetwork.IsMasterClient)
                    {
                        PhotonNetwork.Destroy(gameObject);
                    }
                }
            }
        }
        else
        {
            Debug.LogError("PhotonView is null on FirePillar.");
        }
    }

    [PunRPC]
    void DestroyFirePillar()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
