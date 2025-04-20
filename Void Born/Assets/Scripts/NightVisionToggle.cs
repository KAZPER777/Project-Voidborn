using UnityEngine;

public class NightVisionToggle : MonoBehaviour
{
    public GameObject nightVisionOverlay;
    private bool isActive = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            isActive = !isActive;
            nightVisionOverlay.SetActive(isActive);
        }
    }
}
