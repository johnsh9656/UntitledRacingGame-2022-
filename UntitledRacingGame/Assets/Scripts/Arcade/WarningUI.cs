using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WarningUI : MonoBehaviour
{
    [SerializeField] GameObject warning;
    [SerializeField] TMP_Text mainText;
    [SerializeField] TMP_Text subText;

    public void SetWarning(string main, string sub)
    {
        warning.SetActive(true);
        mainText.SetText(main);
        subText.SetText(sub);
    }

    public void RemoveWarning()
    {
        warning.SetActive(false);
    }
}
