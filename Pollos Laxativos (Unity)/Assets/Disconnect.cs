using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Disconnect : MonoBehaviour
{
    [SerializeField] private Button _button;

    private void Update()
    {
        if (GameManager.Instance.LevelManager.GameStarted == false)
        {
            _button.interactable = true;
        }

        else
        {
            _button.interactable = false;
        }
    }

    public void DisconnectFromGame()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("Loader");
    }
}
