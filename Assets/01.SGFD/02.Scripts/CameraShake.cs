using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance; // 싱글톤 인스턴스

    public Camera mainCamera; // 흔들릴 메인 카메라
    private Vector3 originalCameraPos; // 카메라 초기 위치 저장 변수

    private void Awake()
    {
        if (instance == null)
        {
            instance = this; // 인스턴스 설정
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 현재 객체를 파괴
            return;
        }

        mainCamera = Camera.main; // "MainCamera" 태그가 붙은 카메라를 자동으로 찾음
        if (mainCamera != null)
        {
            originalCameraPos = mainCamera.transform.position; // 초기 위치 저장
        }
        else
        {
            Debug.LogError("Main camera not found. Please tag the main camera as 'MainCamera'.");
        }
    }

    [SerializeField]
    [Range(0.1f, 0.5f)]
    private float shakeRange = 0.5f; // 흔들림 범위

    [SerializeField]
    [Range(0.1f, 1f)]
    private float duration = 0.1f; // 흔들림 지속 시간

    public void Shake()
    {
        if (mainCamera != null)
        {
            Debug.Log("ddsadsas");
            StopAllCoroutines(); // 다른 흔들림이 실행 중이면 멈춤
            StartCoroutine(ShakeCoroutine());
        }
        else
        {
            Debug.LogError("Main camera is not assigned.");
        }
    }

    private IEnumerator ShakeCoroutine()
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float CameraPosX = Random.value * shakeRange * 2 - shakeRange;
            float CameraPosY = Random.value * shakeRange * 2 - shakeRange;

            Vector3 newCameraPos = originalCameraPos;
            newCameraPos.x += CameraPosX;
            newCameraPos.y += CameraPosY;

            mainCamera.transform.position = newCameraPos; // 카메라 위치 변경

            elapsed += Time.deltaTime;

            yield return null;
        }

        mainCamera.transform.position = originalCameraPos; // 초기 위치로 복구
    }
}
