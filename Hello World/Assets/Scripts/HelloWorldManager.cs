
using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class HelloWorldManager : MonoBehaviour
{
    [SerializeField] private Text status, message;
    [SerializeField] private Button buttonHost, buttonPlayClient, buttonServer, buttonShutdown;
    [SerializeField] private InputField IPAddress, port, playerName;



    private void Start()
    {
        status.text = "NOT CONNECTED";

        HandleUI(false);

        IPAddress.text = PlayerPrefs.GetString("IPaddress");
        if (IPAddress.text == "") IPAddress.text = ((UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport).ConnectionData.Address;
        port.text = PlayerPrefs.GetString("port");
        if (port.text == "") port.text = ((UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport).ConnectionData.Port.ToString();
        playerName.text = PlayerPrefs.GetString("PlayerName");
        

        buttonHost.onClick.AddListener(StartHost);
        buttonPlayClient.onClick.AddListener(StartClient);
        buttonServer.onClick.AddListener(StartServer);
        buttonShutdown.onClick.AddListener(Shutdown);

        NetworkManager.Singleton.OnServerStarted += HandleSeverStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        // NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;
    }

    public void StartHost()
    {
        Int16 p;
        if (!Int16.TryParse(port.text, out p)) return;
        ((UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport).SetConnectionData(IPAddress.text, (ushort)p);
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        Int16 p;
        if (!Int16.TryParse(port.text, out p)) return;
        ((UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport).SetConnectionData(IPAddress.text, (ushort)p);
        NetworkManager.Singleton.StartClient();
    }

    public void StartServer()
    {
        Int16 p;
        if (!Int16.TryParse(port.text, out p)) return;
        ((UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport).SetConnectionData(IPAddress.text, (ushort)p);
        NetworkManager.Singleton.StartServer();
    }

    public void Shutdown()
    {
        NetworkManager.Singleton.Shutdown();
        status.text = "NOT CONNECTED";
        HandleUI(false);
    }

    private void HandleSeverStarted()
    {
        status.text = NetworkManager.Singleton.IsHost ? "Connected as Host" : "Connected as Server";

        HandleUI(true);

        PlayerPrefs.SetString("IPaddress", IPAddress.text);
        PlayerPrefs.SetString("port", port.text);
        PlayerPrefs.SetString("PlayerName", playerName.text);
    }

    private void HandleClientConnected(ulong clientId)
    {
        status.text = NetworkManager.Singleton.IsHost ? "Connected as Host" : "Connected as Client";

        HandleUI(true);


        PlayerPrefs.SetString("IPaddress", IPAddress.text);
        PlayerPrefs.SetString("port", port.text);
        PlayerPrefs.SetString("PlayerName", playerName.text);

        //NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().SetNickname(playerName.text);
    }

    private void HandleUI(bool isConnected)
    {
        buttonHost.gameObject.SetActive(!isConnected);
        buttonPlayClient.gameObject.SetActive(!isConnected);
        buttonServer.gameObject.SetActive(!isConnected);
        IPAddress.gameObject.SetActive(!isConnected);
        port.gameObject.SetActive(!isConnected);
        playerName.gameObject.SetActive(!isConnected);
        buttonShutdown.gameObject.SetActive(isConnected);
    }

    void Update()
    {

    }

    [ClientRpc]
    public void DamagePlayerClientRpc(PlayerController player, int damage)
    {
        player.Hit();
        player.playerHealth.Value -= damage;
        Debug.Log(player.playerName.Value + " has been hit for " + damage + " damage");
        Debug.Log(player.playerName.Value + " is at " + player.playerHealth.Value + " health");
        if (player.playerHealth.Value <= 0)
        {
            player.playerHealth.Value = 0;
            player.playerIsDead.Value = true;
            Debug.Log(player.playerName.Value + " is Dead");
            player.PlayerDead();
        }
    }

    [ClientRpc]
    public void DamageWallClientRpc(RisingWall wall, int damage)
    {
        wall.takeDamage(damage);
        Debug.Log("Wall has been hit for " + damage + " damage");
        Debug.Log("Wall is at " + wall.getHealth() + " health");
    }


   

    //private void Start()
    //{
    //    IPaddress.text = PlayerPrefs.GetString("IPaddress");
    //    if (IPaddress.text == "") IPaddress.text = ((UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport).ConnectionData.Address;
    //    port.text = PlayerPrefs.GetString("port");
    //    if (port.text == "") port.text = ((UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport).ConnectionData.Port.ToString();
    //    playerName.text = PlayerPrefs.GetString("playerName");
    //    if (playerName.text == "") playerName.text = "Name";
    //}

    //public void StartHost()
    //{
    //    Int16 p;
    //    if (!Int16.TryParse(port.text, out p)) return;
    //    ((UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport).SetConnectionData(IPaddress.text, (ushort)p);
    //    NetworkManager.Singleton.StartHost();
    //    PlayerPrefs.SetString("IPaddress", IPaddress.text);
    //    PlayerPrefs.SetString("port", port.text);
    //    PlayerPrefs.SetString("playerName", playerName.text);
    //}

    //public void StartClient()
    //{
    //    Int16 p;
    //    if (!Int16.TryParse(port.text, out p)) return;
    //    ((UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport).SetConnectionData(IPaddress.text, (ushort)p);
    //    NetworkManager.Singleton.StartClient();
    //    PlayerPrefs.SetString("IPaddress", IPaddress.text);
    //    PlayerPrefs.SetString("port", port.text);
    //    PlayerPrefs.SetString("playerName", playerName.text);
    //}

    //public void StartServer()
    //{
    //    Int16 p;
    //    if (!Int16.TryParse(port.text, out p)) return;
    //    ((UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport).SetConnectionData(IPaddress.text, (ushort)p);
    //    NetworkManager.Singleton.StartServer();
    //    PlayerPrefs.SetString("IPaddress", IPaddress.text);
    //    PlayerPrefs.SetString("port", port.text);
    //    PlayerPrefs.SetString("playerName", playerName.text);
    //}

    //public void Shutdown()
    //{
    //    NetworkManager.Singleton.Shutdown();
    //}

    //void Update()
    //{
    //    if (NetworkManager.Singleton.IsHost) status.text = "Connected as Host";
    //    else if (NetworkManager.Singleton.IsClient) status.text = "Connected as Client";
    //    else if (NetworkManager.Singleton.IsServer) status.text = "Connected as Server";
    //    else status.text = "NOT CONNECTED";

    //    //if (NetworkManager.Singleton.IsHost) message.text = PlayerPrefs.GetString("playerName");

    //    bool bActive = NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer;
    //    buttonHost.gameObject.SetActive(!bActive);
    //    buttonPlay.gameObject.SetActive(!bActive);
    //    buttonServer.gameObject.SetActive(!bActive);
    //    IPaddress.gameObject.SetActive(!bActive);
    //    port.gameObject.SetActive(!bActive);
    //    playerName.gameObject.SetActive(!bActive);
    //    buttonShutdown.gameObject.SetActive(bActive);
    //}
}
