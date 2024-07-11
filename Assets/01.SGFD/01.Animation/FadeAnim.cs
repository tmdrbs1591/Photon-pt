using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class FadeAnim : MonoBehaviour
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
            images[i].material.SetFloat("_FadeAmount", temp);
    }

    IEnumerator DoShine()
    {
        yield return new WaitForSeconds(1.0f);
        while (true)
        {
            temp += Time.deltaTime;

            if (temp > 1.0f)
            {
                temp = 0.0f;
                yield return new WaitForSeconds(2.0f);
            }

            for (int i = 0; i < images.Length; i++)
                images[i].material.SetFloat("_FadeAmount", temp);

            yield return null;
        }
    }
}
