using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MasterManager : MonoBehaviourPunCallbacks
{
    static MasterManager _instance;
    public GameManagerFullAuth gameManager;

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
        var character = GameManagerFullAuth.Instance.LevelManager.Spawn();
        photonView.RPC("UpdatePlayer", RpcTarget.All, client, character.photonView.ViewID);
    }

    [PunRPC]
    public void UpdatePlayer(Player client, int id)
    {
        PhotonView pv = PhotonView.Find(id);
        var character = pv.gameObject.GetComponent<PlayerModel>();
        _dicChars[client] = character;
        _dicPlayer[character] = client;
        gameManager.SetManager(character);
    }

    [PunRPC]
    public void RequestMove(Player client, Vector3 dir)
    {
        if (_dicChars.ContainsKey(client))
        {
            var character = _dicChars[client];
            character.Move(dir);
            character.Rotate(dir);
        }
    }

    [PunRPC]
    public void RequestAttack(Player client)
    {
        if (_dicChars.ContainsKey(client))
        {
            var character = _dicChars[client];
            character.AttackFA();
        }
    }

    [PunRPC]
    public void RequestDash(Player client)
    {
        if (_dicChars.ContainsKey(client))
        {
            var character = _dicChars[client];
            character.Dash();
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

    public override void OnPlayerLeftRoom(Player player) // Destruimos el player aca en vez de en el controller cuando se desconecta
    {
        PhotonNetwork.Destroy(_dicChars[player].gameObject);
    }
}
