using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ShineAnim : MonoBehaviour
{
    public Color color;
    public Image[] images;
    private float temp = 0; // 클래스 레벨에서 temp 변수를 정의

    private void OnEnable()
    {
        StartCoroutine(DoShine());
    }

    private void OnDisable()
    {
        temp = 0; // 코루틴이 중지될 때 temp 변수를 0으로 초기화
        for (int i = 0; i < images.Length; i++)
            images[i].material.SetFloat("_ShineLocation", temp);
    }

    IEnumerator DoShine()
    {
        // 시작 지연 시간을 0.3초에서 1초 사이의 랜덤 값으로 설정
        float startDelay = Random.Range(0.3f, 1.0f);
        yield return new WaitForSeconds(startDelay);
        while (true)
        {
            temp += Time.deltaTime;

            if (temp > 1.0f)
            {
                temp = 0.0f;
                yield return new WaitForSeconds(0.2f);
            }

            for (int i = 0; i < images.Length; i++)
                images[i].material.SetFloat("_ShineLocation", temp);

            yield return null;
        }
    }
}
