using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MasterManager : MonoBehaviourPunCallbacks
{
    static MasterManager _instance;
    public GameManager gameManager;

    Dictionary<Player, PlayerModel> _dicChars = new Dictionary<Player, PlayerModel>();
    Dictionary<PlayerModel, Player> _dicPlayer = new Dictionary<PlayerModel, Player>();

    public static MasterManager Instance
    {
        get
        {
            return _instance;
        }
    }
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (var item in _dicChars)
            {
                if (item.Value.CanAttack)
                {
                    item.Value.DoAttack();
                }
            }
        }
    }

    public void RPCMaster(string name, params object[] p)
    {
        RPC(name, PhotonNetwork.MasterClient, p);
    }
    public void RPC(string name, Player target, params object[] p)
    {
        photonView.RPC(name, target, p);
    }

    [PunRPC]
    public void RequestConnectPlayer(Player client)
    {

        //GameObject obj = PhotonNetwork.Instantiate("CharacterFullAuth", Vector3.zero, Quaternion.identity);
        var character = GameManager.Instance.LevelManager.SpawnPlayer();
        photonView.RPC("UpdatePlayer", RpcTarget.All, client, character.photonView.ViewID);
    }

    [PunRPC]
    public void UpdatePlayer(Player client, int id)
    {
        PhotonView pv = PhotonView.Find(id);
        var character = pv.gameObject.GetComponent<PlayerModel>();

        if (!_dicChars.ContainsKey(client))
        {
            _dicChars[client] = character;
        }

        if (!_dicPlayer.ContainsKey(character))
        {
            _dicPlayer[character] = client;
        }


    }

    [PunRPC]
    public void RequestMove(Player client, Vector3 dir)
    {
        if (_dicChars.ContainsKey(client))
        {
            var character = _dicChars[client];
            character.DoMove(dir);
            character.DoRotate(dir);
        }
    }

    [PunRPC]
    public void RequestAttack(Player client)
    {
        if (_dicChars.ContainsKey(client))
        {
            var character = _dicChars[client];
            character.DoAttack();
        }
    }

    [PunRPC]
    public void RequestDash(Player client)
    {
        if (_dicChars.ContainsKey(client))
        {
            var character = _dicChars[client];
            if (character.CanDash)
                character.DoDash();
        }
    }

    public Player GetClientFromModel(PlayerModel model)
    {
        if (_dicPlayer.ContainsKey(model))
        {
            return _dicPlayer[model];
        }
        return null;
    }

    public override void OnPlayerLeftRoom(Player player)
    {
        PhotonNetwork.Destroy(_dicChars[player].gameObject);
    }

    [PunRPC]
    public void StartCountdown(Player client)
    {
       // GameManager.Instance.LevelManager.photonView.RPC;
    }

    [PunRPC]
    public void DoCountdown(string status)
    {
        GameManager.Instance.LevelManager.UpdateCountdown(status);
    }
}
