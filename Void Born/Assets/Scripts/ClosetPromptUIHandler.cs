using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ClosetPromptUIHandler : MonoBehaviour
{
    public GameObject promptPanel; // The full panel with icon + text
    public ClosetDoorMaster closetDoor;
    public Transform player;
    public Transform closetTriggerPoint;
    public float triggerDistance = 2f;

    private bool isNearCloset = false;

    void Update()
    {
        if (!closetDoor) return;

        float distance = Vector3.Distance(player.position, closetTriggerPoint.position);
        isNearCloset = distance <= triggerDistance;

        if (isNearCloset && !IsPlayerInside())
        {
            promptPanel.SetActive(true);
        } else if (IsPlayerInside())
        {
            promptPanel.SetActive(true);
            // Optional: change text to "Mouse 2 to exit" etc.
        } else
        {
            promptPanel.SetActive(false);
        }
    }

    bool IsPlayerInside()
    {
        return closetDoor.GetType().GetField("isPlayerInside", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(closetDoor) is bool inside && inside;
    }
}
