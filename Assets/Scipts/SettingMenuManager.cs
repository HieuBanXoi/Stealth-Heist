using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.SceneManagement;

public class SettingMenuManager : MonoBehaviour
{
    private readonly Resolution[] commonResolutions = new Resolution[]
    {
        new Resolution { width = 1920, height = 1080 }, // Full HD
        new Resolution { width = 1366, height = 768 },  // HD
        new Resolution { width = 1280, height = 720 },  // HD
        new Resolution { width = 1024, height = 768 },  // XGA
        new Resolution { width = 800, height = 600 }    // SVGA
    };

    public Resolution[] resolutions;
    public TMP_Dropdown resolutionDropdown;
    public AudioMixer audioMixer;
    public void Start()
    {
        resolutions = commonResolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
    public void BackToMenu()
    {
        SceneManager.LoadScene("MainHall");
    }

}
