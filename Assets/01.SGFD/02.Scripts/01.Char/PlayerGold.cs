using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using TMPro;

public class PlayerGold : MonoBehaviour
{
    void Start()
    {
    }

    void Update()
    {
    }
    private void OnCollisionEnter(Collision collision)
    {
        var GoldCom = collision.gameObject.GetComponent<Gold>();
        if (collision.gameObject.CompareTag("Gold") && GoldCom.isget)
        {
            AudioManager.instance.PlaySound(transform.position, 6, Random.Range(1f, 0.9f), 0.4f);
            var ps = GetComponent<PlayerStats>();
            ps.currentXp += 10;
            ps.LV_UP();
            Destroy(collision.gameObject);
        }
    }
}
