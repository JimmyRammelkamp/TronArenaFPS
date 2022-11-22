using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniLaser : MonoBehaviour
{
    LineRenderer LR;

    [SerializeField]
    private GameObject convergencePoint;

    private Vector3 CP;

    // Start is called before the first frame update
    void Start()
    {
        LR = GetComponent<LineRenderer>();
        CP = convergencePoint.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        LR.SetPosition(0, transform.position);
        LR.SetPosition(1, CP);
    }
}
