using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelManagerFullAuth : MonoBehaviourPunCallbacks
{
    [SerializeField] private List<Transform> _3playersSpawns;
    [SerializeField] private List<Transform> _4playersSpawns;
    [SerializeField] private List<Transform> _5playersSpawns;

    [SerializeField] private GameObject _winScreen;
    [SerializeField] private GameObject _looseScreen;

    [SerializeField] private TMP_Text _roomName;
    [SerializeField] private TMP_Text _ping;

    [SerializeField] private GameObject _startingScreen;
    [SerializeField] private TMP_Text _startingText;

    [SerializeField] private Dictionary<byte, List<Transform>> _playerSpawns;

    [SerializeField] private bool _gameStarted = false;

    private bool _ended = false;

    public bool GameStarted => _gameStarted;

    [SerializeField] private List<Player> playersList = new List<Player>();

    private void Awake()
    {
        _playerSpawns = new Dictionary<byte, List<Transform>>();
        _playerSpawns.Add(3, _3playersSpawns);
        _playerSpawns.Add(4, _4playersSpawns);
        _playerSpawns.Add(5, _5playersSpawns);

        //Spawn();
    }

    private void Start()
    {
        GameManagerFullAuth.Instance.LevelManager = this;
        _roomName.text = $"Room Name: {PhotonNetwork.CurrentRoom.Name.ToString()}";
        _startingText.text = $"Waiting for Players {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
        
        MasterManager.Instance.RPCMaster("RequestConnectPlayer", PhotonNetwork.LocalPlayer);
        }

    public PlayerModel SpawnPlayer()    // Hice que devuelva el playermodel que spawnea para que quede en el mastermanager, no se si funciona(?
    {
        if (PhotonNetwork.CurrentRoom == null) return null;
        if (GameStarted) return null;

        // Instantiate Player.
        if (_playerSpawns.ContainsKey(PhotonNetwork.CurrentRoom.MaxPlayers))
        {
            Debug.Log("[Level Manager]: Found Spawns List.");

            // Get Available Spawns.
            List<Transform> spawns;
            if (_playerSpawns.TryGetValue(PhotonNetwork.CurrentRoom.MaxPlayers, out spawns))
            {
                var playerNumber = PhotonNetwork.CurrentRoom.PlayerCount;
                Debug.Log($"[Level Manager]: Spawning Player with ID {playerNumber}.");

                // Instantiate Looking at Centre.

                var player = PhotonNetwork.Instantiate("PlayerFA", spawns[playerNumber - 1].position, Quaternion.identity);
                player.transform.right = (-spawns[playerNumber - 1].position).normalized;

                player.GetComponent<PlayerModel>().loseAction += LoseScreen;
                player.GetComponent<PlayerModel>().winAction += WinScreen;

                return player.GetComponent<PlayerModel>();
            }
        }

        return null;
        
    }
    public void SpawnProjectile(PlayerModel owner)    // Cambi� el owner a PlayerModel porque el photonview siempre va a ser el del master
    {
        var projectile = PhotonNetwork.Instantiate("ProjectileFA", owner.transform.position, owner.transform.rotation * Quaternion.Euler(0f, 0f, 180f));
        projectile.GetComponent<ProjectileFullAuth>().Initialize(owner);
    }

    // Photon
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (_gameStarted) return;

            

            int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

            if (playerCount >= PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                Debug.LogWarning("[Level Manager]: Starting Countdown.");

                StartCoroutine(Countdown());
            }
        }

        _startingText.text = $"Waiting for Players {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
    }

    private IEnumerator Countdown()
    {
        yield return new WaitForSeconds(1);

        Debug.LogWarning("[Level Manager]: 3...");
        photonView.RPC("UpdateCountdown", RpcTarget.All, "Starting in 3...");
        yield return new WaitForSeconds(1);

        Debug.LogWarning("[Level Manager]: 2...");
        photonView.RPC("UpdateCountdown", RpcTarget.All, "Starting in 2...");
        yield return new WaitForSeconds(1);

        Debug.LogWarning("[Level Manager]: 1...");
        photonView.RPC("UpdateCountdown", RpcTarget.All, "Starting in 1...");
        yield return new WaitForSeconds(1);

        Debug.LogWarning("[Level Manager]: Start!");
        photonView.RPC("UpdateCountdown", RpcTarget.All, "Start!");
        yield return new WaitForSeconds(1);

        _gameStarted = true;

        // Get all Players.
        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var player in players)
        {
            PhotonView playerOwner = player.GetPhotonView();
            playersList.Add(playerOwner.Owner);
            photonView.RPC("AddPlayers", RpcTarget.Others, playerOwner.Owner);
        }

        photonView.RPC("RunningGame", RpcTarget.Others, true);

        yield return null;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount > 0)
        {
            _startingText.text = $"Waiting for Players {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
            photonView.RPC("RemovePlayers", RpcTarget.All, otherPlayer);
        }
    }

    [PunRPC]
    public void UpdateCountdown(string status)
    {
        _startingText.text = status;
    }

    [PunRPC]
    public void AddPlayers(Player player)
    {
        playersList.Add(player);
    }

    [PunRPC]
    public void RemovePlayers(Player player)
    {
        playersList.Add(player);
    }

    /// <summary>
    /// Gets Called When Player Leaves the Arena.
    /// </summary>
    public void LoseScreen(Player player)
    {
        if (!_ended)
        {
            _ended = true;
            _looseScreen.SetActive(true);
            playersList.Remove(player);

            // Tells Other Clients to Check Win Condition. removing him from Players.
            photonView.RPC("WinScreen", RpcTarget.Others, player);
        }
    }

    [PunRPC]
    public void WinScreen(Player looser)
    {
        // Removes Lost Player.
        playersList.Remove(looser);

        if (_ended) return;

        if (playersList.Count == 1 && playersList[0] != looser)
        {
            _winScreen.SetActive(true);
            photonView.RPC("RunningGame", RpcTarget.All, false);
        }
    }

    [PunRPC]
    private void RunningGame(bool state)
    {
        _gameStarted = state;
        _startingScreen.SetActive(false);
    }

    public void LoadMenu()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("Loader");
    }
}
