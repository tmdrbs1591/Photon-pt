using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreCharacter : MonoBehaviour
{
    [Header("Ui")]
    public GameObject shopUI;

    [Header("PlayerCtrl")]
    private PlayerCtrl currentPlayer;

    private void Update()
    {
        if (currentPlayer != null && Input.GetKeyDown(KeyCode.E))
        {
            ToggleShop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            currentPlayer = other.GetComponent<PlayerCtrl>();

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            currentPlayer = null;
        }
    }

    private void ToggleShop()
    {
        if (shopUI.activeSelf)
        {
            CloseShop();
        }
        else
        {
            OpenShop();
        }
    }

    private void OpenShop()
    {
        shopUI.SetActive(true);
    }

    private void CloseShop()
    {
        shopUI.SetActive(false);
    }
}
