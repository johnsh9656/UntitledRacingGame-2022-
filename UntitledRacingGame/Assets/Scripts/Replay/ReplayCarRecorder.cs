using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReplayCarRecorder : MonoBehaviour
{
    public Transform carObj;
    public Transform wheelXObj;
    public Transform wheelYObj;
    public Timer timer;

    // local variables
    ReplayCarData replayCarData = new ReplayCarData();

    bool isRecording = true;

    //[SerializeField] Rigidbody rb;
    ArcadeCarController carController;

    private void Awake()
    {
        carController = GetComponent<ArcadeCarController>();
        Debug.Log(SceneManager.GetActiveScene().name);
    }

    public void StartRecording()
    {
        StartCoroutine(RecordCarPos());
    }

    IEnumerator RecordCarPos()
    {
        while (isRecording)
        {
            //Debug.Log("isRecording : " + isRecording);
            if (carObj != null)
                replayCarData.AddDataItem(new ReplayCarDataListItem(carObj.position, carObj.rotation, timer.GetTime(), wheelXObj.rotation.x, wheelYObj.rotation.y));

            yield return new WaitForSeconds(0.01f);
        }
    }

    public void SaveData()
    {
        string jsonEncodedData = JsonUtility.ToJson(replayCarData);

        Debug.Log($"Saved replay data {jsonEncodedData}");

        if (carController != null)
        {
            PlayerPrefs.SetString($"{SceneManager.GetActiveScene().name}_replay", jsonEncodedData);
            // add _{carController.carStr} after scene and before replay to set replay for each vehicle
            PlayerPrefs.SetInt($"{SceneManager.GetActiveScene().name}_carIndex", Profile.instance.carIndex);
            PlayerPrefs.Save();
        }

        isRecording = false;
    }
}
