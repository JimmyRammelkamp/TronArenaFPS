using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollBehaviour : MonoBehaviour
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

    //HDR Colours
    [Header("Team Colours")]
    [ColorUsage(true, true)]
    public Color team1Color = Color.blue;
    [ColorUsage(true, true)]
    public Color team2Color = Color.red;
    [ColorUsage(true, true)]
    public Color defaultColor = Color.white;

    private int teamSelection = 0;
    private int helmetSelection = 0;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroySelf", 4);
    }

    void Update()
    {
        if (teamSelection == 1)
        {
            bodyRenderer.material.SetColor("_EmissionColor", team1Color);
            railgunRenderer.material.SetColor("_EmissionColor", team1Color);
            helmet2Renderer.material.SetColor("_EmissionColor", team1Color);
            helmet4Renderer.material.SetColor("_EmissionColor", team1Color);
            helmet3Renderer.material.SetColor("_EmissionColor", team1Color);
            helmet4Renderer.material.SetColor("_EmissionColor", team1Color);

        }
        else if (teamSelection == 2)
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

        if(helmetSelection == 1)
        {
            helmet1Renderer.gameObject.SetActive(true);
            helmet2Renderer.gameObject.SetActive(false);
            helmet3Renderer.gameObject.SetActive(false);
            helmet4Renderer.gameObject.SetActive(false);
        }
        else if (helmetSelection == 2)
        {
            helmet1Renderer.gameObject.SetActive(false);
            helmet2Renderer.gameObject.SetActive(true);
            helmet3Renderer.gameObject.SetActive(false);
            helmet4Renderer.gameObject.SetActive(false);
        }
        else if (helmetSelection == 3)
        {
            helmet1Renderer.gameObject.SetActive(false);
            helmet2Renderer.gameObject.SetActive(false);
            helmet3Renderer.gameObject.SetActive(true);
            helmet4Renderer.gameObject.SetActive(false);
        }
        else if (helmetSelection == 4)
        {
            helmet1Renderer.gameObject.SetActive(false);
            helmet2Renderer.gameObject.SetActive(false);
            helmet3Renderer.gameObject.SetActive(false);
            helmet4Renderer.gameObject.SetActive(true);
        }
    }

    void DestroySelf()
    {
        Destroy(this.gameObject);
    }

    public void SetTeam(int team)
    {
        teamSelection = team;
    }

    public void SetHelmet(int helmet)
    {
        helmetSelection = helmet;
    }
}
