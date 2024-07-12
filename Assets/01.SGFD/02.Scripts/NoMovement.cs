using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoMovement : MonoBehaviour
{
    private Vector2 originalXY;

    private void Start()
    {
        originalXY = new Vector2(transform.position.x, transform.position.y); // �ʱ� x, y ��ġ ����
    }

    private void Update()
    {
        // ���� ��ġ�� originalXY�� �����ϰ� z ���� ����
        transform.position = new Vector3(originalXY.x, originalXY.y, transform.position.z);
    }
}


