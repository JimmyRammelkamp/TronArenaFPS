using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

// code from tutorial - https://www.youtube.com/watch?v=sBR0oJJjx6Q and https://www.youtube.com/watch?v=PnQutPyMZhI

public class LobbyUI : NetworkBehaviour
{
    [SerializeField] private LobbyPlayerCard[] lobbyPlayerCards;
    [SerializeField] private Button startGameButton;

    //network variables
    [SerializeField] private NetworkList<LobbyPlayerState> lobbyPlayers;

    private void Awake()
    {
        lobbyPlayers = new NetworkList<LobbyPlayerState>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            lobbyPlayers.OnListChanged += HandleLobbyPlayerStateChange;
        }

        if (IsServer)
        {
            startGameButton.gameObject.SetActive(true);

            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;

            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                HandleClientConnected(client.ClientId);
            }
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        
        lobbyPlayers.OnListChanged -= HandleLobbyPlayerStateChange;

        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
        }
    }

    private bool IsEveryoneReady()
    {
        if (lobbyPlayers.Count < 4)
        {
            return false;
        }

        foreach (var player in lobbyPlayers)
        {
            if (!player.IsReady)
            {
                return false;
            }
        }

        return true;
    }

    private void HandleClientConnected(ulong clientID)
    {

    }

    private void HandleClientDisconnected(ulong clientID)
    {
        for (int i = 0; i < lobbyPlayers.Count; i++)
        {
            if (lobbyPlayers[i].ClientID == clientID)
            {
                lobbyPlayers.RemoveAt(i);
                break;
            }
        }
    }

    private void HandleLobbyPlayerStateChange(NetworkListEvent<LobbyPlayerState> changeEvent)
    {
        throw new NotImplementedException();
    }
}
