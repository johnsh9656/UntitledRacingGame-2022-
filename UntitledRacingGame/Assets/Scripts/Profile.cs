using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Profile : MonoBehaviour
{
    public static Profile instance;

    [SerializeField] float City_Rain_time;
    [SerializeField] float town_Time;

    [SerializeField] AudioMixer mainMixer;

    public int carIndex = 0;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
        //Application.targetFrameRate = 60;
        //QualitySettings.vSyncCount = 0;
    }

    private void Start()
    {
        if (!PlayerPrefs.HasKey("main_volume")) PlayerPrefs.SetFloat("main_volume", .8f);
        mainMixer.SetFloat("volume", Mathf.Log10(PlayerPrefs.GetFloat("main_volume")) * 20);
        if (!PlayerPrefs.HasKey("quality")) PlayerPrefs.SetInt("quality", 5);
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("quality"));

        if (!PlayerPrefs.HasKey("City_time"))
        {
            PlayerPrefs.SetFloat("City_time", 600);
        }
        if (!PlayerPrefs.HasKey("Town_time"))
        {
            PlayerPrefs.SetFloat("Town_time", 600);
        }

        City_Rain_time = PlayerPrefs.GetFloat("City_time");
        town_Time = PlayerPrefs.GetFloat("Town_time");
    }

    public float CheckSetTime(int m, float t)
    {
        if (m == 1)
        {
            if (t < City_Rain_time)
            {
                City_Rain_time = t;
                PlayerPrefs.SetFloat("City_time", City_Rain_time);
                PlayerPrefs.Save();
                FindObjectOfType<ReplayCarRecorder>().SaveData();
            }
            return City_Rain_time;
        }
        else
        {
            if (t < town_Time)
            {
                town_Time = t;
                PlayerPrefs.SetFloat("Town_time", town_Time);
                PlayerPrefs.Save();
                FindObjectOfType<ReplayCarRecorder>().SaveData();
            }
            return town_Time;
        }
    }

    public void SetCarIndex(int i) { carIndex = i; }
    public int GetCarIndex() { return carIndex; }

    public void SetVolume(float vol) 
    {
        mainMixer.SetFloat("volume", Mathf.Log10(vol) * 20);
        
        PlayerPrefs.SetFloat("main_volume", vol);
        PlayerPrefs.Save();
    }
}
