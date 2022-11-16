using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisingWall : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Total health received on spawn")]
    private float maxHealth = 30;

    private float currentHealth = 30;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentHealth <= 0)
        {
            // Execute destroy self after delay
            Invoke("DisableSelf", 0.4f);
        }
    }

    public void takeDamage(float incDamage)
    {
        currentHealth -= incDamage;
    }
    public float getHealth()
    {
        return currentHealth;
    }

    private void DisableSelf()
    {
        Destroy(this.gameObject);
    }
}
