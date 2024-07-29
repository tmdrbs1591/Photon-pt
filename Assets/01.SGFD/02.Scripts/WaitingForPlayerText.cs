using System.Collections;
using UnityEngine;
using TMPro;

public class WaitingForPlayerText : MonoBehaviour
{
    public TextMeshProUGUI waitingText; // TextMeshProUGUI ������Ʈ�� ������ ����
    private string baseText = "�÷��̾� ��ٸ�����";
    private int dotCount = 0;
    private float interval = 0.5f; // ���� �߰��Ǵ� ����

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
            dotCount = (dotCount + 1) % 4; // ���� ������ 0, 1, 2, 3 ������ �ݺ�
            yield return new WaitForSeconds(interval);
        }
    }
}
