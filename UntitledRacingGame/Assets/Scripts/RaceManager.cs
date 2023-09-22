using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.EventSystems;

public class RaceManager : MonoBehaviour
{
    [SerializeField] Timer timer;
    [SerializeField] Image lapBar;
    [SerializeField] TMP_Text startingTimer;

    [SerializeField] GameObject finishUI;
    [SerializeField] GameObject returnButton;
    [SerializeField] TMP_Text timeText;
    [SerializeField] TMP_Text bestTimeText;

    [SerializeField] GameObject[] replayCarPrefabs;

    [SerializeField] PostProcessProfile dofProfile;
    public int map = 1;
    public bool end;

    public bool day = true;

    void Start()
    {
        StartCoroutine(StartingTimer());
        dofProfile.GetSetting<DepthOfField>().focusDistance.Override(0);

        if (PlayerPrefs.HasKey($"{SceneManager.GetActiveScene().name}_carIndex"))
        {
            int prefabIndex = PlayerPrefs.GetInt($"{SceneManager.GetActiveScene().name}_carIndex");
            GameObject replayCar = Instantiate(replayCarPrefabs[prefabIndex]);
        }

        if (day)
        {
            foreach (DayNightLight l in FindObjectsOfType<DayNightLight>())
            {
                l.SetDay();
            }
        }
    }

    private void Update()
    {
        // Depth of Field
        if (dofProfile.GetSetting<DepthOfField>().focusDistance < 7 && !end)
        {
            float dof = dofProfile.GetSetting<DepthOfField>().focusDistance + Time.deltaTime * 2;
            dofProfile.GetSetting<DepthOfField>().focusDistance.Override(dof);

            if (dofProfile.GetSetting<DepthOfField>().focusDistance > 7)
                dofProfile.GetSetting<DepthOfField>().focusDistance.Override(7);
        }
        else if (dofProfile.GetSetting<DepthOfField>().focusDistance > 1 && end)
        {
            float dof = dofProfile.GetSetting<DepthOfField>().focusDistance - Time.deltaTime * 2;
            dofProfile.GetSetting<DepthOfField>().focusDistance.Override(dof);
        }
    }

    IEnumerator StartingTimer()
    {
        lapBar.enabled = false;
        startingTimer.text = "";
        yield return new WaitForSeconds(1f);
        lapBar.enabled = true;
        startingTimer.text = "3";
        yield return new WaitForSeconds(1f);
        startingTimer.text = "2";
        yield return new WaitForSeconds(1f);
        startingTimer.text = "1";
        yield return new WaitForSeconds(1f);
        startingTimer.text = "GO!";
        StartGame();
        yield return new WaitForSeconds(1f);
        lapBar.enabled = false;
        startingTimer.text = "";
    }

    private void StartGame()
    {
        timer.StartTimer();
        FindObjectOfType<ArcadeCarController>().StartGame();
    }

    public void EndGame()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(returnButton);

        timer.EndTimer();
        FindObjectOfType<ArcadeCarController>().EndGame(false);
        FindObjectOfType<CheckpointSystem>().RemoveAllWarnings();

        end = true;
        finishUI.SetActive(true);
        timeText.text = timer.GetTimeString();
        float best = Profile.instance.CheckSetTime(map, timer.GetTime());
        TimeSpan t = TimeSpan.FromSeconds(best);
        bestTimeText.text = "Best Time: " + t.Minutes.ToString("00") + ":" + t.Seconds.ToString("00") + ":" + t.Milliseconds.ToString("00");

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
