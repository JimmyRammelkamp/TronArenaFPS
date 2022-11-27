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
    public GameObject waitingForPlayerPannel, playerDataPannel;

    public GameObject playerEntityOBJ;

    //player data
    public TMP_Text playerName;
    public Image playerCardBackground;
    public Toggle isPlayerReady;
}
