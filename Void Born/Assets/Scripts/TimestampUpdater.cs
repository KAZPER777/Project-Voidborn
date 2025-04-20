using UnityEngine;
using TMPro;
using System;

public class TimestampUpdater : MonoBehaviour
{
    private TMP_Text timestampText;

    void Start()
    {
        timestampText = GetComponent<TMP_Text>();
    }

    void Update()
    {
        DateTime now = DateTime.Now;
        string timeString = now.ToString("MMM. dd yyyy | hh:mm:ss tt").ToUpper();
        timestampText.text = timeString;
    }
}
