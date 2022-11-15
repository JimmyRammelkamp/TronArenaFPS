using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourController : MonoBehaviour
{
    Renderer renderer;
    PlayerController playerController;

    [ColorUsage(true, true)]
    public Color team1Color = Color.blue;
    [ColorUsage(true, true)]
    public Color team2Color = Color.red;
    [ColorUsage(true, true)]
    public Color defaultColor = Color.white;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
        playerController = GetComponentInParent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerController.team.Value == 1)
        {
            renderer.material.SetColor("_EmissionColor", team1Color);

        }
        else if (playerController.team.Value == 2)
        {
            renderer.material.SetColor("_EmissionColor", team2Color);
        }
        else
        {
            renderer.material.SetColor("_EmissionColor", defaultColor);
        }

    }
}
