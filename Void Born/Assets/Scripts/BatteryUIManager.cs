using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BatteryUIManager : MonoBehaviour
{
    public BatterySystem batterySystem;
    public Image batteryFill;
    public List<Image> batterySegments; 
    public GameObject lowBatteryWarning;
    public float lowBatteryThreshold = 0.2f;
    public Color normalColor = Color.white;
    public Color flashColor = Color.red;

    private bool isFlashing = false;

    private void Update()
    {
        if (batterySystem == null) return;

        UpdateSegments();
        UpdateLowBatteryWarning();
        UpdateBatteryIconVisibility();
    }

    private void UpdateSegments()
    {
        if (batterySegments == null || batterySegments.Count == 0) return;

        float batteryPercent = batterySystem.CurrentBattery;

        for (int i = 0; i < batterySegments.Count; i++)
        {
            if (batterySegments[i] == null) continue;

            float threshold = 1f - (i * (1f / batterySegments.Count));

            batterySegments[i].enabled = batteryPercent >= threshold;
        }
    }

    private void UpdateLowBatteryWarning()
    {
        if (lowBatteryWarning == null) return;

        if (batterySystem.CurrentBattery <= lowBatteryThreshold)
        {
            if (!isFlashing)
                StartCoroutine(BlinkWarning());
        }
        else
        {
            if (isFlashing)
            {
                StopAllCoroutines();
                ResetSegmentColor();
                lowBatteryWarning.SetActive(false);
                isFlashing = false;
            }
        }
    }

    private IEnumerator BlinkWarning()
    {
        isFlashing = true;
        Image lastSegment = batterySegments.Count > 0 ? batterySegments[batterySegments.Count - 1] : null;

        while (true)
        {
            if (lastSegment != null)
                lastSegment.color = flashColor;

            lowBatteryWarning.SetActive(!lowBatteryWarning.activeSelf);
            yield return new WaitForSeconds(0.5f);

            if (lastSegment != null)
                lastSegment.color = normalColor;

            yield return new WaitForSeconds(0.5f);
        }
    }

    private void ResetSegmentColor()
    {
        Image lastSegment = batterySegments.Count > 0 ? batterySegments[batterySegments.Count - 1] : null;
        if (lastSegment != null)
            lastSegment.color = normalColor;
    }

    private void UpdateBatteryIconVisibility()
    {
        if (batteryFill == null) return;

        if (batterySystem.CurrentBattery <= 0.01f)
        {
            batteryFill.gameObject.SetActive(false);
        }
        else
        {
            batteryFill.gameObject.SetActive(true);
        }
    }


}
