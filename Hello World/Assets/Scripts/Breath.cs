using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breath : MonoBehaviour
{
    private bool isBreathing = true;

    public float minHeight = 1.4f;
    public float maxHeight = 1.48f;

    [Range(1f, 3f)]
    public float breathSpeed = 1f;

    private float movement;

    private void Start()
    {
        movement = transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (isBreathing)
        {
            // Lerp up and flip once near top
            movement = Mathf.Lerp(movement, maxHeight, Time.deltaTime * 0.5f * breathSpeed);
            transform.localPosition = new Vector3(transform.localPosition.x, movement, transform.localPosition.z);
            if (movement >= maxHeight - 0.01f) isBreathing = !isBreathing;        
        }
        else
        {
            // Lerp down and flip once near bottom
            movement = Mathf.Lerp(movement, minHeight, Time.deltaTime * 0.5f * breathSpeed);
            transform.localPosition = new Vector3(transform.localPosition.x, movement, transform.localPosition.z);
            if (movement <= minHeight + 0.01f) isBreathing = !isBreathing;
        }
    }
}
