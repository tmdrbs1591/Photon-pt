using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShop : MonoBehaviour
{
    private ShopUI shopUI; // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    public bool isShop;    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    private PlayerCtrl playerCtrl;
    private ArcherCtrl archerCtrl;

    private void Awake()
    {
        playerCtrl = GetComponent<PlayerCtrl>();
        archerCtrl = GetComponent<ArcherCtrl>();
        shopUI = GetComponentInChildren<ShopUI>(true);      // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && isShop) // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        {
            ToggleShop();
        }
    }

    public void OnIncreaseStatButton(string statType, float amount)
    {
        switch (statType)
        {
            case "attackPower":
                if (playerCtrl != null)
                {
                    playerCtrl.attackPower += amount;
                }
                else
                {
                    Debug.LogWarning("playerCtrl is null.");
                }

                if (archerCtrl != null)
                {
                    archerCtrl.attackPower += amount;
                }
                else
                {
                    Debug.LogWarning("archerCtrl is null.");
                }
                break;

            default:
                Debug.LogError("Unknown stat type: " + statType);
                break;
        }
    }

    public void ToggleShop() // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    {
        if (shopUI != null)
        {
            shopUI.ToggleShop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Shop"))
        {
            isShop = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Shop"))
        {
            isShop = false;
        }
    }
}
