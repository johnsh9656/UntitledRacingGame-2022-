using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightLight : MonoBehaviour
{
    [SerializeField] bool day = false;
    [SerializeField] Light[] lights;

    public void SetDay()
    {
        day = true;
        foreach (Light l in lights)
        {
            l.gameObject.SetActive(false);
        }
    }
}
