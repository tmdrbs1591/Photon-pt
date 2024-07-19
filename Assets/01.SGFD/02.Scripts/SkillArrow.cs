using Photon.Pun;
using UnityEngine;
using TMPro;

public class SkillArrow : MonoBehaviourPunCallbacks
{
    [SerializeField] private float speed = 10f; // ȭ���� �ӵ�
    public PhotonView PV;
    private Rigidbody rb;
    public float _damage = 15f; // ������ ��


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // ȭ�쿡 �ӵ��� �ο��Ͽ� ������ ������ ��
        rb.velocity = transform.forward * speed;

        // PhotonView ������Ʈ �Ҵ�
        PV = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        // ȭ���� ��� ������ �������� ��
        rb.velocity = transform.forward * speed;
    }
    private void OnCollisionEnter(Collision collision)
    {
   
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            var enemyPhotonView = other.gameObject.GetComponent<PhotonView>();
            //&& enemyPhotonView.IsMine
            if (enemyPhotonView != null )
            {
                // ������ ����ȭ RPC ȣ��
                enemyPhotonView.RPC("TakeDamage", RpcTarget.AllBuffered, _damage);

                // ��Ʈ ��ƼŬ ����
                PhotonNetwork.Instantiate("HitPtc", other.transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);

                // ������ �ؽ�Ʈ ���� RPC ȣ��
                if (PV != null)
                {
                    PV.RPC("SpawnDamageText", RpcTarget.AllBuffered, other.transform.position, _damage);
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
}
