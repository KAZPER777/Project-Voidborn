using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

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

    [Header("Flashlight Prompt")]
    public TextMeshProUGUI flashlightPromptText;
    public float flashlightPromptDuration = 5f; // Seconds to fade out


    [Header("UI Elements")]
    public GameObject winMenuUI;
    public GameObject checkpointPopup;
    public GameObject keyObjective; //for key objective
    public GameObject playerDamageScreen;
    public GameObject youLoseScreen;
    public Slider playerHPBar;
	public Image sanityBar;

    [Header("Pause Menu")]
    public GameObject pauseMenuUI;
    public GameObject hudUI;
    public GameObject controlsPanel;

    private bool isPaused = false;
    public bool gameStarted = false;
    public bool wonGame = false;
    private bool flashlightFaded = false;

    [Header("Checkpoint System")]
    [SerializeField] private Transform currentCheckpoint;

    public GameState CurrentState { get; private set; } = GameState.StartScreen;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); // Optional if you switch scenes
        } else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }

        if (flashlightPromptText != null && !flashlightFaded)
        {
            Debug.Log("Not null flaslight");
            flashlightPromptText.gameObject.SetActive(true);
            flashlightPromptText.alpha = 1f;
            StartCoroutine(FadeOutFlashlightPrompt());
        }
    }

    public void StartGame()
    {
        Debug.Log("Game Started!");
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
        pauseMenuUI.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        CurrentState = GameState.Paused;
        Time.timeScale = 0f;
    }

    public void OpenControls()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (controlsPanel != null) controlsPanel.SetActive(true);
    }

    public void CloseControls()
    {
        if (controlsPanel != null) controlsPanel.SetActive(false);
        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        CurrentState = GameState.Playing;
        Time.timeScale = 1f;
    }

    public void EndGame()
    {
        Debug.Log("Game Over");
        CurrentState = GameState.GameOver;

        if (youLoseScreen != null)
        {
            youLoseScreen.SetActive(true);
            

        }
    }


    public void SetObjective(string newObjective)
    {
        if (keyObjective != null)
        {
            
          
                TMP_Text textComponent = keyObjective.GetComponent<TMP_Text>();
                if (textComponent != null)
                {
                    textComponent.text = newObjective;
                }
                else
                {
                    Debug.LogWarning("No TMP_Text component found on GameObject.");
                }
            
            
        }
        else
        {
            Debug.LogWarning("GameObject not assigned in GameManager.");
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
            wonGame = true;
        } else
        {
            Debug.LogWarning("[GameManager] winMenuUI not assigned!");
            wonGame = false;
        }

        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private IEnumerator FadeOutFlashlightPrompt()
    {
        yield return new WaitForSeconds(2f); // Wait before starting fade

        float elapsed = 0f;
        float duration = flashlightPromptDuration;
        Color originalColor = flashlightPromptText.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            flashlightPromptText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        flashlightFaded = true;
        flashlightPromptText.gameObject.SetActive(false);
       
    }

}
