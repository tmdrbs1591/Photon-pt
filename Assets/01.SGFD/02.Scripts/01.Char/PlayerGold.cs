using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using TMPro;

public class PlayerGold : MonoBehaviour
{
    [SerializeField] private float gold; // �÷��̾ ���� �������ִ� ���
    [SerializeField] TMP_Text goldtext;

    void Start()
    {
    }

    void Update()
    {
        goldtext.text = gold.ToString();
    }
    private void OnCollisionEnter(Collision collision)
    {
        var GoldCom = collision.gameObject.GetComponent<Gold>();
        if (collision.gameObject.CompareTag("Gold") && GoldCom.isget)
        {
            Destroy(collision.gameObject);
            gold++;
        }
    }
}
