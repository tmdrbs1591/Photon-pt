using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonTextChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public TextMeshProUGUI buttonText; // ��ư�� �ؽ�Ʈ ������Ʈ
    public Button button; // ��ư�� �ؽ�Ʈ ������Ʈ
    public Color normalTextColor = Color.white; // �⺻ �ؽ�Ʈ ����
    public Color highlightedTextColor = Color.black; // ���� �ؽ�Ʈ ����
    public Color pressedTextColor = Color.gray; // ������ �� �ؽ�Ʈ ����


    private void Awake()
    {
        button = GetComponent<Button>();
    }
    // ���콺�� ��ư ���� ���� �� ȣ��
    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.color = highlightedTextColor;
    }

    // ���콺�� ��ư�� ���� �� ȣ��
    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.color = normalTextColor;
    }

    // ��ư�� ������ �� ȣ��
    public void OnPointerDown(PointerEventData eventData)
    {
        buttonText.color = pressedTextColor;
    }

    // ��ư���� ���� ���� �� ȣ��
    public void OnPointerUp(PointerEventData eventData)
    {
        buttonText.color = normalTextColor;
    }
}