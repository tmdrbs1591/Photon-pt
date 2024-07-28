using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class OstrichEvent : MonoBehaviourPunCallbacks
{
    [SerializeField] NavMeshAgent agent; // NavMeshAgent 컴포넌트
    [SerializeField] Transform flag; // NavMeshAgent의 목표 위치
    [SerializeField] Animator anim; // Animator 컴포넌트

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // "M" 키가 눌리면 이벤트 시작
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
