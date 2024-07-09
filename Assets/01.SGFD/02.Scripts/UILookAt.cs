using UnityEngine;
using Cinemachine;

public class UILookAt : MonoBehaviour
{
    public string cinemachineTag = "CinemachineCamera"; // Cinemachine ī�޶��� �±׸� �����մϴ�.
    private Transform cinemachineTransform; // Cinemachine ī�޶��� Transform�� ������ ����

    void Start()
    {
        // �±׸� ����Ͽ� Cinemachine ī�޶� ã���ϴ�.
        GameObject cinemachineCameraObject = GameObject.FindGameObjectWithTag(cinemachineTag);

        if (cinemachineCameraObject == null)
        {
            Debug.LogError("Cinemachine camera not found with tag '" + cinemachineTag + "'. Make sure it is tagged correctly.");
            return;
        }

        // Cinemachine ī�޶��� Transform�� �����ɴϴ�.
        cinemachineTransform = cinemachineCameraObject.transform;
    }

    void LateUpdate()
    {
        // Cinemachine ī�޶��� ��ġ�� UI ����� ��ġ ���̸� ���մϴ�.
        Vector3 direction = cinemachineTransform.position - transform.position;

        // ���� ���͸� ������� ȸ���� ����մϴ�.
        Quaternion lookRotation = Quaternion.LookRotation(-direction, Vector3.up);

        // UI ��Ҹ� ȸ����ŵ�ϴ�.
        transform.rotation = lookRotation;
    }
}
