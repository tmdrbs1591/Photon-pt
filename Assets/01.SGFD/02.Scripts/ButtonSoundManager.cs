using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSoundManager : MonoBehaviour
{
   public void PlaySound(int index)
    {
        SingleAudioManager.instance.PlaySound(transform.position, index, Random.Range(1f, 1f), 0.4f);

    }
}
