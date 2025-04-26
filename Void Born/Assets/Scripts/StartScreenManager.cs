using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StartScreenManager : MonoBehaviour
{
    public static StartScreenManager Instance;

    public Text titleText;
    public Button startButton;
    public Button quitButton;
    public Button creditsButton;
    public GameObject creditsPanel;
    public Image fadePanel;
    public float fadeDuration = 1.0f;
    public GameObject startScreenCanvas;

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

        startButton.onClick.AddListener(StartGame);
        quitButton.onClick.AddListener(QuitGame);
        creditsButton.onClick.AddListener(ToggleCredits);
    }

    public void StartGame()
    {
        StartCoroutine(FadeOutAndStart());
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
        Application.Quit();
    }

    public void ToggleCredits()
    {
        showingCredits = !showingCredits;
        creditsPanel.SetActive(showingCredits);

        startButton.gameObject.SetActive(!showingCredits);
        quitButton.gameObject.SetActive(!showingCredits);
        creditsButton.gameObject.SetActive(!showingCredits);
    }

   
    public bool IsStartScreenActive()
    {
        return startScreenCanvas != null && startScreenCanvas.activeSelf;
    }
}
