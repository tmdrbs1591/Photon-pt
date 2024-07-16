using UnityEngine;
using Photon.Pun;
using TMPro;

public class Shop : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)
        {
            PlayerCtrl playerCtrl = other.GetComponent<PlayerCtrl>();
            //playerCtrl.isShop = true;
            Debug.Log("À×");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)
        {
            PlayerCtrl playerCtrl = other.GetComponent<PlayerCtrl>();
            //playerCtrl.isShop = false;
        }
    }
}