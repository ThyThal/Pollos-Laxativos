using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Voice.Unity;
using Photon.Realtime;
using TMPro;


public class VoicePlayersUI : MonoBehaviour
{
    Dictionary<Speaker, Player> _dic = new Dictionary<Speaker, Player>();
    public TextMeshProUGUI text;
    
    public void AddSpeaker(Speaker speaker, Player client)
    {
        _dic[speaker] = client;
    }

    private void Update()
    {
        string voiceSpeakers = "";

        foreach (var item in _dic)
        {
            var speaker = item.Key;
            if (speaker.IsPlaying)
            {
                voiceSpeakers += item.Value.NickName + "\n";
            }
            text.text = voiceSpeakers;
        }
    }
}
