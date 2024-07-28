using Febucci.UI;
using UnityEngine;

public class TextAnim : MonoBehaviour
{

    [SerializeField] TypewriterByCharacter typewriter;

    [TextArea]
    [SerializeField] public string textToShow = "<shake>�� ���丮 �̤�\r\n�̰� �� ������Ƥ̤�<shake>";

    private void OnEnable()
    {
        TextAnimStart();
    }
    public void TextAnimStart()
    {
        // Ÿ�ڱ� ��带 �����ϸ� �ؽ�Ʈ�� �����մϴ�.
        typewriter.ShowText(textToShow);
        
        // OnCharacterVisible �̺�Ʈ�� �ڵ鷯�� ����մϴ�.
        typewriter.onCharacterVisible.AddListener(OnCharacterVisible);
    }

    private void OnDisable()
    {
        // OnCharacterVisible �̺�Ʈ���� �ڵ鷯�� �����մϴ�.
        typewriter.onCharacterVisible.RemoveListener(OnCharacterVisible);
    }

    // ���ڰ� ȭ�鿡 ǥ�õ� �� ȣ��Ǵ� �Լ��Դϴ�.
    private void OnCharacterVisible(char letter)
    {
      //  Debug.Log("���ڰ� ǥ�õǾ����ϴ�: " + letter);
        // ���⿡ ���ϴ� ������ �߰��� �� �ֽ��ϴ�.
    }
}
