using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WallCooldownUI : MonoBehaviour
{
    public Slider slider;
    // Update is called once per frame
    public void UpdateCooldownUI(float fill)
    {
        slider.value = fill;
    }
}
