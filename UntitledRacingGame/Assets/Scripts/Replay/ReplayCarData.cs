using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ReplayCarData
{
    [SerializeField]
    List<ReplayCarDataListItem> replayCarRecorderList = new List<ReplayCarDataListItem>();

    public void AddDataItem(ReplayCarDataListItem listItem)
    {
        replayCarRecorderList.Add(listItem);
    }

    public List<ReplayCarDataListItem> GetDataList()
    {
        return replayCarRecorderList;
    }
}
