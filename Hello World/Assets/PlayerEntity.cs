using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;

public class PlayerEntity : NetworkBehaviour
{
    //Network Variables
    public NetworkVariable<FixedString128Bytes> playerName = new NetworkVariable<FixedString128Bytes>("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> helmetSelection = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> team = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> clientId = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> activePlayer = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public GameObject playerprefab;

    GameObject player;


    public TextMeshProUGUI button;

    public Button spawnButton;

    // Start is called before the first frame update
    void Start()
    {
        if(IsOwner)
        {
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (IsOwner)
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                if (Cursor.visible == false)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else if (Cursor.visible == true)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }

            if(Input.GetKeyDown(KeyCode.N))
            {
                PlayerSpawnServerRpc();
            }


            if (activePlayer.Value == false)
            {
                spawnButton.gameObject.SetActive(true);
            }
            else if(activePlayer.Value == true)
            {
                spawnButton.gameObject.SetActive(false);
            }
        }
        
        
            
    }

    [ServerRpc]
    public void PlayerSpawnServerRpc()
    {
        Vector3 spawnPos;
        Quaternion spawnRot;

        if (team.Value == 1) //Spawn on Team1 spawn point
        {
            spawnPos = GameObject.FindGameObjectWithTag("Team1Spawn").transform.position;
            spawnRot = GameObject.FindGameObjectWithTag("Team1Spawn").transform.rotation;
            
        }
        else if (team.Value == 2) //Spawn on Team2 spawn point
        {
            spawnPos = GameObject.FindGameObjectWithTag("Team2Spawn").transform.position;
            spawnRot = GameObject.FindGameObjectWithTag("Team2Spawn").transform.rotation;

        }
        else //Spawn Randomly on map
        {
            spawnPos = new Vector3(Random.Range(-11f, -25f), 1f, Random.Range(-30f, 0f));
            spawnRot = new Quaternion(0f, Random.Range(0f, 360f),0f,0f);
        }

        player = Instantiate(playerprefab,spawnPos + new Vector3(Random.Range(-5,5),0,Random.Range(-5,5)),spawnRot);
        player.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
        player.GetComponent<PlayerController>().playerName.Value = playerName.Value;
        player.GetComponent<PlayerController>().team.Value = team.Value;
        player.GetComponent<PlayerController>().helmetSelection.Value = helmetSelection.Value;

        activePlayer.Value = true;

    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            playerName.Value = PlayerPrefs.GetString("PlayerName");


            //Helmet Assignment
            helmetSelection.Value = PlayerPrefs.GetInt("Helmet");

            clientId.Value = (int)OwnerClientId;

            if (clientId.Value % 2 == 0)
            {
                team.Value = 2;

            }
            else
            {
                team.Value = 1;
            }
        }
    }


}
