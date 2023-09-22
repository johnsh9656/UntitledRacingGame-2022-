using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CheckpointSystem : MonoBehaviour
{
    [SerializeField] List<Checkpoint> checkpoints;
    [SerializeField] WarningUI warning;
    [SerializeField] string warningTextMain = "Missed Checkpoint";
    [SerializeField] string warningTextSub = "'R'/Y Button to restart from last checkpoint";
    [SerializeField] TMP_Text lapsText;
    [SerializeField] int maxLaps = 3;
    [SerializeField] Image lapBar;
    [SerializeField] TMP_Text lapBarText;
    int nextCPIndex = 0;
    int lastCPIndex = 0;
    int laps = 0;

    [SerializeField] bool town;
    [SerializeField] List<Checkpoint> ignoreSkip;

    public void ThroughCheckpont(Checkpoint cp)
    {
        if (laps > maxLaps) return;

        if (checkpoints.IndexOf(cp) == nextCPIndex) // correct checkpoint
        {
            // check to add lap
            if (nextCPIndex == 0)
            {
                foreach (Checkpoint c in checkpoints)
                {
                    c.gameObject.SetActive(true);
                }

                // check for finished
                if (laps == maxLaps)
                {
                    Finish();
                    laps++;
                    return;
                }

                StartCoroutine(NewLap());
            }

            nextCPIndex = (nextCPIndex + 1) % checkpoints.Count;
            lastCPIndex = nextCPIndex == 0 ? checkpoints.Count - 1 : nextCPIndex - 1;
            warning.RemoveWarning();
            cp.HideMesh();
            cp.gameObject.SetActive(false);

            checkpoints[0].gameObject.SetActive(true);
        }
        else if (checkpoints.IndexOf(cp) != nextCPIndex && checkpoints.IndexOf(cp) != lastCPIndex)
        {
            if (town)
            {
                foreach (Checkpoint c in ignoreSkip)
                {
                    if (c == cp) return;
                }
            }

            warning.SetWarning(warningTextMain, warningTextSub);
            checkpoints[nextCPIndex].ShowMesh();
        }
    }

    IEnumerator NewLap()
    {
        laps++;
        lapsText.text = laps + " / " + maxLaps;

        if (laps == 1) yield break;
        if (laps == maxLaps)
        {
            FindObjectOfType<ArcadeCarController>().FinalLap();
        }

        lapBar.enabled = true;
        lapBarText.text = lapsText.text;

        yield return new WaitForSeconds(2f);
        lapBar.enabled = false;
        lapBarText.text = "";
    }

    private void Finish()
    {
        FindObjectOfType<RaceManager>().EndGame();
        
        // disable checkpoints
        foreach (Checkpoint c in checkpoints)
        {
            c.enabled = false;
        }
    }

    public Vector3 GetCurrentCheckpointPos()
    {
        Vector3 pos = checkpoints[lastCPIndex].transform.position;
        return new Vector3(pos.x, pos.y - 3.5f, pos.z);
    }

    public Quaternion GetCurrentCheckpointRotation()
    {
        return checkpoints[lastCPIndex].transform.rotation;
    }

    public void RemoveAllWarnings()
    {
        warning.RemoveWarning();
    }

    public bool FirstCheckpoint() { return nextCPIndex == 0 && laps == 0; }

}