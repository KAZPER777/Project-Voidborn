using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StartScreenManager : MonoBehaviour
{
    public static StartScreenManager Instance;

    [Header("UI Elements")]
    public Text titleText;
    public Button startButton;
    public Button quitButton;
    public Button creditsButton;
    public GameObject creditsPanel;
    public Image fadePanel;
    public GameObject startScreenCanvas;
    public GameObject backButton;
    public GameObject creditsText;

    [Header("Settings")]
    public float fadeDuration = 1.0f;

    private bool showingCredits = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        fadePanel.gameObject.SetActive(false);

        startButton.onClick.AddListener(OnStartButtonPressed);
        quitButton.onClick.AddListener(QuitGame);
        creditsButton.onClick.AddListener(OpenCredits);
    }

    private void OnStartButtonPressed()
    {
        // Disable interaction to avoid double-press
        startButton.interactable = false;

        // Delegate transition to GameManager
        GameManager.Instance.BeginStartSequence();
    }

    private IEnumerator FadeOutAndStart()
    {
        fadePanel.gameObject.SetActive(true);
        fadePanel.color = new Color(0, 0, 0, 0);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadePanel.color = new Color(0, 0, 0, elapsed / fadeDuration);
            yield return null;
        }

        if (startScreenCanvas != null)
        {
            startScreenCanvas.SetActive(false);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
    }

    public void QuitGame()
    {
        Debug.Log("❌ Quitting Game...");
        Application.Quit();
    }

    public void OpenCredits()
    {
        creditsPanel.SetActive(true);

        startButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
        creditsButton.gameObject.SetActive(false);

        Debug.Log("✅ Opened Credits.");
    }

    public void CloseCredits()
    {
        creditsPanel.SetActive(false);

        startButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
        creditsButton.gameObject.SetActive(true);

        Debug.Log("✅ Closed Credits.");
    }



public bool IsStartScreenActive()
    {
        return startScreenCanvas != null && startScreenCanvas.activeSelf;
    }
}
