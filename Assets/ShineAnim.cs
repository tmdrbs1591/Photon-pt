using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ShineAnim : MonoBehaviour
{
    public Color color;
    public Image[] images;
    private float temp = 0; // Ŭ���� �������� temp ������ ����

    private void OnEnable()
    {
        StartCoroutine(DoShine());
    }

    private void OnDisable()
    {
        temp = 0; // �ڷ�ƾ�� ������ �� temp ������ 0���� �ʱ�ȭ
        for (int i = 0; i < images.Length; i++)
            images[i].material.SetFloat("_ShineLocation", temp);
    }

    IEnumerator DoShine()
    {
        // ���� ���� �ð��� 0.3�ʿ��� 1�� ������ ���� ������ ����
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
