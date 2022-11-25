using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;

public class VoiceUI : MonoBehaviourPun
{
    public Speaker speaker;
    MicUI _mic;
    private void Awake()
    {
        if (photonView.IsMine)
        {
            _mic = FindObjectOfType<MicUI>();
        }
        else
        {
            FindObjectOfType<VoicePlayersUI>().AddSpeaker(speaker, photonView.Owner);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            bool v = PunVoiceClient.Instance.PrimaryRecorder.TransmitEnabled;
            _mic.Show(v);
        }
    }
}
