using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerHud : NetworkBehaviour
{
    
    //code from https://github.com/dilmerv/UnityMultiplayerPlayground
    [SerializeField]
    private NetworkVariable<NetworkString> playerNetworkName = new NetworkVariable<NetworkString>();

    private bool overlaySet = false;

    public override void OnNetworkSpawn()
    {
        //if (IsServer)
        {
           // playerNetworkName.Value = PlayerPrefs.GetString("playerName"); ;
            playerNetworkName.Value = $"Player {OwnerClientId}";
        }
    }

    public void SetOverlay()
    {
        var localPlayerOverlay = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        localPlayerOverlay.text = $"{playerNetworkName.Value}";
    }

    public void Update()
    {
        if (!overlaySet && !string.IsNullOrEmpty(playerNetworkName.Value))
        {
            SetOverlay();
            overlaySet = true;
        }
    }
}
