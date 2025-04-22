using UnityEngine;

public class CamcorderToggle : MonoBehaviour
{
    public GameObject camcorderUI;
    public GameObject nightVisionOverlay; 

    private bool isActive = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isActive = !isActive;
            camcorderUI.SetActive(isActive);

            // Toggle the night vision overlay only if assigned
            if (nightVisionOverlay != null)
                nightVisionOverlay.SetActive(isActive);
        }
    }
}
