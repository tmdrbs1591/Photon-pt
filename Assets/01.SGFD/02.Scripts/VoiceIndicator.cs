using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Voice.PUN;

public class VoiceIndicator : MonoBehaviour
{
    [SerializeField] private Image micImage;
    [SerializeField] private Image speakerImage;
    [SerializeField] private PhotonVoiceView photonVoiceView;

    private void Awake()
    {
        this.micImage.enabled = true;
        this.speakerImage.enabled = false;
    }

    // Update is called once per frame 
    void Update()
    {
        if (this.photonVoiceView.IsSpeaking)
        {
            this.micImage.enabled = false;
            this.speakerImage.enabled = true;
        }
        else
        {
            this.micImage.enabled = true;
            this.speakerImage.enabled = false;
        }
    }
}
