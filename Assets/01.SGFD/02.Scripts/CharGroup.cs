using UnityEngine;

public class CharGroup : MonoBehaviour
{
    [SerializeField] private GameObject[] charObj;

    void Update()
    {
        // ���� ĳ���Ϳ� ���� ĳ���� ������Ʈ���� Ȱ��ȭ/��Ȱ��ȭ
        switch (CharManager.instance.currentCharacter)
        {
            case Character.Knight:
                SetActiveCharacter(0);
                break;
            case Character.Archer:
                SetActiveCharacter(1);
                break;
            case Character.Dragoon:
                SetActiveCharacter(2);
                break;
        }
    }

    // �ش� �ε����� ĳ���� ������Ʈ�� Ȱ��ȭ�ϰ� �������� ��Ȱ��ȭ�ϴ� �Լ�
    private void SetActiveCharacter(int index)
    {
        for (int i = 0; i < charObj.Length; i++)
        {
            charObj[i].SetActive(i == index);
        }
    }
}
