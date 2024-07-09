using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // �÷��̾� Transform�� �Ҵ��մϴ�.
    public Vector3 offset = new Vector3(0, 5, -10); // �⺻ ������ ���� �����մϴ�.
    public float smoothTime = 0.3f; // �ε巴�� �̵��ϱ� ���� �ð��Դϴ�.

    private Vector3 velocity = Vector3.zero; // ���� �ӵ�

    void LateUpdate()
    {
        if (target != null)
        {
            // ��ǥ ��ġ�� ��� (�÷��̾� ��ġ + ������)
            Vector3 desiredPosition = target.position + offset;

            // �ε巴�� �̵��ϱ� ���� SmoothDamp �Լ� ���
            Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);

            // ī�޶� ��ġ�� ������Ʈ
            transform.position = smoothedPosition;
        }
    }
    private void Update()
    {
        // �÷��̾ �±׷� ã�� �Ҵ��մϴ�.
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
    }
    void Start()
    {
   
        // ������ ���۵� �� �ʱ� �������� �����մϴ�.
        if (target != null)
        {
            offset = transform.position - target.position;
        }
    }
}
