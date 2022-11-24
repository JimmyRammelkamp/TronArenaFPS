using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CooldownUI : MonoBehaviour
{
    public Color readyColor = Color.green;
    public Color rechargingColor = Color.red;

    public Slider slider;
    public Image fillArea;
    // Update is called once per frame
    public void UpdateCooldownUI(float fill)
    {
        slider.value = fill;
        if(fill < 1)
        {
            fillArea.color = rechargingColor;
        }
        else
        {
            fillArea.color = readyColor;
        }
    }
}
