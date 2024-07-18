using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharImage : MonoBehaviour
{
    [SerializeField] Image currentImage;

    [SerializeField] Sprite KnightSprite; // Knight 캐릭터의 스프라이트
    [SerializeField] Sprite ArcherSprite; // Archer 캐릭터의 스프라이트


    void Start()
    {
    }

    void Update()
    {

        // CharManager에서 현재 캐릭터를 가져와서 이미지를 설정합니다.
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
