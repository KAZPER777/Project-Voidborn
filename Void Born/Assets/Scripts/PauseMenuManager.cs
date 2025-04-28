using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PauseMenuManager : MonoBehaviour
{
    public static PauseMenuManager Instance;

    [Header("UI References")]
    public GameObject pauseMenuPanel; 
    public GameObject mainPanel;       
    public Button resumeButton;
    public Button settingsButton;
    public Button settingsBackButton;
    public GameObject settingsPanel;
    public Button controlsButton;
    public Button controlsBackButton;
    public GameObject controlsPanel;
    public Button quitToDesktopButton;

    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        controlsPanel.SetActive(false);

       
        resumeButton.onClick.AddListener(TogglePause);
        settingsButton.onClick.AddListener(OpenSettings);
        settingsBackButton.onClick.AddListener(CloseSettings);
        controlsButton.onClick.AddListener(OpenControls);
        controlsBackButton.onClick.AddListener(CloseControls);
        quitToDesktopButton.onClick.AddListener(OnQuitClicked);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenuPanel.SetActive(isPaused);

        if (isPaused)
        {
          
            mainPanel.SetActive(true);
            settingsPanel.SetActive(false);
            controlsPanel.SetActive(false);

            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void OpenSettings()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    private void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    private void OpenControls()
    {
        mainPanel.SetActive(false);
        controlsPanel.SetActive(true);
    }

    private void CloseControls()
    {
        controlsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    private void OnQuitClicked()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

