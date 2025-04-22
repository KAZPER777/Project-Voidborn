using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player Settings")]
    public JaidensPlayerController playerScript;
    public Transform playerSpawnPos;

    [Header("UI References")]
    public Image playerHPBar;
    public GameObject playerdamagescreen;
    public GameObject checkpointPopup;
    public GameObject YouLose;

    [Header("Pause Menu UI")]
    public GameObject pauseMenuUI;
    public GameObject settingsPanel;
    public GameObject controlsPanel;

    [Header("Pause Menu Buttons")]
    public Button resumeButton;
    public Button quitButton;
    public Button settingsButton;
    public Button controlsButton;
    public Button backFromSettingsButton;
    public Button backFromControlsButton;

    [Header("Settings Controls")]
    public Slider volumeSlider;
    public Slider brightnessSlider;

    private bool isPaused = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(transform.root.gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Hide all panels at start
        pauseMenuUI?.SetActive(false);
        settingsPanel?.SetActive(false);
        controlsPanel?.SetActive(false);

        // Button Listeners
        resumeButton?.onClick.AddListener(ResumeGame);
        quitButton?.onClick.AddListener(QuitGame);
        settingsButton?.onClick.AddListener(OpenSettings);
        controlsButton?.onClick.AddListener(OpenControls);
        backFromSettingsButton?.onClick.AddListener(BackToPauseMenu);
        backFromControlsButton?.onClick.AddListener(BackToPauseMenu);

        // Initial spawn
        if (playerScript != null)
        {
            playerScript.SpawnPlayer();
        }
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

    // ==============================
    // Pause / Resume / Quit
    // ==============================

    public void PauseGame()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        pauseMenuUI?.SetActive(true);
        settingsPanel?.SetActive(false);
        controlsPanel?.SetActive(false);
        Time.timeScale = 0f;
        isPaused = true;

        // Also hide HUD if UIManager is being used
        UIManager.Instance?.ShowPauseMenu(true);
    }

    public void ResumeGame()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenuUI?.SetActive(false);
        settingsPanel?.SetActive(false);
        controlsPanel?.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        UIManager.Instance?.ShowPauseMenu(false);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ==============================
    // Pause Menu Navigation
    // ==============================

    public void OpenSettings()
    {
        settingsPanel?.SetActive(true);
        controlsPanel?.SetActive(false);
    }

    public void OpenControls()
    {
        controlsPanel?.SetActive(true);
        settingsPanel?.SetActive(false);
    }

    public void BackToPauseMenu()
    {
        settingsPanel?.SetActive(false);
        controlsPanel?.SetActive(false);
    }

    // ==============================
    // Game Events
    // ==============================

    public void ShowLoseScreen()
    {
        if (YouLose != null)
            YouLose.SetActive(true);
    }

    public void ShowCheckpointPopup()
    {
        if (checkpointPopup != null)
        {
            checkpointPopup.SetActive(true);
            CancelInvoke(nameof(HideCheckpointPopup));
            Invoke(nameof(HideCheckpointPopup), 2f);
        }
    }

    private void HideCheckpointPopup()
    {
        if (checkpointPopup != null)
            checkpointPopup.SetActive(false);
    }

    public void ResetDamageScreen()
    {
        if (playerdamagescreen != null)
        {
            playerdamagescreen.SetActive(false);
        }
    }
}
