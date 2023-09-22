using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] RaceManager raceManager;
    [SerializeField] GameObject menuObj;
    [SerializeField] GameObject buttonsMenu;
    [SerializeField] GameObject settingsMenu;
    bool paused = false;
    AudioSource[] audioSources;
    bool alreadyPaused = false;

    [SerializeField] GameObject pauseFirstButton, settingsFirstButton, optionsClosedButton;

    void Start()
    {
        menuObj.SetActive(false);
        audioSources = FindObjectsOfType<AudioSource>();
    }

    public void TogglePause()
    {
        if (raceManager.end) return;

        paused = !paused;
        menuObj.SetActive(paused);
        Time.timeScale = paused ? 0 : 1;
        foreach (AudioSource a in audioSources) a.mute = paused;
        if (paused)
        {
            // clear selected object
            EventSystem.current.SetSelectedGameObject(null);
            // set a new selected object
            EventSystem.current.SetSelectedGameObject(pauseFirstButton);

            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            foreach (AudioSource a in audioSources)
            {
                if (a.GetComponent<Radio>() && !a.isPlaying) alreadyPaused = true;

                a.Pause();
            }
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            foreach (AudioSource a in audioSources)
            {
                if (a.GetComponent<Radio>() && alreadyPaused) return;

                a.UnPause();
            }
        }
    }

    public void OpenSettings()
    {
        buttonsMenu.SetActive(false);
        settingsMenu.SetActive(true);
        settingsMenu.GetComponent<Settings>().SetValues();

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(settingsFirstButton);
    }

    public void CloseSettings()
    {
        buttonsMenu.SetActive(true);
        settingsMenu.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(optionsClosedButton);
    }

    public void ReturnToMenu()
    {
        TogglePause();
        SceneManager.LoadScene("Main Menu");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
