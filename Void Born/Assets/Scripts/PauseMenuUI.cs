using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Main Panels")]
    public GameObject pauseMenuUI;
    public GameObject settingsPanel;
    public GameObject controlsPanel;

    [Header("Buttons")]
    public Button resumeButton;
    public Button quitButton;
    public Button settingsButton;
    public Button controlsButton;
    public Button backFromSettingsButton;
    public Button backFromControlsButton;

    [Header("Settings UI")]
    public Slider volumeSlider;
    public Slider brightnessSlider;

    private bool isPaused = false;

    void Start()
    {
        // Hide all panels at start
        pauseMenuUI.SetActive(false);
        settingsPanel.SetActive(false);
        controlsPanel.SetActive(false);

        // Button Listeners
        resumeButton.onClick.AddListener(ResumeGame);
        quitButton.onClick.AddListener(QuitGame);
        settingsButton.onClick.AddListener(OpenSettings);
        controlsButton.onClick.AddListener(OpenControls);
        backFromSettingsButton.onClick.AddListener(BackToPauseMenu);
        backFromControlsButton.onClick.AddListener(BackToPauseMenu);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    void PauseGame()
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
        Debug.Log("Quit Game");
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
