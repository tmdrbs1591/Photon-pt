using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour
{
    public AugmentData.AugmentType type;
    public float rate;
    public PlayerStats playerstats;

    private void Awake()
    {
        playerstats =transform.parent.GetComponent<PlayerStats>();
    }
    public void Init(AugmentData data)
    {
        //Basic Set
        name = "Gear " + data.augmentId;

        //Property Set
        type = data.augmentType;
        rate = data.damages[0];
        ApplyGear();
    }
    void ApplyGear()
    {
        switch (type)
        {
            case AugmentData.AugmentType.Damage:
                DamageUp();
                break;
            case AugmentData.AugmentType.Speed:
                SpeedUp();
                break;
        }
    }

    public void LevelUp(float rate)
    {
        this.rate = rate;
        ApplyGear();
    }

    void DamageUp()
    {
        playerstats.attackPower += rate;
    }

    void SpeedUp()
    {
        float speed = 3;
        playerstats.speed = speed + speed * rate;
    }
}
