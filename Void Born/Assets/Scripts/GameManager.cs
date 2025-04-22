using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player Settings")]
    public JaidensPlayerController playerScript;
    public Transform playerSpawnPos;

    [Header("UI References")]
    public Image playerHPBar;                // Assign Image component directly
    public GameObject playerdamagescreen;
    public GameObject checkpointPopup;
    public GameObject YouLose;

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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        if (UIManager.Instance != null)
            UIManager.Instance.ShowPauseMenu(isPaused);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (UIManager.Instance != null)
            UIManager.Instance.ShowPauseMenu(false);
    }

    public void ShowLoseScreen()
    {
        if (YouLose != null)
            YouLose.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
