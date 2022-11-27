using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : NetworkBehaviour
{
    [SerializeField] private LobbyPlayerCard[] lobbyPlayerCards = new LobbyPlayerCard[8];
    public PlayerEntity[] playerEntities;

    Color team1Colour, team2Colour;

    private void Start()
    {
        team1Colour = Color.blue;
        team2Colour = Color.red;
    }

    private void Update()
    {
        playerEntities = FindObjectsOfType<PlayerEntity>();

        for (int i = 0; i < playerEntities.Length; i++)
        {
            lobbyPlayerCards[i].playerEntityOBJ = playerEntities[i].gameObject;

            lobbyPlayerCards[i].waitingForPlayerPannel.SetActive(false);
            lobbyPlayerCards[i].playerDataPannel.SetActive(true);

            lobbyPlayerCards[i].playerName.text = playerEntities[i].playerName.Value.ToString();
            if (playerEntities[i].team.Value == 1) lobbyPlayerCards[i].playerCardBackground.color = team1Colour;
            if (playerEntities[i].team.Value == 2) lobbyPlayerCards[i].playerCardBackground.color = team2Colour;

        }

        foreach (LobbyPlayerCard playerCards in lobbyPlayerCards)
        {
            if (playerCards.playerEntityOBJ == null)
            {
                playerCards.waitingForPlayerPannel.SetActive(true);
                playerCards.playerDataPannel.SetActive(false);

                playerCards.playerName.text = "Player Name";
                playerCards.playerCardBackground.color = Color.black;
            }
        }
    }
}
