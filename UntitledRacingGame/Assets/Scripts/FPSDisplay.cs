using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    public int avgFrameRate;
    public TMP_Text displayText;

    public void Update()
    {
        avgFrameRate = (int)(1f / Time.unscaledDeltaTime);
        displayText.text = avgFrameRate.ToString();
    }
}
