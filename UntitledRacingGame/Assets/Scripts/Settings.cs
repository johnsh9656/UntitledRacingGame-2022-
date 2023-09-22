using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class Settings : MonoBehaviour
{
    [SerializeField] Toggle fullscreenToggle;
    [SerializeField] TMP_Dropdown dropdown;
    [SerializeField] Slider volume;

    public void SetValues()
    {
        fullscreenToggle.isOn = Screen.fullScreen;
        dropdown.value = QualitySettings.GetQualityLevel() - 3;
        volume.value = PlayerPrefs.GetFloat("main_volume");
    }

    public void ToggleFullscreen()
    {
        Screen.fullScreen = fullscreenToggle.isOn;
    }

    public void SetQuality()
    {
        int q = dropdown.value + 3;
        QualitySettings.SetQualityLevel(q);

        PlayerPrefs.SetInt("quality", q);
        PlayerPrefs.Save();
    }

    public void SetVolume()
    {
        FindObjectOfType<Profile>().SetVolume(volume.value);
    }
}
