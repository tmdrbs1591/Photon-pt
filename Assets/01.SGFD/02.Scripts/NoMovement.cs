using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoMovement : MonoBehaviour
{
    private Vector2 originalXY;

    private void Start()
    {
        originalXY = new Vector2(transform.position.x, transform.position.y); // 초기 x, y 위치 저장
    }

    private void Update()
    {
        // 현재 위치를 originalXY로 고정하고 z 값은 유지
        transform.position = new Vector3(originalXY.x, originalXY.y, transform.position.z);
    }
}


