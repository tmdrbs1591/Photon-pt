using Febucci.UI;
using UnityEngine;

public class TextAnim : MonoBehaviour
{

    [SerializeField] TypewriterByCharacter typewriter;

    [TextArea]
    [SerializeField] public string textToShow = "<shake>내 도토리 ㅜㅜ\r\n이걸 다 언제담아ㅜㅜ<shake>";

    private void OnEnable()
    {
        TextAnimStart();
    }
    public void TextAnimStart()
    {
        // 타자기 모드를 시작하며 텍스트를 전달합니다.
        typewriter.ShowText(textToShow);
        
        // OnCharacterVisible 이벤트에 핸들러를 등록합니다.
        typewriter.onCharacterVisible.AddListener(OnCharacterVisible);
    }

    private void OnDisable()
    {
        // OnCharacterVisible 이벤트에서 핸들러를 제거합니다.
        typewriter.onCharacterVisible.RemoveListener(OnCharacterVisible);
    }

    // 문자가 화면에 표시될 때 호출되는 함수입니다.
    private void OnCharacterVisible(char letter)
    {
      //  Debug.Log("문자가 표시되었습니다: " + letter);
        // 여기에 원하는 동작을 추가할 수 있습니다.
    }
}
