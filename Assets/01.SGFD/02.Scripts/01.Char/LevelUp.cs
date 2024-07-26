using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // Button ������Ʈ ����� ���� ���ӽ����̽�
using DG.Tweening;

public class LevelUp : MonoBehaviour
{
    RectTransform rect;
    public Augment[] augments;
    public Button[] buttons;  // UI ��ư �迭

    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        augments = GetComponentsInChildren<Augment>(true);
        // ��ư ������Ʈ�� �迭�� ��������
        buttons = GetComponentsInChildren<Button>(true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Show()
    {
        Next();
        rect.localScale = Vector3.zero;  // �ʱ� �������� 0���� ����
        rect.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);  // DOTween�� ����Ͽ� ������ �ִϸ��̼� �߰�
    }

    public void Hide()
    {
        // ��ư ��ȣ�ۿ� ��Ȱ��ȭ
        SetButtonsInteractable(false);

        // �ִϸ��̼� �Ϸ� �� ��ư ��ȣ�ۿ��� �ٽ� Ȱ��ȭ�ϴ� �ڷ�ƾ ȣ��
        rect.DOScale(Vector3.zero, 0.5f)
            .SetEase(Ease.InBack)
            .OnComplete(() => SetButtonsInteractable(true));  // �ִϸ��̼� �Ϸ� �� ��ư ��ȣ�ۿ��� �ٽ� Ȱ��ȭ
    }

    void Next()
    {
        foreach (Augment aug in augments)
        {
            aug.gameObject.SetActive(false);
        }

        // �ߺ����� �ʴ� �ε��� ����
        List<int> indices = new List<int>();
        for (int i = 0; i < augments.Length; i++)
        {
            indices.Add(i);
        }

        // �������� ����
        for (int i = 0; i < indices.Count; i++)
        {
            int temp = indices[i];
            int randomIndex = Random.Range(i, indices.Count);
            indices[i] = indices[randomIndex];
            indices[randomIndex] = temp;
        }

        // ó�� 3���� �ε��� ����
        int[] ran = indices.GetRange(0, 3).ToArray();

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

            // RectTransform�� ����Ͽ� �ʱ� ��ġ ���� �� �ִϸ��̼� �߰�
            RectTransform augRect = ranAug.GetComponent<RectTransform>();
            if (augRect != null)
            {
                // �ʱ� ��ġ�� ���� (ȭ�� �Ʒ�)
                Vector2 startPosition = new Vector2(augRect.anchoredPosition.x, -augRect.rect.height * 1.5f);
                augRect.anchoredPosition = startPosition;

                // DOTween �ִϸ��̼�
                float delay = index * 0.1f;  // ran �迭 ���� ����Ͽ� ������ ����
                // ��ǥ ��ġ�� ȭ�� �Ʒ������� ���� (��: -370)
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
