using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PylonBehaviour : MonoBehaviour
{
    [SerializeField]
    private float maxDuration = 2;
    private float remainingDuration = 2;

    private int animIDDespawn;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animIDDespawn = Animator.StringToHash("Despawning");

        remainingDuration = maxDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if(remainingDuration <= 0)
        {
            animator.SetBool(animIDDespawn, true);
            Invoke("DestroySelf", 0.6f);
        }
        else
        {
            remainingDuration -= 1 * Time.deltaTime;
        }
    }

    private void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}
