using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player Settings")]
    public JaidensPlayerController playerScript;
    public Transform playerSpawnPos;

    [Header("UI Elements")]
    public GameObject winMenuUI;
    public GameObject checkpointPopup;
    public GameObject playerdamagescreen;
    public GameObject YouLose;
    public Image playerHPBar;

    [Header("Pause Menu")]
    public GameObject pauseMenuUI;
    public GameObject hudUI;

    private bool isPaused = false;

    [Header("Checkpoint System")]
    [SerializeField] private Transform currentCheckpoint;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
        }
        else
        {
            Destroy(gameObject);
        }

     
    }

    private void Start()
    {
        
        if (winMenuUI != null) winMenuUI.SetActive(false);
        if (checkpointPopup != null) checkpointPopup.SetActive(false);
        if (playerdamagescreen != null) playerdamagescreen.SetActive(false);
        if (YouLose != null) YouLose.SetActive(false);
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
    }



    private void Update()
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

        if (pauseMenuUI != null) pauseMenuUI.SetActive(isPaused);
        if (hudUI != null) hudUI.SetActive(!isPaused);

        Cursor.visible = isPaused;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;

        // Optional UIManager integration
        if (UIManager.Instance != null)
            UIManager.Instance.ShowPauseMenu(isPaused);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (hudUI != null) hudUI.SetActive(true);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (UIManager.Instance != null)
            UIManager.Instance.ShowPauseMenu(false);
    }

    public void WinGame(Transform checkpointOverride = null)
    {
        Debug.Log("[GameManager] WinGame() called.");

        if (checkpointOverride != null)
            SetCheckpoint(checkpointOverride);

        if (winMenuUI != null)
            winMenuUI.SetActive(true);
        else
            Debug.LogWarning("[GameManager] winMenuUI not assigned!");

        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ShowLoseScreen()
    {
        if (YouLose != null)
            YouLose.SetActive(true);
    }

    public void SetCheckpoint(Transform newCheckpoint)
    {
        currentCheckpoint = newCheckpoint;
        Debug.Log("[GameManager] Checkpoint set to: " + newCheckpoint.name);

        PlayerPrefs.SetFloat("CheckpointX", newCheckpoint.position.x);
        PlayerPrefs.SetFloat("CheckpointY", newCheckpoint.position.y);
        PlayerPrefs.SetFloat("CheckpointZ", newCheckpoint.position.z);
        PlayerPrefs.Save();

        if (checkpointPopup != null)
            StartCoroutine(ShowCheckpointRoutine(2f));
    }

    public void RespawnPlayer()
    {
        if (playerScript != null && currentCheckpoint != null)
        {
            playerScript.transform.position = currentCheckpoint.position;
            playerScript.SpawnPlayer();
        }
    }

    private IEnumerator ShowCheckpointRoutine(float duration)
    {
        checkpointPopup.SetActive(true);
        yield return new WaitForSecondsRealtime(duration);
        checkpointPopup.SetActive(false);
    }

    
}
