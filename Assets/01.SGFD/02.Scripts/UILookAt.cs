using UnityEngine;

public class UILookAt : MonoBehaviour
{
    private Transform mainCameraTransform; // 메인 카메라의 Transform을 저장할 변수

    void Start()
    {
        // 메인 카메라를 찾습니다.
        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found. Make sure it is tagged as 'MainCamera'.");
            return;
        }

        // 메인 카메라의 Transform을 가져옵니다.
        mainCameraTransform = mainCamera.transform;
    }

    void LateUpdate()
    {
        if (mainCameraTransform == null)
        {
            return; // 메인 카메라가 설정되지 않은 경우 업데이트를 중단합니다.
        }

        // 메인 카메라의 위치와 UI 요소의 위치 차이를 구합니다.
        Vector3 direction = mainCameraTransform.position - transform.position;

        // 방향 벡터를 기반으로 회전을 계산합니다.
        Quaternion lookRotation = Quaternion.LookRotation(-direction, Vector3.up);

        // UI 요소를 회전시킵니다.
        transform.rotation = lookRotation;
    }
}
