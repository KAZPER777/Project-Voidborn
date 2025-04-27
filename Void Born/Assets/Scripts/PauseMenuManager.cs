using UnityEngine;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject settingsPanel;
    public GameObject controlsPanel;

    public Button resumeButton;
    public Button quitButton;
    public Button settingsButton;
    public Button controlsButton;
    public Button backFromSettingsButton;
    public Button backFromControlsButton;

    private bool isPaused = false;

    void Start()
    {
        pauseMenuUI.SetActive(false);
        settingsPanel.SetActive(false);
        controlsPanel.SetActive(false);

        resumeButton.onClick.AddListener(ResumeGame);
        quitButton.onClick.AddListener(QuitGame);
        settingsButton.onClick.AddListener(OpenSettings);
        controlsButton.onClick.AddListener(OpenControls);
        backFromSettingsButton.onClick.AddListener(BackToPauseMenu);
        backFromControlsButton.onClick.AddListener(BackToPauseMenu);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !StartScreenManager.Instance.IsStartScreenActive())
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        settingsPanel.SetActive(false);
        controlsPanel.SetActive(false);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        settingsPanel.SetActive(false);
        controlsPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        controlsPanel.SetActive(false);
    }

    public void OpenControls()
    {
        controlsPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void BackToPauseMenu()
    {
        settingsPanel.SetActive(false);
        controlsPanel.SetActive(false);
    }
}
