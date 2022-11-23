using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using Photon.Chat;
using Photon.Pun;
using TMPro;
using UnityEngine.AI;

public class Chaterino : MonoBehaviour, IChatClientListener
{
    [SerializeField] private ChatClient _chatClient;
    [SerializeField] private TextMeshProUGUI _chatContent;
    [SerializeField] private TMP_InputField _chatInput;
    [SerializeField] private string _channel;
    [SerializeField] private string _command = "/w";

    private void Start()
    {
        _chatClient = new ChatClient(this);
        var appID = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat;
        var appVersion = PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion;
        var auth = new AuthenticationValues(PhotonNetwork.NickName);
        _chatClient.Connect(appID, appVersion, auth);
    }

    private void Update()
    {
        _chatClient.Service();
    }

    public void DebugReturn(DebugLevel level, string message)
    {
    }

    public void OnDisconnected()
    {
        _chatContent.text += $"Could not Connect to {_channel}.\n";
    }

    public void OnConnected()
    {
        _channel = PhotonNetwork.CurrentRoom.Name;
        _chatClient.Subscribe(_channel);
        _chatContent.text += $"Connected to {_channel}.\n";
    }

    public void OnChatStateChange(ChatState state)
    {
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < senders.Length; i++)
        {
            var currentUsername = senders[i];
            var currentMessage = messages[i];
            
            string color;
            if(PhotonNetwork.NickName == currentUsername) { color = "<color=yellow>"; }
            else { color = "<color=orange>"; }

            _chatContent.text += $"{color}{currentUsername}:</color> {currentMessage}\n";
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        _chatContent.text += $"<color=pink>{sender}:</color> {message}\n";
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
    }

    public void OnUnsubscribed(string[] channels)
    {
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
    }

    public void OnUserSubscribed(string channel, string user)
    {
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
    }

    public void SendChatMessage()
    {
        var message = _chatInput.text;
        
        if(string.IsNullOrEmpty(message) || string.IsNullOrWhiteSpace(message)) return;;

        // Splits Input Text.
        string[] messageWords = message.Split(' ');

        // Sending a Command.
        if (messageWords.Length > 2 && messageWords[0] == _command)
        {
            var target = messageWords[1];

            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (target == player.NickName)
                {
                    // Gets Message to Whisper.
                    var currentMessage = string.Join(" ", messageWords, 2, messageWords.Length - 2);
                     _chatClient.SendPrivateMessage(target, currentMessage);
                    return; 
                }
            }
        }

        else
        {
            _chatClient.PublishMessage(_channel, message);
        }
    }
}
