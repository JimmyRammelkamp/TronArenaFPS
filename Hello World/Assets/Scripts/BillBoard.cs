using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    //float nameDistance;
    //Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        //camera = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //if (camera == null) camera = Camera.current;

        //nameDistance = (transform.position - camera.transform.position ).magnitude;

        //RaycastHit hit;

        //if (Physics.Raycast(camera.transform.position, transform.position - camera.transform.position, out hit, nameDistance))
        //{
        //    if(hit.distance < nameDistance)
        //    {
        //        transform.GetComponent<MeshRenderer>().enabled = false;
        //    }    
        //    else
        //    {
        //        transform.GetComponent<MeshRenderer>().enabled = true;
        //        transform.LookAt(transform.position + Camera.current.transform.forward);
        //    }
        //}

        //transform.LookAt(transform.position + Camera.main.transform.forward);

        //https://www.youtube.com/watch?v=-CopHN3f0vg
    }
}
