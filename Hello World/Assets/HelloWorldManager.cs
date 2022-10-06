
using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class HelloWorldManager : MonoBehaviour
{
    [SerializeField] private Text status, message;
    [SerializeField] private Button buttonHost, buttonPlay, buttonServer, buttonShutdown;
    [SerializeField] private InputField IPaddress, port, playerName;

    private void Start()
    {
        IPaddress.text = PlayerPrefs.GetString("IPaddress");
        if (IPaddress.text == "") IPaddress.text = ((UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport).ConnectionData.Address;
        port.text = PlayerPrefs.GetString("port");
        if (port.text == "") port.text = ((UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport).ConnectionData.Port.ToString();
        playerName.text = PlayerPrefs.GetString("playerName");
        if (playerName.text == "") playerName.text = "Name";
    }

    public void StartHost()
    {
        Int16 p;
        if (!Int16.TryParse(port.text, out p)) return;
        ((UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport).SetConnectionData(IPaddress.text, (ushort)p);
        NetworkManager.Singleton.StartHost();
        PlayerPrefs.SetString("IPaddress", IPaddress.text);
        PlayerPrefs.SetString("port", port.text);
        PlayerPrefs.SetString("playerName", playerName.text);
    }

    public void StartClient()
    {
        Int16 p;
        if (!Int16.TryParse(port.text, out p)) return;
        ((UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport).SetConnectionData(IPaddress.text, (ushort)p);
        NetworkManager.Singleton.StartClient();
        PlayerPrefs.SetString("IPaddress", IPaddress.text);
        PlayerPrefs.SetString("port", port.text);
        PlayerPrefs.SetString("playerName", playerName.text);
    }

    public void StartServer()
    {
        Int16 p;
        if (!Int16.TryParse(port.text, out p)) return;
        ((UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport).SetConnectionData(IPaddress.text, (ushort)p);
        NetworkManager.Singleton.StartServer();
        PlayerPrefs.SetString("IPaddress", IPaddress.text);
        PlayerPrefs.SetString("port", port.text);
        PlayerPrefs.SetString("playerName", playerName.text);
    }

    public void Shutdown()
    {
        NetworkManager.Singleton.Shutdown();
    }

    void Update()
    {
        if (NetworkManager.Singleton.IsHost) status.text = "Connected as Host";
        else if (NetworkManager.Singleton.IsClient) status.text = "Connected as Client";
        else if (NetworkManager.Singleton.IsServer) status.text = "Connected as Server";
        else status.text = "NOT CONNECTED";

        //if (NetworkManager.Singleton.IsHost) message.text = PlayerPrefs.GetString("playerName");

        bool bActive = NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer;
        buttonHost.gameObject.SetActive(!bActive);
        buttonPlay.gameObject.SetActive(!bActive);
        buttonServer.gameObject.SetActive(!bActive);
        IPaddress.gameObject.SetActive(!bActive);
        port.gameObject.SetActive(!bActive);
        playerName.gameObject.SetActive(!bActive);
        buttonShutdown.gameObject.SetActive(bActive);
    }
}
