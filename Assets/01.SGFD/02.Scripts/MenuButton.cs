using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    public ButtonManager buttonManager; // ��ư ������ ��ü
    [SerializeField] float startDelay; // ��ư �ִϸ��̼� ���� ������
    [SerializeField] float scaleDuration = 0.2f; // ������ �ִϸ��̼� ���� �ð�
    [SerializeField] Vector3 scaleUp = new Vector3(1.2f, 1.2f, 1.2f); // ���콺 ���� �� ������
    [SerializeField] Vector3 navScaleUp = new Vector3(1.1f, 1.1f, 1.1f); // �׺���̼� �� ������

    Button menuBtn; // Unity UI Button ������Ʈ

    public Material outlineMaterial; // �ƿ����� ���׸���

    private Image buttonImage;
    private Material originalMaterial; // �⺻ ���׸��� ���� ����

    private bool isNavigated = false; // �׺���̼����� ���õ� �������� ����

    public string Type;

    private Vector3 originalPosition;

    private void Awake()
    {
        // Awake���� �ʱ� ��ġ�� ����
        originalPosition = transform.position;
    }

    private void OnEnable()
    {
        // ��ġ�� �ʱ�ȭ�ϰ� �ִϸ��̼��� ����
        transform.position = originalPosition;
        StartCoroutine(AnimateButton());
    }

    void Start()
    {
        buttonImage = GetComponent<Image>();
        originalMaterial = buttonImage.material;
        menuBtn = GetComponent<Button>();
    }

    IEnumerator AnimateButton()
    {
        // ��ư�� �ʱ� ��ġ�� �������� �̵���ŵ�ϴ�.
        Vector3 startPos = originalPosition + new Vector3(-30, 0, 0); // 100�� ������ ���Դϴ�.
        transform.position = startPos;
        yield return new WaitForSeconds(startDelay);
        // ��ư�� ���������� �ε巴�� �̵���Ű�� �ִϸ��̼�
        transform.DOMoveX(originalPosition.x, 1f).SetEase(Ease.OutSine);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isNavigated)
        {
            //if (Type == "MenuButton")
            //    AudioManager.instance.PlaySound(transform.position, 3, Random.Range(1.2f, 1.2f), 1);
            //else if (Type == "ModeButton")
            //    AudioManager.instance.PlaySound(transform.position, 3, Random.Range(1.2f, 1.2f), 1);
            //else
            //    AudioManager.instance.PlaySound(transform.position, 12, Random.Range(1f, 1f), 1);
            // ���콺�� ��ư ���� ���� �� �ƿ����� ���׸���� ��ü�մϴ�.
            buttonImage.material = outlineMaterial;

            // �߰����� �ִϸ��̼� ���� ������ �� �ֽ��ϴ�.
            transform.DOScale(scaleUp, scaleDuration).SetEase(Ease.OutSine);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isNavigated)
        {
            // ���콺�� ��ư���� ��� �� �⺻ ���׸���� ��ü�մϴ�.
            buttonImage.material = originalMaterial;

            // �߰����� �ִϸ��̼� ���� ������ �� �ֽ��ϴ�.
            transform.DOScale(Vector3.one, scaleDuration).SetEase(Ease.OutSine);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        isNavigated = true;

        // �׺���̼����� ��ư�� ���õ��� ���� ó��
        //transform.DOScale(navScaleUp, scaleDuration).SetEase(Ease.OutSine);
        //buttonImage.material = outlineMaterial;
        //if (Type == "MenuButton")
        //    AudioManager.instance.PlaySound(transform.position, 3, Random.Range(1.2f, 1.2f), 1);
        //else if (Type == "ModeButton")
        //    AudioManager.instance.PlaySound(transform.position, 3, Random.Range(1.2f, 1.2f), 1);
        //else
        //    AudioManager.instance.PlaySound(transform.position, 12, Random.Range(1f, 1f), 1);

    }

    public void OnDeselect(BaseEventData eventData)
    {
        isNavigated = false;

        //�׺���̼ǿ��� ��ư�� ���� �������� ���� ó��
        transform.DOScale(Vector3.one, scaleDuration).SetEase(Ease.OutSine);
        buttonImage.material = originalMaterial;
    }

    void Update()
    {
        NavLock();
    }
    public void NavLock()
    {
        //if (buttonManager.isNavimpossible && Type == "MenuButton" || buttonManager.isCharPanel && Type == "MenuButton" || buttonManager.isTitleSettingPanel && Type == "MenuButton")
        //{
        //    var navigation = new Navigation();
        //    navigation.mode = Navigation.Mode.None;

        //    menuBtn.navigation = navigation;
        //}
        //else if (Type == "SettingButton")
        //{
        //    var navigation = new Navigation();
        //    navigation.mode = Navigation.Mode.Vertical;

        //    menuBtn.navigation = navigation;
        //}
        //else
        //{
        //    var navigation = new Navigation();
        //    navigation.mode = Navigation.Mode.Horizontal;

        //    menuBtn.navigation = navigation;
        //}
    }
}
