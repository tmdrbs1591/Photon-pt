using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance; // �̱��� �ν��Ͻ�

    private Camera mainCamera; // ��鸱 ���� ī�޶�

    private void Awake()
    {
        if (instance == null)
        {
            instance = this; // �ν��Ͻ� ����
        }
        else
        {
            Destroy(gameObject); // �̹� �ν��Ͻ��� �����ϸ� ���� ��ü�� �ı�
            return;
        }

        mainCamera = Camera.main; // "MainCamera" �±װ� ���� ī�޶� �ڵ����� ã��
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found. Please tag the main camera as 'MainCamera'.");
        }
    }

    [SerializeField]
    [Range(0.1f, 0.5f)]
    private float shakeRange = 0.5f; // ��鸲 ����

    [SerializeField]
    [Range(0.1f, 1f)]
    private float duration = 0.1f; // ��鸲 ���� �ð�

    // ��鸲 �޼���
    public void Shake()
    {
        if (mainCamera != null)
        {
            StopAllCoroutines(); // �ٸ� ��鸲�� ���� ���̸� ����
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
        Vector3 originalCameraPos = mainCamera.transform.position; // ���� ī�޶� ��ġ ����

        while (elapsed < duration)
        {
            float cameraPosX = Random.value * shakeRange * 2 - shakeRange;
            float cameraPosY = Random.value * shakeRange * 2 - shakeRange;

            Vector3 newCameraPos = originalCameraPos;
            newCameraPos.x += cameraPosX;
            newCameraPos.y += cameraPosY;

            mainCamera.transform.position = newCameraPos; // ī�޶� ��ġ ����

            elapsed += Time.deltaTime;

            yield return null;
        }

        mainCamera.transform.position = originalCameraPos; // �ʱ� ��ġ�� ����
    }

    // �� �� �޼���
    public void ZoomIn(float zoomFOV, float duration)
    {
        if (mainCamera != null)
        {
            float startFOV = mainCamera.fieldOfView;
            float targetFOV = zoomFOV;
            StartCoroutine(ZoomCoroutine(startFOV, targetFOV, duration));
        }
    }

    public void ZoomOut(float defaultFOV, float duration)
    {
        if (mainCamera != null)
        {
            float startFOV = mainCamera.fieldOfView;
            float targetFOV = defaultFOV;
            StartCoroutine(ZoomCoroutine(startFOV, targetFOV, duration));
        }
    }

    private IEnumerator ZoomCoroutine(float startFOV, float targetFOV, float duration)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float currentFOV = Mathf.Lerp(startFOV, targetFOV, elapsed / duration);
            mainCamera.fieldOfView = currentFOV;

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.fieldOfView = targetFOV; // ���� FOV ���� ��Ȯ�� ����
    }



}
