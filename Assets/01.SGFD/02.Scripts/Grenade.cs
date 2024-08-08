using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Grenade : MonoBehaviourPunCallbacks
{

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GrenadeBoom());
    }


    IEnumerator GrenadeBoom()
    {
        yield return new WaitForSeconds(2f);
        PhotonNetwork.Instantiate("GrenadeBoom", transform.position, Quaternion.identity);
        PhotonNetwork.Destroy(gameObject);

    }
}

