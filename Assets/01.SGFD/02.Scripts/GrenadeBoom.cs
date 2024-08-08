using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class GrenadeBoom : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    public float _damage = 15f; // ������ ��

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();

    }

    // Update is called once per frame
    void Update()
    {
        
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
        if (enemyPhotonView != null)
        {
       
                enemyPhotonView.RPC("TakeDamage", RpcTarget.AllBuffered, _damage);

                // ������ �ؽ�Ʈ ���� RPC ȣ��
                PV.RPC("SpawnDamageText", RpcTarget.AllBuffered, _other.transform.position, _damage);

                Debug.Log("Hit the enemy!");

                yield return new WaitForSeconds(0.1f);
            
        }
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
}
