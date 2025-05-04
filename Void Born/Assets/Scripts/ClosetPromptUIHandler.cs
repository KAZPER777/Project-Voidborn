using UnityEngine;

public class ClosetPromptUIHandler : MonoBehaviour
{
    public static ClosetPromptUIHandler Instance;

    [SerializeField] private GameObject promptUI;
    [SerializeField] private TMPro.TextMeshProUGUI promptText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        promptUI.SetActive(false);
    }

    public void ShowPrompt(string message)
    {
        promptText.text = message;
        promptUI.SetActive(true);
    }

    public void HidePrompt()
    {
        promptUI.SetActive(false);
    }
}
