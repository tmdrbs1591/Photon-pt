using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // 플레이어 Transform을 할당합니다.
    public Vector3 offset = new Vector3(0, 5, -10); // 기본 오프셋 값을 설정합니다.
    public float smoothTime = 0.3f; // 부드럽게 이동하기 위한 시간입니다.

    private Vector3 velocity = Vector3.zero; // 현재 속도

    void LateUpdate()
    {
        if (target != null)
        {
            // 목표 위치를 계산 (플레이어 위치 + 오프셋)
            Vector3 desiredPosition = target.position + offset;

            // 부드럽게 이동하기 위해 SmoothDamp 함수 사용
            Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);

            // 카메라 위치를 업데이트
            transform.position = smoothedPosition;
        }
    }
    private void Update()
    {
        // 플레이어를 태그로 찾아 할당합니다.
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
    }
    void Start()
    {
   
        // 게임이 시작될 때 초기 오프셋을 설정합니다.
        if (target != null)
        {
            offset = transform.position - target.position;
        }
    }
}
