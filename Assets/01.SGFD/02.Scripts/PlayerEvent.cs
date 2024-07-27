using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerEvent : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private Vector3 eventBoxSize; // 이벤트 발생 박스
    [SerializeField] private Transform eventBoxPos;

    public bool isHold; // 현재 도토리를 잡고 있는지
    private GameObject heldObject; // 현재 잡고 있는 오브젝트
    private int nextChildIndex = 0; // 다음에 활성화할 바구니의 자식 오브젝트의 인덱스

    private void Update()
    {
        AcornHold();
        AcornHoldOut();
    }

    private void AcornHold()
    {
        Collider[] colliders = Physics.OverlapBox(eventBoxPos.position, eventBoxSize / 2f);
        foreach (Collider collider in colliders)
        {
            if (collider != null)
            {
                PhotonView pv = collider.gameObject.GetComponent<PhotonView>();
                if (pv == null)
                {
                    Debug.LogWarning($"PhotonView not found on object: {collider.gameObject.name}");
                    continue;
                }

                if (collider.CompareTag("Acorn"))
                {
                    if (Input.GetKeyDown(KeyCode.C) && !isHold)
                    {
                        isHold = true;
                        heldObject = collider.gameObject;
                        photonView.RPC("SyncAcornPickup", RpcTarget.AllBuffered, pv.ViewID);
                        Debug.Log("잡았다!");
                    }
                }
                else if (collider.CompareTag("Basket"))
                {
                    if (Input.GetKeyDown(KeyCode.C) && isHold)
                    {
                        isHold = false;
                        photonView.RPC("SyncAcornDrop", RpcTarget.AllBuffered, heldObject.GetComponent<PhotonView>().ViewID, pv.ViewID);
                        heldObject = null;
                        Debug.Log("바구니에서 도토리를 놓았다!");
                    }
                }
            }
        }

        if (isHold && heldObject != null)
        {
            heldObject.transform.position = transform.position + Vector3.up * 2.5f;
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }
    }

    private void AcornHoldOut()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (isHold)
            {
                if (heldObject != null)
                {
                    Rigidbody rb = heldObject.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.isKinematic = false;
                    }
                }
                isHold = false;
                photonView.RPC("SyncAcornDrop", RpcTarget.AllBuffered, heldObject?.GetComponent<PhotonView>().ViewID ?? -1, -1);
                heldObject = null;
                Debug.Log("놓았다!");
            }
        }
    }

    [PunRPC]
    private void SyncAcornPickup(int viewID)
    {
        GameObject acorn = PhotonView.Find(viewID)?.gameObject;
        if (acorn != null)
        {
            isHold = true;
            heldObject = acorn;
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }
    }

    [PunRPC]
    private void SyncAcornDrop(int acornViewID, int basketViewID)
    {
        GameObject acorn = PhotonView.Find(acornViewID)?.gameObject;
        if (acorn != null)
        {
            Rigidbody rb = acorn.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }
            Destroy(acorn);
        }
        if (basketViewID != -1)
        {
            GameObject basket = PhotonView.Find(basketViewID)?.gameObject;
            if (basket != null)
            {
                ActivateNextChild(basket);
            }
        }
        isHold = false;
    }

    private void ActivateNextChild(GameObject basket)
    {
        Transform[] children = basket.GetComponentsInChildren<Transform>(true);

        while (nextChildIndex < children.Length)
        {
            Transform nextChild = children[nextChildIndex];
            if (nextChild != basket.transform && !nextChild.gameObject.activeSelf)
            {
                nextChild.gameObject.SetActive(true);
                photonView.RPC("SyncActivateNextChild", RpcTarget.OthersBuffered, nextChild.gameObject.GetComponent<PhotonView>().ViewID);
                nextChildIndex++;
                return;
            }
            else
            {
                nextChildIndex++;
            }
        }

        if (nextChildIndex >= children.Length)
        {
            nextChildIndex = 0;
        }
    }

    [PunRPC]
    private void SyncActivateNextChild(int viewID)
    {
        GameObject child = PhotonView.Find(viewID)?.gameObject;
        if (child != null)
        {
            child.SetActive(true);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(eventBoxPos.position, eventBoxSize);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isHold);
            stream.SendNext(nextChildIndex);
            if (heldObject != null)
            {
                stream.SendNext(heldObject.GetComponent<PhotonView>().ViewID);
            }
            else
            {
                stream.SendNext(-1);
            }
        }
        else
        {
            isHold = (bool)stream.ReceiveNext();
            nextChildIndex = (int)stream.ReceiveNext();
            int heldObjectViewID = (int)stream.ReceiveNext();
            if (heldObjectViewID != -1)
            {
                heldObject = PhotonView.Find(heldObjectViewID)?.gameObject;
                Rigidbody rb = heldObject?.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                }
            }
            else
            {
                heldObject = null;
            }
        }
    }
}
