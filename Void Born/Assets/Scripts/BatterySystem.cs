using UnityEngine;
using UnityEngine.UI;

public class BatterySystem : MonoBehaviour
{
    [SerializeField] private Image batteryFill;
    [SerializeField] private float drainRate = 0.05f;
    [SerializeField] public float currentBattery = 1f;

    public float CurrentBattery => currentBattery;

    private void Update()
    {
        DrainBattery();
        UpdateBatteryUI();
    }

    private void DrainBattery()
    {
        currentBattery -= drainRate * Time.deltaTime;
        currentBattery = Mathf.Clamp01(currentBattery);
    }

    private void UpdateBatteryUI()
    {
        if (batteryFill != null)
        {
            batteryFill.fillAmount = currentBattery;
        }
    }
}
