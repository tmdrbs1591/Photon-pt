using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class OstrichEvent : MonoBehaviourPunCallbacks
{
    [SerializeField] NavMeshAgent agent; // NavMeshAgent ������Ʈ
    [SerializeField] Transform flag; // NavMeshAgent�� ��ǥ ��ġ
    [SerializeField] Animator anim; // Animator ������Ʈ

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // "M" Ű�� ������ �̺�Ʈ ����
        if (Input.GetKeyDown(KeyCode.M))
        {
            photonView.RPC("EventStart", RpcTarget.All);
        }

        anim.SetBool("isRun", agent.velocity.magnitude > 0.1f);
    }

    [PunRPC]
    void EventStart()
    {
        agent.SetDestination(flag.position);
    }
}
