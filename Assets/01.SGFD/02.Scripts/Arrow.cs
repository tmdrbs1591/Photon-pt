using Photon.Pun;
using UnityEngine;
using TMPro;
using System.Collections;

public class Arrow : MonoBehaviourPunCallbacks
{
    [SerializeField] private float speed = 10f; // ȭ���� �ӵ�
    public PhotonView PV;
    private Rigidbody rb;
    public float _damage = 15f; // ������ ��
    public ArcherCtrl archerctrl;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // ȭ�쿡 �ӵ��� �ο��Ͽ� ������ ������ ��
        rb.velocity = transform.forward * speed;

        // PhotonView ������Ʈ �Ҵ�
        PV = GetComponent<PhotonView>();

       // StartCoroutine(DestroyArrowDelayed());
    }

    // Update is called once per frame
    void Update()
    {
        // ȭ���� ��� ������ �������� ��
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
                // ������ ����ȭ RPC ȣ��
                enemyPhotonView.RPC("TakeDamage", RpcTarget.AllBuffered, _damage);

                // ��Ʈ ��ƼŬ ����
                PhotonNetwork.Instantiate("HitPtc", collision.transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);

                // ������ �ؽ�Ʈ ���� RPC ȣ��
                if (PV != null)
                {
                    PV.RPC("SpawnDamageText", RpcTarget.AllBuffered, collision.transform.position, _damage);
                }
                else
                {
                    Debug.LogError("PhotonView is null on Arrow.");
                }

                Debug.Log("Hit the enemy!");
                // ȭ�� �ı� RPC ȣ�� (1�� �ڿ� �ı��ǵ���)
                StartCoroutine(DestroyArrowDelayed());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ʈ���� �浹 ó�� (�ʿ信 ���� ����)
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
        // Destroy(damageTextObj, 2f);
    }

    private IEnumerator DestroyArrowDelayed()
    {
        yield return new WaitForSeconds(0.4f);

        if (PV != null)
        {
            // �����ڰ� �ƴϸ� �������� �����ɴϴ�.
            if (!PV.IsMine && !PhotonNetwork.IsMasterClient)
            {
                PV.RequestOwnership();
            }

            PV.RPC("DestroyArrow", RpcTarget.MasterClient);
        }
        else
        {
            Debug.LogError("PhotonView is null on Arrow.");
        }
    }

    [PunRPC]
    void DestroyArrow()
    {
        if (PV.IsMine || PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
