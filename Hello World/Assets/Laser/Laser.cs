using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private LineRenderer laserLR;
    [Tooltip("Reference to self for destroy function")]
    public GameObject self;

    // Start is called before the first frame update
    void Start()
    {
        laserLR = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        laserLR.SetPosition(0, transform.position);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            if (hit.collider)
            {
                laserLR.SetPosition(1, hit.point);
            }
        }
        else laserLR.SetPosition(1, transform.forward * 5000);

        // Execute destroy self after delay
        Invoke("DisableSelf", 0.4f);
    }

    // Destroy Self function
    private void DisableSelf()
    {
        Destroy(self);
    }
}
