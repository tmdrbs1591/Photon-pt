using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // Button 컴포넌트 사용을 위한 네임스페이스
using DG.Tweening;

public class LevelUp : MonoBehaviour
{
    RectTransform rect;
    public Augment[] augments;
    public Button[] buttons;  // UI 버튼 배열

    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        augments = GetComponentsInChildren<Augment>(true);
        // 버튼 컴포넌트를 배열로 가져오기
        buttons = GetComponentsInChildren<Button>(true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Show()
    {
        Next();
        rect.localScale = Vector3.zero;  // 초기 스케일을 0으로 설정
        rect.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);  // DOTween을 사용하여 스케일 애니메이션 추가
    }

    public void Hide()
    {
        // 버튼 상호작용 비활성화
        SetButtonsInteractable(false);

        // 애니메이션 완료 후 버튼 상호작용을 다시 활성화하는 코루틴 호출
        rect.DOScale(Vector3.zero, 0.5f)
            .SetEase(Ease.InBack)
            .OnComplete(() => SetButtonsInteractable(true));  // 애니메이션 완료 후 버튼 상호작용을 다시 활성화
    }

    void Next()
    {
        foreach (Augment aug in augments)
        {
            aug.gameObject.SetActive(false);
        }

        int[] ran = new int[3];
        while (true)
        {
            ran[0] = Random.Range(0, augments.Length);
            ran[1] = Random.Range(0, augments.Length);
            ran[2] = Random.Range(0, augments.Length);

            if (ran[0] != ran[1] && ran[1] != ran[2] && ran[0] != ran[2])
                break;
        }

        for (int index = 0; index < ran.Length; index++)
        {
            Augment ranAug = augments[ran[index]];
            if (ranAug.level == ranAug.data.damages.Length)
            {
                augments[2].gameObject.SetActive(true);
            }
            else
            {
                ranAug.gameObject.SetActive(true);
            }

            // RectTransform을 사용하여 초기 위치 설정 및 애니메이션 추가
            RectTransform augRect = ranAug.GetComponent<RectTransform>();
            if (augRect != null)
            {
                // 초기 위치를 설정 (화면 아래)
                Vector2 startPosition = new Vector2(augRect.anchoredPosition.x, -augRect.rect.height * 1.5f);
                augRect.anchoredPosition = startPosition;

                // DOTween 애니메이션
                float delay = index * 0.1f;  // ran 배열 값에 기반하여 딜레이 설정
                // 목표 위치를 화면 아래쪽으로 조정 (예: -400)
                float finalPositionY = -370f;
                augRect.DOAnchorPosY(finalPositionY, 0.5f).SetDelay(delay).SetEase(Ease.OutBack);
            }
        }
    }

    void SetButtonsInteractable(bool interactable)
    {
        foreach (Button button in buttons)
        {
            button.interactable = interactable;
        }
    }
}
