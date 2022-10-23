using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerView : MonoBehaviourPun
{
    [SerializeField] private Username _usernamePrefab;
    [SerializeField] private Username _username;
    [SerializeField] private string _name;

    public Username Username => _username;

    private void Start()
    {
        var canvas = GameObject.Find("Game Canvas");
        _username = GameObject.Instantiate<Username>(_usernamePrefab, canvas.transform);
        _username.gameObject.AddComponent<PhotonView>();
        _username.SetTarget(transform);
        //_username.SetUsername(photonView.Owner.NickName); 

        if (string.IsNullOrEmpty(_name))
        {
            _username.SetUsername(photonView.Owner.NickName);
        }

        if (photonView.IsMine)
        {
            var username = photonView.Owner.NickName;
            photonView.RPC("UpdateUsername", RpcTarget.AllBuffered, username);
        }

        else
        {
            photonView.RPC("RequestUsername", photonView.Owner, PhotonNetwork.LocalPlayer);
        }
    }

    [PunRPC]
    public void RequestUsername(Player client)
    {
        photonView.RPC("UpdateUsername", client, photonView.Owner.NickName);
    }

    [PunRPC]
    public void UpdateUsername(string username)
    {
        _name = username;
        
        if (_name != null && _username != null)
        {
            _username.SetUsername(username);
        }
    }

    private void OnDestroy()
    {
        if (_username == null) return;
        Destroy(_username.gameObject);
    }
}
 