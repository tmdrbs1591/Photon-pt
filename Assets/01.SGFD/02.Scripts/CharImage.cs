using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class CharImage : MonoBehaviourPunCallbacks
{
    [SerializeField] Image currentImage;

    [SerializeField] Sprite KnightSprite; // Knight 캐릭터의 스프라이트
    [SerializeField] Sprite ArcherSprite; // Archer 캐릭터의 스프라이트
    [SerializeField] Sprite DragoonSprite; // Archer 캐릭터의 스프라이트


    void Start()
    {
    }

    void Update()
    {

        // CharManager에서 현재 캐릭터를 가져와서 이미지를 설정합니다.
        switch (CharManager.instance.currentCharacter)
        {
            case Character.Knight:
                NetworkManager.instance.charImage.currentImage.sprite = KnightSprite;
                break;

            case Character.Archer:
                NetworkManager.instance.charImage.currentImage.sprite = ArcherSprite;
                break;

            case Character.Dragoon:
                NetworkManager.instance.charImage.currentImage.sprite = DragoonSprite;
                break;
        }
    }

}
