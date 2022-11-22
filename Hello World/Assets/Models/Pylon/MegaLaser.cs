using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaLaser : MonoBehaviour
{
    LineRenderer LR;
    // Start is called before the first frame update
    void Start()
    {
        LR = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        LR.SetPosition(0, transform.position);
        LR.SetPosition(1, transform.position + transform.up * 100);
    }
}
