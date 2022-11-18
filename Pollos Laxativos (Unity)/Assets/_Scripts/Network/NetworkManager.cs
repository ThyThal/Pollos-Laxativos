using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;   

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private string _gameScene = "Game";
    [SerializeField] private TMP_Text _statusText;
    [SerializeField] private MenuManager _menuManager;

    [Header("Network Settings")]
    [SerializeField] private Button _retryConnection;
    [SerializeField] private Button _joinGame;

    [Header("Connection Colors")]
    [SerializeField] private Image _connectionImage;
    [SerializeField] private Color _connectedColor;
    [SerializeField] private Color _disconnectedColor;

    private void Start()
    {
        ConnectToPhotonMaster();
    }

    /// <summary>
    /// Connect to Photon Master.
    /// </summary>
    public void ConnectToPhotonMaster()
    {
        Debug.Log("[Photon]: Connecting to Photon Master...");
        _statusText.text = "Connecting to Photon Master...";
        _retryConnection.gameObject.GetComponentInChildren<TMP_Text>().text = "Connecting to Photon Master...";

        PhotonNetwork.ConnectUsingSettings();
        InteractableReconnectButton(false);
    }

    #region Photon Generic Callbacks
    /// <summary>
    /// Connected to Photon Master Server.
    /// </summary>
    public override void OnConnectedToMaster()
    {
        Debug.Log("[Photon]: Connecting to Lobby...");
        _statusText.text = "Status: Connecting to Lobby...";

        // Disable Interactable Settings.
        _menuManager.InteractableRoomSettings(false);

        // Changes Enabled Button.
        ConnectedToMaster(true);

        // Photon Join Lobby.
        PhotonNetwork.JoinLobby();
        
    }

    /// <summary>
    /// Photon Disconnected from Master.
    /// </summary>
    /// <param name="cause"></param>
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("[Photon]: Unable to Connect to Master.");
        _statusText.text = "Status: Unable to Connect to Master.";
        _retryConnection.gameObject.GetComponentInChildren<TMP_Text>().text = "Retry Connection";

        _menuManager.InteractableRoomSettings(false);
        ConnectedToMaster(false);
    }

    /// <summary>
    /// Photon Try to Connect to Game.
    /// </summary>
    public void Connect()
    {
        Debug.Log("[Photon]: Connecting to Game...");

        PhotonNetwork.LocalPlayer.NickName = _menuManager.GetUsername();

        // Disables Room Settings and Game Button.
        _menuManager.InteractableRoomSettings(false);
        InteractableGameButton(false);

        // Creates Room Settings
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = _menuManager.MaxPlayer;
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;

        // Try to Join or Create Room.
        PhotonNetwork.JoinOrCreateRoom(_menuManager.RoomName, roomOptions, TypedLobby.Default);
    }
    #endregion

    #region Photon Lobby Callbacks
    /// <summary>
    /// Connected to Game Lobby.
    /// </summary>
    public override void OnJoinedLobby()
    {
        Debug.Log("[Photon]: Connected to Lobby.");
        _statusText.text = "Status: Connected to Lobby.";

        ValidateRoomSettings();
        _menuManager.InteractableRoomSettings(true);        
    }

    /// <summary>
    /// Disconnected from Game Lobby.
    /// </summary>
    public override void OnLeftLobby()
    {
        Debug.Log("[Photon]: Left Lobby.");
        _statusText.text = "Status: Left Lobby.";
    }
    #endregion 

    #region Photon Game Room Callbacks
    /// <summary>
    /// Photon Created Room.
    /// </summary>
    public override void OnCreatedRoom()
    {
        Debug.Log("[Photon]: Created Room.");
        _statusText.text = "Status: Created Room.";

        // Disable Interactable Settings and Buttons.
        _menuManager.InteractableRoomSettings(false);
        InteractableGameButton(false);
    }

    /// <summary>
    /// Photon Failed to Create Room.
    /// </summary>
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("[Photon]: Failed to Create Room.");
        _statusText.text = "Status: Failed to Create Room.";

        // Enables Room Settings and Game Button.
        _menuManager.InteractableRoomSettings(true);
    }

    /// <summary>
    /// Photon Joined Room.
    /// </summary>
    public override void OnJoinedRoom()
    {
        Debug.Log("[Photon]: Failed to Create Room.");
        _statusText.text = "Status: Joined Room.";

        // Disable Interactable Settings and Buttons.
        _menuManager.InteractableRoomSettings(false);
        InteractableGameButton(false);

        // Loads Game Scene
        PhotonNetwork.LoadLevel(_gameScene);
    }

    /// <summary>
    /// Photon Failed to Join Room.
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("[Photon]: Failed to Join Room.");
        _statusText.text = "Status: Failed to Join Room.";

        // Enables Room Settings and Game Button.
        _menuManager.InteractableRoomSettings(true);
    }
    #endregion

    /// <summary>
    /// Change the availability of Retry Connection & Join.
    /// </summary>
    private void ConnectedToMaster(bool connectedToMaster)
    {
        if (connectedToMaster)
        {
            _connectionImage.color = _connectedColor;
            _retryConnection.gameObject.SetActive(false);
            _joinGame.gameObject.SetActive(true);
        }

        else
        {
            _connectionImage.color = _disconnectedColor;
            _joinGame.gameObject.SetActive(false);
            _retryConnection.gameObject.SetActive(true);
            InteractableReconnectButton(true);
        }
    }

    /// <summary>
    /// Enables the Interaction with Join Game if Settings are Passed.
    /// </summary>
    public void ValidateRoomSettings()
    {
        if (_menuManager.RoomName != "" && _menuManager.Username != "")
        {
            InteractableGameButton(true);
        }

        else
        {
            InteractableGameButton(false);
        }
    }

    private void InteractableReconnectButton(bool enable)
    {
        if (enable) _retryConnection.interactable = true;
        else _retryConnection.interactable = false;
    }
    private void InteractableGameButton(bool enable)
    {
        if (enable) _joinGame.interactable = true;
        else _joinGame.interactable = false;
    }
}
