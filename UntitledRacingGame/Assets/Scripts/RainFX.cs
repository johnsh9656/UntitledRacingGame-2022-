using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainFX : MonoBehaviour
{
    ArcadeCarController controller;

    private void Start()
    {
        controller = FindObjectOfType<ArcadeCarController>();
    }

    private void Update()
    {
        transform.position = controller.transform.position;
    }
}
