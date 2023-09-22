using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ReplayCarDataListItem : ISerializationCallbackReceiver
{
    [System.NonSerialized]
    public Vector3 position = Vector3.zero;
    [System.NonSerialized]
    public float timeSinceLevelLoaded;
    [System.NonSerialized]
    public float wheelsX = 0;
    [System.NonSerialized]
    public float wheelsY = 0;
    [System.NonSerialized]
    public Quaternion rotation;

    // preserve file size by rounding off the values of the floats
    // position
    [SerializeField]
    int x = 0;
    [SerializeField]
    int y = 0;
    [SerializeField]
    int z = 0;

    // time
    [SerializeField]
    int t = 0;

    // wheels
    [SerializeField]
    int wx;
    [SerializeField]
    int wy;

    // rotation
    [SerializeField]
    int qx;
    [SerializeField]
    int qy;
    [SerializeField]
    int qz;
    [SerializeField]
    int qw;

    public ReplayCarDataListItem(Vector3 position_, Quaternion rotation_, float timeSinceLoaded_, float wheelsX_, float wheelsY_)
    {
        position = position_;
        rotation = rotation_;
        timeSinceLevelLoaded = timeSinceLoaded_;
        wheelsX = wheelsX_;
        wheelsY = wheelsY_;
    }

    public void OnBeforeSerialize()
    {
        t = (int)(timeSinceLevelLoaded * 10000.0f);
        x = (int)(position.x * 10000.0f);
        y = (int)(position.y * 10000.0f);
        z = (int)(position.z * 10000.0f);

        wx = Mathf.RoundToInt(wheelsX * 10000.0f);
        wy = Mathf.RoundToInt(wheelsY * 10000.0f);

        qx = (int)(rotation.x * 100000.0f);
        qy = (int)(rotation.y * 100000.0f);
        qz = (int)(rotation.z * 100000.0f);
        qw = (int)(rotation.w * 100000.0f);
    }

    public void OnAfterDeserialize()
    {
        timeSinceLevelLoaded = (int)(t / 10000.0f);
        position.x = x / 10000.0f;
        position.y = y / 10000.0f;
        position.z = z / 10000.0f;

        wheelsX = wx / 100.0f;
        wheelsY = wy / 100.0f;

        rotation.x = qx / 1000.0f;
        rotation.y = qy / 1000.0f;
        rotation.z = qz / 1000.0f;
        rotation.w = qw / 1000.0f;
    }
}
