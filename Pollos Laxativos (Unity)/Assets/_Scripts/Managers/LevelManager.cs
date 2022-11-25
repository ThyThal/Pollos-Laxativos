using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelManager : MonoBehaviourPunCallbacks
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

    [SerializeField] private Transform _arena;
    [SerializeField] private float _shrinkSoftness = 10f;

    [SerializeField] private bool _gameStarted = false;


    private bool _ended = false;
    private bool _starting = false;

    public float currentTime;
    public bool Starting => _starting;
    public bool GameStarted => _gameStarted;

    [SerializeField] private List<Player> playersList = new List<Player>();

    private void Awake()
    {
        //if (!PhotonNetwork.IsMasterClient) Destroy(this);

        _playerSpawns = new Dictionary<byte, List<Transform>>();
        _playerSpawns.Add(3, _3playersSpawns);
        _playerSpawns.Add(4, _4playersSpawns);
        _playerSpawns.Add(5, _5playersSpawns);
    }

    private void Start()
    {
        GameManager.Instance.LevelManager = this;
        _roomName.text = $"Room Name: {PhotonNetwork.CurrentRoom.Name.ToString()}";
        _startingText.text = $"Waiting for Players {PhotonNetwork.CurrentRoom.PlayerCount - 1}/{PhotonNetwork.CurrentRoom.MaxPlayers - 1}";

        if(!PhotonNetwork.IsMasterClient) MasterManager.Instance.RPCMaster("RequestConnectPlayer", PhotonNetwork.LocalPlayer);

        currentTime = 0f;

    }

    private void Update()
    {
        if (_gameStarted)
        {
            currentTime += Time.deltaTime;
            if (!PhotonNetwork.IsMasterClient) return;
            _arena.localScale = Vector2.Lerp(_arena.localScale, new Vector2(2f, 2f), Time.deltaTime / _shrinkSoftness);
        }
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
            if (_playerSpawns.TryGetValue((byte)(PhotonNetwork.CurrentRoom.MaxPlayers - 1), out spawns))
            {
                var playerNumber = PhotonNetwork.CurrentRoom.PlayerCount - 1;
                Debug.Log($"[Level Manager]: Spawning Player with ID {playerNumber}.");

                // Instantiate Looking at Centre.

                var player = PhotonNetwork.Instantiate("PlayerFA", spawns[playerNumber - 1].position, Quaternion.identity);
                player.transform.right = (-spawns[playerNumber - 1].position).normalized;

                player.GetComponent<PlayerModel>().loseAction += OnPlayerDied;
                player.GetComponent<PlayerModel>().winAction += WinScreen;

                return player.GetComponent<PlayerModel>();
            }
        }

        return null;
        
    }
    public void SpawnProjectile(PlayerModel owner)    // Cambi� el owner a PlayerModel porque el photonview siempre va a ser el del master
    {
        var projectile = PhotonNetwork.Instantiate("ProjectileFA", owner.transform.position, owner.transform.rotation * Quaternion.Euler(0f, 0f, 180f));
        projectile.GetComponent<ProjectileController>().Initialize(owner);
    }

    // Photon
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (_gameStarted) return;
            
            int playerCount = PhotonNetwork.CurrentRoom.PlayerCount - 1;

            if (playerCount >= PhotonNetwork.CurrentRoom.MaxPlayers - 1)
            {
                Debug.LogWarning("[Level Manager]: Starting Countdown.");
                StartCoroutine(Countdown());
            }
        }

        _startingText.text = $"Waiting for Players {PhotonNetwork.CurrentRoom.PlayerCount - 1}/{PhotonNetwork.CurrentRoom.MaxPlayers - 1}";
    }

    private IEnumerator Countdown()
    {
        _starting = true;
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
            PlayerModel model = player.GetComponent<PlayerModel>();
            //PhotonView playerOwner = player.GetPhotonView();
            //playersList.Add(playerOwner.Owner);
            //photonView.RPC("AddPlayers", RpcTarget.Others, playerOwner.Owner);
            playersList.Add(MasterManager.Instance.GetClientFromModel(model));
            photonView.RPC("AddPlayers", RpcTarget.Others, MasterManager.Instance.GetClientFromModel(model));
        }

        PhotonNetwork.CurrentRoom.IsOpen = false;
        photonView.RPC("RunningGame", RpcTarget.All, true);
        StartTimeSync();

        yield return null;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount > 0)
        {
            _startingText.text = $"Waiting for Players {PhotonNetwork.CurrentRoom.PlayerCount-1}/{PhotonNetwork.CurrentRoom.MaxPlayers-1}";
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("RemovePlayers", RpcTarget.All, otherPlayer);
            }
            //photonView.RPC("RemovePlayers", RpcTarget.All, MasterManager.Instance.);
            
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        CloseRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            CloseRoom();
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
        playersList.Remove(player);
    }

    /// <summary>
    /// Gets Called When Player Leaves the Arena.
    /// </summary>
    public void OnPlayerDied(Player player)
    {
        //if (!_ended)
        //{
            //_ended = true;

              // Activates Lose Screen to dead player.
              if (player.IsMasterClient) LoseScreen();
              else photonView.RPC("LoseScreen", player);
            
              //_looseScreen.SetActive(true);
              //playersList.Remove(player);

              // Tells Other Clients to Check Win Condition. removing him from Players.
              photonView.RPC("WinScreen", RpcTarget.All, player);
        //}
    }

    [PunRPC]
    public void WinScreen(Player looser)
    {
        // Removes Lost Player.
        playersList.Remove(looser);

        if (_ended) return;

        if (playersList.Count == 1 && PhotonNetwork.LocalPlayer != looser)
        {
            // Si es el master que muestre otra cosa, o si queres sacamos la camara
            _winScreen.SetActive(true);
            photonView.RPC("RunningGame", RpcTarget.All, false);
            Debug.Log(playersList[0].NickName);
        }
    }

    
    [PunRPC]
    public void LoseScreen()
    {
        _ended = true;
        _looseScreen.SetActive(true);
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

    public void StartTimeSync()
    {
        InvokeRepeating("UpdateTimeSync", 0f, 5f);
    }

    void UpdateTimeSync()
    {
        photonView.RPC("SyncTime", RpcTarget.Others, currentTime);
        Debug.Log("Time Sync!");
    }

    [PunRPC]
    public void SyncTime(float time)
    {
        currentTime = time;
    }

    public void CloseRoom()
    {
        photonView.RPC("QuitRoom", RpcTarget.All);
    }

    [PunRPC]
    public void QuitRoom()
    {
        PhotonNetwork.LoadLevel("Loader");
        PhotonNetwork.LeaveRoom();
    }
}
