using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public enum GameState
    {
        StartScreen,
        Playing,
        Paused,
        GameOver,
        Win
    }


    public static GameManager Instance;

    [Header("Player Settings")]
    public JaidensPlayerController playerScript;
    public Transform playerSpawnPos;

    [Header("UI Elements")]
    public GameObject winMenuUI;
    public GameObject checkpointPopup;

    public GameObject playerDamageScreen;
    public GameObject youLoseScreen;
    public Slider playerHPBar;
	public Image sanityBar;

    [Header("Pause Menu")]
    public GameObject pauseMenuUI;
    public GameObject hudUI;

    private bool isPaused = false;
    public bool gameStarted = false;

    [Header("Checkpoint System")]
    [SerializeField] private Transform currentCheckpoint;

    public GameState CurrentState { get; private set; } = GameState.StartScreen;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional if you switch scenes
        } else
        {
            Destroy(gameObject);
        }
    }

    public void StartGame()
    {
        Debug.Log("🚀 Game Started!");
        CurrentState = GameState.Playing;

        // ✅ Correctly reset HP bar
        if (playerHPBar != null)
        {
            playerHPBar.value = playerHPBar.maxValue;
        }

        if (playerDamageScreen != null)
        {
            playerDamageScreen.SetActive(false);
        }

        if (youLoseScreen != null)
        {
            youLoseScreen.SetActive(false);
        }

        
    }


    public void PauseGame()
    {
        Debug.Log("⏸ Game Paused");
        CurrentState = GameState.Paused;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Debug.Log("▶️ Game Resumed");
        CurrentState = GameState.Playing;
        Time.timeScale = 1f;
    }

    public void EndGame()
    {
        Debug.Log("🏁 Game Over");
        CurrentState = GameState.GameOver;

        if (youLoseScreen != null)
        {
            youLoseScreen.SetActive(true);
        }
    }

    // Optional: respawn system
    public void SpawnPlayer(GameObject playerPrefab)
    {
        if (playerSpawnPos != null && playerPrefab != null)
        {
            Instantiate(playerPrefab, playerSpawnPos.position, playerSpawnPos.rotation);
        } else
        {
            Debug.LogWarning("⚠️ Missing playerPrefab or playerSpawnPos!");
        }
    }

    public void WinGame()
    {
        Debug.Log("🏆 You Win!");
        CurrentState = GameState.Win;

        if (winMenuUI != null)
        {
            winMenuUI.SetActive(true);
        } else
        {
            Debug.LogWarning("[GameManager] winMenuUI not assigned!");
        }

        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
