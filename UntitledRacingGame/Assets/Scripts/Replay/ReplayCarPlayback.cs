using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReplayCarPlayback : MonoBehaviour
{
    ReplayCarData replayCarData = new ReplayCarData();
    List<ReplayCarDataListItem> replayCarDataList = new List<ReplayCarDataListItem>();
    public Transform[] wheels;
    public Transform[] frontWheels;
    public Transform model;
    Timer timer;

    int playbackIndex = 0;

    // playback stored information
    float lastStoredTime = 0.01f;
    Vector3 lastStoredPos = Vector3.zero;

    Quaternion lastStoredRot = Quaternion.identity;

    float duration = 0.01f;

    private void Awake()
    {
        Debug.Log("playblack");
        timer = FindObjectOfType<Timer>();
        LoadData();
        lastStoredPos = transform.position;
    }

    private void Update()
    {
        // only playback if there is data
        if (replayCarDataList.Count == 0 || !timer.timing)
            return;

        if (timer.GetTime() >= replayCarDataList[playbackIndex].timeSinceLevelLoaded)
        {
            lastStoredTime = replayCarDataList[playbackIndex].timeSinceLevelLoaded;
            lastStoredPos = replayCarDataList[playbackIndex].position;
            lastStoredRot = replayCarDataList[playbackIndex].rotation;
            
            // step to next item
            if (playbackIndex < replayCarDataList.Count - 1)
                playbackIndex++;

            duration = replayCarDataList[playbackIndex].timeSinceLevelLoaded - lastStoredTime;
        }

        // calculate how much of the data framed that we have completed
        float timePassed = timer.GetTime() - lastStoredTime;
        float lerpPercentage = timePassed / duration;
        
        // lerp everything
        transform.position = Vector3.Lerp(lastStoredPos, replayCarDataList[playbackIndex].position, lerpPercentage);
        transform.rotation = Quaternion.Lerp(lastStoredRot, replayCarDataList[playbackIndex].rotation, lerpPercentage);

        foreach (Transform fw in frontWheels)
            fw.rotation = Quaternion.Euler(0, replayCarDataList[playbackIndex].wheelsY, 0);

        foreach (Transform w in wheels)
            w.Rotate(Time.deltaTime * 1000, 0, 0, Space.Self);
    }

    void LoadData()
    {
        if (!PlayerPrefs.HasKey($"{SceneManager.GetActiveScene().name}_replay"))
        {
            Destroy(gameObject); // set to default playbacks
        }
        else
        {
            string jsonEncodedData = PlayerPrefs.GetString($"{SceneManager.GetActiveScene().name}_replay");

            replayCarData = JsonUtility.FromJson<ReplayCarData>(jsonEncodedData);
            replayCarDataList = replayCarData.GetDataList();
        }
    }
}
