using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.PlayerLoop;
using System.Security.Cryptography;

public class Radio : MonoBehaviour
{
    public static Radio instance;

    [Header("Tracks")]
    [SerializeField] private Track[] tracks;

    [Header("UI")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text artistText;
    [SerializeField] private Image albumImage;
    [SerializeField] private Image[] volumeImages;

    private AudioSource audS;
    private Animator anim;
    private Vector3 artistTransform;
    private int trackIndex;
    private float volume = 5;

    bool started = false;
    bool waiting;
    float waitTime = 0;
    float waitTimeMax = 4;
    
    // Singleton Pattern
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        audS = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        artistTransform = artistText.transform.localPosition;
        trackIndex = Random.Range(0, tracks.Length);
    }

    public void StartRadio()
    {
        if (started) return;

        started = true;
        UpdateTrack();
        UpdateVolume();
    }

    private void UpdateTrack()
    {
        audS.clip = tracks[trackIndex].trackAudioClip;
        titleText.text = tracks[trackIndex].name;
        artistText.text = tracks[trackIndex].artist;
        albumImage.sprite = tracks[trackIndex].image;
        audS.Play();

        if (tracks[trackIndex].name.Length > 18)
        {
            artistText.transform.localPosition = new Vector3
                (artistTransform.x, artistTransform.y - 25, artistTransform.z);
        }
        else
        {
            artistText.transform.localPosition = artistTransform;
        }
        AppearAnim();
    }

    private void UpdateVolume()
    {
        for (int i = 0; i < volumeImages.Length; i++)
        {
            if (i + 1 <= volume) volumeImages[i].enabled = true;
            else volumeImages[i].enabled = false;
        }
        AppearAnim();
    }

    private void NextSong()
    {
        if (Time.timeScale == 0) return;

        trackIndex++;
        if (trackIndex >= tracks.Length) trackIndex = 0;
        UpdateTrack();
    }

    private void AppearAnim()
    {
        anim.SetBool("b", true);
        waitTime = 0;
        waiting = true;
    }

    private void Update()
    {
        if (!waiting) return;

        if (waitTime < waitTimeMax) waitTime += Time.deltaTime;
        else 
        {
            anim.SetBool("b", false);
            waiting = false;
        }
    }

    public void Toggle(InputAction.CallbackContext context)
    {
        if (!context.performed || Time.timeScale == 0) return;

        if (audS.isPlaying)
        {
            audS.Pause();
        } 
        else
        {
            audS.Play();
        }
        AppearAnim();
    }

    public void Skip(InputAction.CallbackContext context)
    {
        if (!context.performed || Time.timeScale == 0) return;

        NextSong();
    }

    public void Volume(InputAction.CallbackContext context)
    {
        if (!context.performed || Time.timeScale == 0) return;

        volume += context.ReadValue<float>();
        if (volume < 0) volume = 0;
        else if (volume > 10) volume = 10;

        audS.volume = volume / 20;

        UpdateVolume();
    }
}
