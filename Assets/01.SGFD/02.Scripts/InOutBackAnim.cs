using DG.Tweening;
using UnityEngine;

public class InOutBackAnim : MonoBehaviour
{
    public RectTransform rect;

    // Start is called before the first frame update
    void Start()
    {
        // RectTransform 초기화
        rect = GetComponent<RectTransform>();

        // 초기 스케일을 0으로 설정 (Optional: Start에서 바로 숨긴 상태로 시작하고 싶다면)
        // rect.localScale = Vector3.zero;
    }

    // Update는 필요 없으므로 제거합니다

    public void Show()
    {
        rect.localScale = Vector3.zero;  // 초기 스케일을 0으로 설정
        rect.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);  // DOTween을 사용하여 스케일 애니메이션 추가
    }

    public void Hide()
    {
        // 애니메이션을 사용하여 스케일을 0으로 줄입니다
        rect.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
    }
}
