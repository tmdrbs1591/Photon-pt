using DG.Tweening;
using UnityEngine;

public class InOutBackAnim : MonoBehaviour
{
    public RectTransform rect;

    // Start is called before the first frame update
    void Start()
    {
        // RectTransform �ʱ�ȭ
        rect = GetComponent<RectTransform>();

        // �ʱ� �������� 0���� ���� (Optional: Start���� �ٷ� ���� ���·� �����ϰ� �ʹٸ�)
        // rect.localScale = Vector3.zero;
    }

    // Update�� �ʿ� �����Ƿ� �����մϴ�

    public void Show()
    {
        rect.localScale = Vector3.zero;  // �ʱ� �������� 0���� ����
        rect.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);  // DOTween�� ����Ͽ� ������ �ִϸ��̼� �߰�
    }

    public void Hide()
    {
        // �ִϸ��̼��� ����Ͽ� �������� 0���� ���Դϴ�
        rect.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
    }
}
