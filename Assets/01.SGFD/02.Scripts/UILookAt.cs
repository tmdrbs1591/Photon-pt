using UnityEngine;
using Cinemachine;

public class UILookAt : MonoBehaviour
{
    public string cinemachineTag = "CinemachineCamera"; // Cinemachine 카메라의 태그를 지정합니다.
    private Transform cinemachineTransform; // Cinemachine 카메라의 Transform을 저장할 변수

    void Start()
    {
        // 태그를 사용하여 Cinemachine 카메라를 찾습니다.
        GameObject cinemachineCameraObject = GameObject.FindGameObjectWithTag(cinemachineTag);

        if (cinemachineCameraObject == null)
        {
            Debug.LogError("Cinemachine camera not found with tag '" + cinemachineTag + "'. Make sure it is tagged correctly.");
            return;
        }

        // Cinemachine 카메라의 Transform을 가져옵니다.
        cinemachineTransform = cinemachineCameraObject.transform;
    }

    void LateUpdate()
    {
        // Cinemachine 카메라의 위치와 UI 요소의 위치 차이를 구합니다.
        Vector3 direction = cinemachineTransform.position - transform.position;

        // 방향 벡터를 기반으로 회전을 계산합니다.
        Quaternion lookRotation = Quaternion.LookRotation(-direction, Vector3.up);

        // UI 요소를 회전시킵니다.
        transform.rotation = lookRotation;
    }
}
