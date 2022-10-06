using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class TestStringContainer : NetworkBehaviour
{
    [SerializeField]
    public Text message;

    public StringContainer m_StringContainer = new StringContainer();
    public override void OnNetworkSpawn()
    {
       // if (IsServer)
        {
            // m_StringContainer.Text = "This is my test to see if the string is replicated on clients.";
            // m_StringContainer.Text = PlayerPrefs.GetString("playerName");
         //   updateMessage("Host Player " + PlayerPrefs.GetString("playerName") + " joined");
        }
       // else
        {
           // Debug.Log($"Client-Side StringContainer = {m_StringContainer.Text}");
            //message.text = m_StringContainer.Text;
            updateMessage("Client Player " + PlayerPrefs.GetString("playerName") + " joined");
        }
    }

    public void updateMessage(string text)
    {
        Debug.Log("updateMessage");
        m_StringContainer.Text = text;
        message.text = m_StringContainer.Text;
        
    }
}