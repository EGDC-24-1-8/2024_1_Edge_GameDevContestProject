using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ScreenManager : MonoBehaviour
{
    public Toggle fullscreenToggle;
    public TMP_Dropdown resolutionDropdown;

    private List<Resolution> customResolutions;

    void Start()
    {
        fullscreenToggle.isOn = Screen.fullScreen;
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        
        InitializeResolutionOptions();
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
        
        UpdateResolutionDropdownState();
    }

    void InitializeResolutionOptions()
    {
        customResolutions = new List<Resolution>()
        {
            new Resolution { width = 2560, height = 1440 },
            new Resolution { width = 1920, height = 1080 },
            new Resolution { width = 1600, height = 900 },
            new Resolution { width = 1366, height = 768 },
            new Resolution { width = 1280, height = 720 },
            new Resolution { width = 720, height = 480 },


        };

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < customResolutions.Count; i++)
        {
            string option = customResolutions[i].width + " x " + customResolutions[i].height;
            options.Add(option);

            if (customResolutions[i].width == Screen.currentResolution.width &&
                customResolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        UpdateResolutionDropdownState();
    }

    void SetResolution(int resolutionIndex)
    {
        if (!Screen.fullScreen)
        {
            Resolution resolution = customResolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, false); 
        }
    }

    void UpdateResolutionDropdownState()
    {
        resolutionDropdown.interactable = !fullscreenToggle.isOn;
    }


    public void ExitGame()
            {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit(); // 어플리케이션 종료
        #endif
    }
}
