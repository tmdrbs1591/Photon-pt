using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f; // 총알의 속도

    void Start()
    {
        // 앞으로 이동하는 방향으로 초기화
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
    }

    void Update()
    {
        // 예를 들어, 총알이 일정 거리를 움직였을 때 자동으로 제거할 수 있습니다.
        // 이 코드는 예시이며, 필요에 따라 수정하셔야 합니다.
        if (transform.position.magnitude > 100f)
        {
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 다른 객체와 충돌할 때 처리할 코드를 작성합니다.
        // 예를 들어, 충돌 시 피해를 입히거나 효과를 적용할 수 있습니다.
    }
}
