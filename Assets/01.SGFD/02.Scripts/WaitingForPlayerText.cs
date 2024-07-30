using System.Collections;
using UnityEngine;
using TMPro;

public class WaitingForPlayerText : MonoBehaviour
{
    public TextMeshProUGUI waitingText; // TextMeshProUGUI 컴포넌트를 연결할 변수
    private string baseText = "플레이어 기다리는중";
    private int dotCount = 0;
    private float interval = 0.5f; // 점이 추가되는 간격

    void Start()
    {
        waitingText = GetComponent<TextMeshProUGUI>();

        StartCoroutine(UpdateWaitingText());
    }

    IEnumerator UpdateWaitingText()
    {
        while (true)
        {
            waitingText.text = baseText + new string('.', dotCount);
            dotCount = (dotCount + 1) % 4; // 점의 개수를 0, 1, 2, 3 순으로 반복
            yield return new WaitForSeconds(interval);
        }
    }
}
