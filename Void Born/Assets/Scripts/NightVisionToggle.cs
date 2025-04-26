using UnityEngine;

public class NightVisionToggle : MonoBehaviour
{
    public GameObject nightVisionOverlay;
    private bool nightVisionActive = false;

    private void Update()
    {
        if (!GameManager.Instance.gameStarted) return; 

        if (Input.GetKeyDown(KeyCode.N))
        {
            nightVisionActive = !nightVisionActive;
            if (nightVisionOverlay != null)
                nightVisionOverlay.SetActive(nightVisionActive);
        }
    }
}
