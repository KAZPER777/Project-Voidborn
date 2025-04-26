using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject pauseMenuUI;
    public GameObject gameplayUI;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ShowPauseMenu(bool show)
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(show);
    }

    public void ShowGameplayUI(bool show)
    {
        if (gameplayUI != null)
            gameplayUI.SetActive(show);
    }
}
