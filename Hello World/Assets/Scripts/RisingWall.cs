using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class RisingWall : NetworkBehaviour
{
    [SerializeField]
    [Tooltip("Total health received on spawn")]
    private float maxHealth = 30;

    public NetworkVariable<float> currentHealth = new NetworkVariable<float>(30, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


    //private float currentHealth = 30;

    private void Start()
    {
        currentHealth.Value = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentHealth.Value <= 0)
        {
            // Execute destroy self after delay
            Invoke("DisableSelf", 0.4f);
        }
    }

    public void takeDamage(float incDamage)
    {
        currentHealth.Value -= incDamage;
    }
    public float getHealth()
    {
        return currentHealth.Value;
    }

    private void DisableSelf()
    {
        Destroy(this.gameObject);
    }
}
