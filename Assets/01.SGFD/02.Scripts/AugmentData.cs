using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using UnityEngine;


[CreateAssetMenu(fileName = "Augment", menuName = "Scriptble Object/AugmentData")]
public class AugmentData : ScriptableObject
{
    public enum AugmentType { Damage,AttackSpeed,Glove,Speed,Heal,SkillCool}

    [Header("# Main Info")]
    public AugmentType augmentType;
    public int augmentId;
    public string augmentName;
    [TextArea]
    public string augmentDesc;
    public Sprite augmentIcon;

    [Header("# Level Data")]
    public float baseDamage;
    public int baseCount;
    public float[] damages;
    public int[] counts;

    [Header("# Weapon")]
    public GameObject projectile;

  
}
