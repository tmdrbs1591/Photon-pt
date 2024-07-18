using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharImage : MonoBehaviour
{
    [SerializeField] Image currentImage;

    [SerializeField] Sprite KnightSprite; // Knight ĳ������ ��������Ʈ
    [SerializeField] Sprite ArcherSprite; // Archer ĳ������ ��������Ʈ


    void Start()
    {
    }

    void Update()
    {

        // CharManager���� ���� ĳ���͸� �����ͼ� �̹����� �����մϴ�.
        switch (CharManager.instance.currentCharacter)
        {
            case Character.Knight:
                currentImage.sprite = KnightSprite;
                break;

            case Character.Archer:
                currentImage.sprite = ArcherSprite;
                break;
        }
    }

}
