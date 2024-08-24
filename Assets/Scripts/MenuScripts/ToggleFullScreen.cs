using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleFullScreen : MonoBehaviour
{
    public Toggle fullscreen;

    private void Start()
    {
        fullscreen.onValueChanged.AddListener(delegate
        {
            toggleColor();
        });
    }

    void toggleColor()
    {
        if (fullscreen.isOn) fullscreen.targetGraphic.color = Color.red;
        else fullscreen.targetGraphic.color = Color.white;

    }
}
