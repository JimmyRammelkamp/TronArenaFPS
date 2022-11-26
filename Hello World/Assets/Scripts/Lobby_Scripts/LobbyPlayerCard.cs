using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

// code from tutorial - https://www.youtube.com/watch?v=sBR0oJJjx6Q and https://www.youtube.com/watch?v=PnQutPyMZhI

public class LobbyPlayerCard : NetworkBehaviour
{
    //pannels
    [SerializeField] private GameObject waitingForPlayerPannel, playerDataPannel;

    //player data
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private Toggle isPlayerReady;
}
