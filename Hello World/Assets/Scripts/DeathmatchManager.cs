using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class DeathmatchManager : NetworkBehaviour
{
    //Network Variables
    public NetworkVariable<int> winKillCount = new NetworkVariable<int>(10, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> team1KillCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> team2KillCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> team1Win = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> team2Win = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> matchOver = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public TextMeshProUGUI team1KillText;
    public TextMeshProUGUI team2KillText;
    public GameObject team1WinMessage;
    public GameObject team2WinMessage;



    // Start is called before the first frame update
    void Start()
    {
        if(IsServer)
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        if(IsServer)
        {
            if (winKillCount.Value == team1KillCount.Value)
            {
                team1Win.Value = true;
                matchOver.Value = true;
                Debug.Log("Team 1 has Won");
                team1WinMessage.SetActive(true);
                Invoke("MatchReset",4);

            }
            else if (winKillCount.Value == team2KillCount.Value)
            {
                team2Win.Value = true;
                matchOver.Value = true;
                Debug.Log("Team 2 has Won");
                team2WinMessage.SetActive(true);
                Invoke("MatchReset", 4);
            }
        }
       
        team1KillText.SetText("Team 1 Kills: " + team1KillCount.Value);
        team2KillText.SetText("Team 2 Kills: " + team2KillCount.Value);
        
    }

    private void MatchReset()
    {
        team1KillCount.Value = 0;
        team2KillCount.Value = 0;
        team1Win.Value = false;
        team2Win.Value = false;
        matchOver.Value = false;
        team1WinMessage.SetActive(false);
        team2WinMessage.SetActive(false);

    }


    [ServerRpc]
    public void PlayerKilledServerRpc(int team)
    {
        if(team == 1)
        {
            team2KillCount.Value += 1;
            Debug.Log("Team2 has " + team2KillCount.Value + " Kills");
        }
        else if(team == 2)
        {
            team1KillCount.Value += 1;
            Debug.Log("Team1 has " + team1KillCount.Value + " Kills");
        }
    }
}
