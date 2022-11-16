using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;

public class PlayerEntity : NetworkBehaviour
{
    //Network Variables
    public NetworkVariable<FixedString128Bytes> playerName = new NetworkVariable<FixedString128Bytes>("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> helmetSelection = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> team = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> clientId = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public GameObject playerprefab;




    // Start is called before the first frame update
    void Start()
    {
        PlayerSpawnServerRpc();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (IsOwner)
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                //playerprefab.SpawnWithOwnership(OwnerClientId);

                // gameObject player = Instantiate()

                PlayerSpawnServerRpc();

            }
        }
        
        
            
    }

    [ServerRpc]
    public void PlayerSpawnServerRpc()
    {
        GameObject player = Instantiate(playerprefab);
        player.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
        player.GetComponent<PlayerController>().playerName.Value = playerName.Value;
        player.GetComponent<PlayerController>().team.Value = team.Value;
        player.GetComponent<PlayerController>().helmetSelection.Value = helmetSelection.Value;


    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            playerName.Value = PlayerPrefs.GetString("PlayerName");

            //Helmet Assignment
            helmetSelection.Value = PlayerPrefs.GetInt("Helmet");

            clientId.Value = (int)OwnerClientId;
        }
    }


}
