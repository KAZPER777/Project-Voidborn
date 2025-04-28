using UnityEngine;

public class CamcorderToggle : MonoBehaviour
{
    public GameObject camcorderUI;
    private bool camcorderActive = false;

    private void Update()
    {
        if (!GameManager.Instance.gameStarted) return; 

        if (Input.GetKeyDown(KeyCode.C))
        {
            camcorderActive = !camcorderActive;
            if (camcorderUI != null)
                camcorderUI.SetActive(camcorderActive);
        }
    }
}
