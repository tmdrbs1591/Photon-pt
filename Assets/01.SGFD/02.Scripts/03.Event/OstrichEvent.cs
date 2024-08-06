using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class OstrichEvent : MonoBehaviourPunCallbacks
{
    [SerializeField] NavMeshAgent agent; // NavMeshAgent ������Ʈ
    [SerializeField] Transform flagtransform; // NavMeshAgent�� ��ǥ ��ġ
    [SerializeField] Animator anim; // Animator ������Ʈ
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
        // "M" Ű�� ������ �̺�Ʈ ����
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
        textanim.textToShow = "Ÿ���� �Բ� �޸��� ���ָ� �ϰ� �¸��� �����ϼ���!";

        // �̵��� ���߰� ��ġ�� �ʱ�ȭ
        agent.isStopped = true;
        agent.ResetPath(); // ���� ��θ� �ʱ�ȭ�մϴ�.
        yield return null; // �������� ����Ͽ� ��� �ʱ�ȭ�� �Ϸ��մϴ�.

        transform.position = startTransform.position;
        transform.rotation = startTransform.rotation; // ȸ���� �ʱ�ȭ�մϴ�.
        agent.Warp(startTransform.position); // ��Ȯ�� ��ġ�� �̵���ŵ�ϴ�.

        startingLine.SetActive(true);
        flag.isClear = false;
        Ostrichtextanim.textToShow = "<shake>���� �޸��� ������ ���\r\n��� ����~<shake>";

        yield return new WaitForSeconds(3); // ��� �ð�

        startingLine.SetActive(false);
        agent.isStopped = false;
        agent.SetDestination(flagtransform.position);
    }
}
