using UnityEngine;

public class CharGroup : MonoBehaviour
{
    [SerializeField] private GameObject[] charObj;

    void Update()
    {
        // 현재 캐릭터에 따라 캐릭터 오브젝트들을 활성화/비활성화
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

    // 해당 인덱스의 캐릭터 오브젝트를 활성화하고 나머지는 비활성화하는 함수
    private void SetActiveCharacter(int index)
    {
        for (int i = 0; i < charObj.Length; i++)
        {
            charObj[i].SetActive(i == index);
        }
    }
}
