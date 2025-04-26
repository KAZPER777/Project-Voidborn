using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private bool isPaused = false;
    public bool gameStarted = false; 

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (!gameStarted) return; // Only allow pause if game started

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void StartGame()
    {
        gameStarted = true;
        Time.timeScale = 1f;
        UIManager.Instance.ShowPauseMenu(false);
        UIManager.Instance.ShowGameplayUI(true);
    }

    public void TogglePause()
    {
        if (!gameStarted) return; 

        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        if (UIManager.Instance != null)
            UIManager.Instance.ShowPauseMenu(isPaused);
    }

    public void ResumeGame()
    {
        if (!gameStarted) return;

        isPaused = false;
        Time.timeScale = 1f;
        UIManager.Instance.ShowPauseMenu(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
