using UnityEngine;

public class FlashlightToggle : MonoBehaviour
{
    public Light flashlight; // assign via inspector
    public KeyCode toggleKey = KeyCode.F;
    private bool isOn = false;

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isOn = !isOn;
            flashlight.enabled = isOn;
        }
    }
}
