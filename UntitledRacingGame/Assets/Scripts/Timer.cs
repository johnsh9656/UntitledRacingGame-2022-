using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Timer : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    public bool timing = false;
    float time = 0;

    private void Update()
    {
        if (!timing) return;

        time += Time.deltaTime;
        TimeSpan t = TimeSpan.FromSeconds(time);
        text.text = t.Minutes.ToString("00") + ":" + t.Seconds.ToString("00");
    }

    public void StartTimer()
    {
        timing = true;
        FindObjectOfType<ReplayCarRecorder>().StartRecording();
    }

    public void EndTimer()
    {
        timing = false;
        text.enabled = false;
    }

    public float GetTime() { return time; }
    public String GetTimeString() { return text.text; }
}
