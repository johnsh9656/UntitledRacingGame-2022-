using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.PostProcessing;

public class MainMenu : MonoBehaviour
{
    [Header("Cars")]
    [SerializeField] GameObject[] cars;
    [SerializeField] int selectedCar;

    [Header("Maps")]
    [SerializeField] Map[] maps;
    //[SerializeField] GameObject[] environments;
    int mapIndex = 0;
    [SerializeField] Image mapImage;
    [SerializeField] TMP_Text mapName;
    [SerializeField] TMP_Text timeText;
    [SerializeField] GameObject menu;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject resetMenu;
    //[SerializeField] GameObject[] ppVolumes;

    [SerializeField] GameObject pauseFirstButton, settingsFirstButton, optionsClosedButton, resetFirstButton, resetClosedFirstButton;

    void Start()
    {
        selectedCar = Profile.instance.GetCarIndex();
        SetCar();
        Radio.instance.StartRadio();

        // randomize map
        mapIndex = UnityEngine.Random.Range(0, maps.Length);
        SetMap();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(maps[mapIndex].sceneName);
    }

    public void NextCar()
    {
        if (++selectedCar >= cars.Length) selectedCar = 0;
        SetCar();
    }

    private void SetCar()
    {
        for (int i = 0; i < cars.Length; i++)
        {
            if (i != selectedCar) cars[i].SetActive(false);
            else cars[i].SetActive(true);
        }
        Profile.instance.SetCarIndex(selectedCar);
    }

    public void CycleMap()
    {
        if (++mapIndex >= maps.Length) mapIndex = 0;
        SetMap();
    }

    void SetMap()
    {
        mapImage.sprite = maps[mapIndex].image;
        mapName.text = maps[mapIndex].name;
        TimeSpan t = TimeSpan.FromSeconds(PlayerPrefs.GetFloat($"{maps[mapIndex].sceneName}_time"));
        timeText.text = $"Best Time: {t.Minutes.ToString("00")}:{t.Seconds.ToString("00")}:{t.Milliseconds.ToString("00")}";

        // environment
        /* foreach (GameObject env in environments)
            env.SetActive(false);
        environments[mapIndex].SetActive(true);
        RenderSettings.skybox = null;
        RenderSettings.skybox = maps[mapIndex].skybox;
        foreach (GameObject v in ppVolumes) { v.SetActive(false); }
        ppVolumes[mapIndex].SetActive(true); */
    }

    public void OpenSettings()
    {
        menu.SetActive(false);
        settingsMenu.SetActive(true);
        settingsMenu.GetComponent<Settings>().SetValues();

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(settingsFirstButton);
    }

    public void CloseSettings()
    {
        menu.SetActive(true);
        settingsMenu.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(optionsClosedButton);
    }

    public void OpenResetMenu()
    {
        resetMenu.SetActive(true); 
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(resetFirstButton);
    }

    public void CloseResetMenu()
    {
        resetMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(resetClosedFirstButton);
    }

    public void ResetTimes()
    {
        PlayerPrefs.SetFloat("City_time", 600);
        PlayerPrefs.DeleteKey("City_replay");
        PlayerPrefs.DeleteKey("City_carIndex");
        PlayerPrefs.SetFloat("Town_time", 600);
        PlayerPrefs.DeleteKey("Town_replay");
        PlayerPrefs.DeleteKey("Town_carIndex");
        PlayerPrefs.Save();
        CloseResetMenu();
        SetMap();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
