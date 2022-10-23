using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class Ping : MonoBehaviour
{
    [SerializeField] private TMP_Text _ping;

    // Update is called once per frame
    void Update()
    {
        if (_ping == null) return;
        _ping.text = $"{PhotonNetwork.GetPing().ToString()} ms.";
    }
}
