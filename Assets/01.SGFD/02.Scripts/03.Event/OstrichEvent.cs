using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class OstrichEvent : MonoBehaviourPunCallbacks
{
    [SerializeField] NavMeshAgent agent; // NavMeshAgent 컴포넌트
    [SerializeField] Transform flagtransform; // NavMeshAgent의 목표 위치
    [SerializeField] Animator anim; // Animator 컴포넌트
    [SerializeField] Transform startTransform;
    [SerializeField] Flag flag;
    [SerializeField] GameObject startingLine;
    [SerializeField] TextAnim Ostrichtextanim;

    [SerializeField] GameObject eventTextPanel;
    [SerializeField] TextAnim textanim;

    void Awake()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // "M" 키가 눌리면 이벤트 시작
        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    photonView.RPC("EventStart", RpcTarget.All);
        //}

        anim.SetBool("isRun", agent.velocity.magnitude > 0.1f);
    }

    public void EventStartTrigger()
    {
        photonView.RPC("EventStart", RpcTarget.All);
    }

    [PunRPC]
    void EventStart()
    {
        StartCoroutine(StartEvent());
    }



    IEnumerator StartEvent()
    {
        eventTextPanel.SetActive(false);
        eventTextPanel.SetActive(true);
        textanim.textToShow = "타조와 함께 달리기 경주를 하고 승리를 쟁취하세요!";

        // 이동을 멈추고 위치를 초기화
        agent.isStopped = true;
        agent.ResetPath(); // 현재 경로를 초기화합니다.
        yield return null; // 프레임을 대기하여 경로 초기화를 완료합니다.

        transform.position = startTransform.position;
        transform.rotation = startTransform.rotation; // 회전도 초기화합니다.
        agent.Warp(startTransform.position); // 정확한 위치로 이동시킵니다.

        startingLine.SetActive(true);
        flag.isClear = false;
        Ostrichtextanim.textToShow = "<shake>나랑 달리기 경주할 사람\r\n어디 없나~<shake>";

        yield return new WaitForSeconds(3); // 대기 시간

        startingLine.SetActive(false);
        agent.isStopped = false;
        agent.SetDestination(flagtransform.position);
    }
}
