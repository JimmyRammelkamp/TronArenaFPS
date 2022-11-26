using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MegaLaser : NetworkBehaviour
{
    LineRenderer LR;

    HelloWorldManager worldManager;

    // Start is called before the first frame update
    void Start()
    {
        LR = GetComponent<LineRenderer>();

        worldManager = FindObjectOfType<HelloWorldManager>();

    }

    // Update is called once per frame
    void Update()
    {
        LR.SetPosition(0, transform.position);
        LR.SetPosition(1, transform.position + transform.up * 100);
    }

    private void OnTriggerEnter(Collider other)
    {
        worldManager.DamagePlayerClientRpc(other.GetComponent<PlayerController>(), 1000);
    }

}
