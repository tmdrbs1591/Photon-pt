using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    public ButtonManager buttonManager; // 버튼 관리자 객체
    [SerializeField] float startDelay; // 버튼 애니메이션 시작 딜레이
    [SerializeField] float scaleDuration = 0.2f; // 스케일 애니메이션 실행 시간
    [SerializeField] Vector3 scaleUp = new Vector3(1.2f, 1.2f, 1.2f); // 마우스 오버 시 스케일
    [SerializeField] Vector3 navScaleUp = new Vector3(1.1f, 1.1f, 1.1f); // 네비게이션 시 스케일

    Button menuBtn; // Unity UI Button 컴포넌트

    public Material outlineMaterial; // 아웃라인 메테리얼

    private Image buttonImage;
    private Material originalMaterial; // 기본 메테리얼 저장 변수

    private bool isNavigated = false; // 네비게이션으로 선택된 상태인지 여부

    public string Type;

    private Vector3 originalPosition;

    private void Awake()
    {
        // Awake에서 초기 위치를 저장
        originalPosition = transform.position;
    }

    private void OnEnable()
    {
        // 위치를 초기화하고 애니메이션을 시작
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
        // 버튼의 초기 위치를 왼쪽으로 이동시킵니다.
        Vector3 startPos = originalPosition + new Vector3(-30, 0, 0); // 100은 임의의 값입니다.
        transform.position = startPos;
        yield return new WaitForSeconds(startDelay);
        // 버튼을 오른쪽으로 부드럽게 이동시키는 애니메이션
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
            // 마우스가 버튼 위에 있을 때 아웃라인 메테리얼로 교체합니다.
            buttonImage.material = outlineMaterial;

            // 추가적인 애니메이션 등을 실행할 수 있습니다.
            transform.DOScale(scaleUp, scaleDuration).SetEase(Ease.OutSine);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isNavigated)
        {
            // 마우스가 버튼에서 벗어날 때 기본 메테리얼로 교체합니다.
            buttonImage.material = originalMaterial;

            // 추가적인 애니메이션 등을 실행할 수 있습니다.
            transform.DOScale(Vector3.one, scaleDuration).SetEase(Ease.OutSine);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        isNavigated = true;

        // 네비게이션으로 버튼이 선택됐을 때의 처리
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

        //네비게이션에서 버튼이 선택 해제됐을 때의 처리
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
