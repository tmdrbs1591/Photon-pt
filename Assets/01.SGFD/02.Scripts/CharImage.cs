using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class CharImage : MonoBehaviourPunCallbacks
{
    [SerializeField] Image currentImage;

    [SerializeField] Sprite KnightSprite; // Knight ĳ������ ��������Ʈ
    [SerializeField] Sprite ArcherSprite; // Archer ĳ������ ��������Ʈ
    [SerializeField] Sprite DragoonSprite; // Archer ĳ������ ��������Ʈ


    void Start()
    {
    }

    void Update()
    {

        // CharManager���� ���� ĳ���͸� �����ͼ� �̹����� �����մϴ�.
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
