using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Voice.Unity;
using Photon.Voice;
using UnityEngine.UI;
using TMPro;

public class MicSelection : MonoBehaviour
{
    public Recorder recorder;

    public TMP_Dropdown dropdown;
    private void Awake()
    {
        var mics = new List<string>(Microphone.devices);
        dropdown.AddOptions(mics);
    }
    public void SetMic(int i)
    {
        string mic = Microphone.devices[i];
        recorder.MicrophoneDevice = new DeviceInfo(mic);
    }
}
