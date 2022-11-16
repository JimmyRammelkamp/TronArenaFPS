using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisingWall : MonoBehaviour
{
    public float maxHealth = 30;
    private float currentHealth = 30;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(currentHealth <= 0)
        {

        }
    }

    public void takeDamage(float incDamage)
    {
        currentHealth -= incDamage;
    }
}
