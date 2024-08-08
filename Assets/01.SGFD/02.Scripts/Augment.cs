using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
public class Augment : MonoBehaviour
{
    public AugmentData data;
    public int level;
    public PlayerCtrl playerCtrl;
    public Gear gear;
    public PlayerStats playerStats;

    Image icon;
    TMP_Text textLevel;
    TMP_Text textName;
    TMP_Text textDesc;

    private void Awake()
    {
        playerStats = transform.parent.parent.parent.parent.parent.GetComponent<PlayerStats>();
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = data.augmentIcon;

        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>();
        textLevel = texts[0];
        textName = texts[1];
        textDesc = texts[2];
        textName.text = data.augmentName;

    }
    private void OnEnable()
    {
        textDesc.text = string.Format(data.augmentDesc);
    }
    private void LateUpdate()
    {
        textLevel.text = "Lv." + level;
    }
    public void OnClick()
    {
        switch (data.augmentType)
        {
            case AugmentData.AugmentType.Damage:
                Debug.Log("damageUp");
                playerStats.attackPower += 10;
                Debug.Log(playerStats.attackPower);

                break;
            case AugmentData.AugmentType.Speed:
                Debug.Log("speedUp");
                playerStats.speed += 1;
                break;
            case AugmentData.AugmentType.Heal:
                break;
            case AugmentData.AugmentType.AttackSpeed:
                Debug.Log("attackspeedUp");
                playerStats.attackCoolTime -= 0.05f;
                break;
            case AugmentData.AugmentType.Glove:
                break;
            case AugmentData.AugmentType.SkillCool:
                playerStats.skillCoolTime -= 1f;
                break;


        }

        level++;

        if (level == data.damages.Length) {
            GetComponent<Button>().interactable = false;
        }
    }
}
