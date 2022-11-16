using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourController : MonoBehaviour
{
    //Mesh Renderers
    [Header("Default Meshes")]
    public Renderer bodyRenderer;
    public Renderer railgunRenderer;

    [Header("Customisable Meshes")]
    public Renderer helmet1Renderer;
    public Renderer helmet2Renderer;
    public Renderer helmet3Renderer;
    public Renderer helmet4Renderer;

    PlayerController playerController;

    //HDR Colours
    [Header("Team Colours")]
    [ColorUsage(true, true)]
    public Color team1Color = Color.blue;
    [ColorUsage(true, true)]
    public Color team2Color = Color.red;
    [ColorUsage(true, true)]
    public Color defaultColor = Color.white;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerController.team.Value == 1)
        {
            bodyRenderer.material.SetColor("_EmissionColor", team1Color);
            railgunRenderer.material.SetColor("_EmissionColor", team1Color);
            helmet2Renderer.material.SetColor("_EmissionColor", team1Color);
            helmet4Renderer.material.SetColor("_EmissionColor", team1Color);
            helmet3Renderer.material.SetColor("_EmissionColor", team1Color);
            helmet4Renderer.material.SetColor("_EmissionColor", team1Color);

        }
        else if (playerController.team.Value == 2)
        {
            bodyRenderer.material.SetColor("_EmissionColor", team2Color);
            railgunRenderer.material.SetColor("_EmissionColor", team2Color);
            helmet2Renderer.material.SetColor("_EmissionColor", team2Color);
            helmet4Renderer.material.SetColor("_EmissionColor", team2Color);
            helmet3Renderer.material.SetColor("_EmissionColor", team2Color);
            helmet4Renderer.material.SetColor("_EmissionColor", team2Color);
        }
        else
        {
            bodyRenderer.material.SetColor("_EmissionColor", defaultColor);
            railgunRenderer.material.SetColor("_EmissionColor", defaultColor);
            helmet2Renderer.material.SetColor("_EmissionColor", defaultColor);
            helmet4Renderer.material.SetColor("_EmissionColor", defaultColor);
            helmet3Renderer.material.SetColor("_EmissionColor", defaultColor);
            helmet4Renderer.material.SetColor("_EmissionColor", defaultColor);
        }

    //https://answers.unity.com/questions/1206632/trigger-event-on-variable-change.html
    }
}
