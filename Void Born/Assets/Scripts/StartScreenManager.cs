using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
using TMPro;

public class StartScreenManager : MonoBehaviour
{
    
    public static StartScreenManager Instance;

    [Header("UI References")]
    public TMP_Text titleText;           
    public Button startButton;
    public Button quitButton;
    public Button creditsButton;
    public Button backButton;            
    public GameObject creditsPanel;      
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
        
        if (creditsPanel != null)
            creditsPanel.SetActive(false);

    
        startButton.onClick.AddListener(OnStartClicked);
        quitButton.onClick.AddListener(OnQuitClicked);
        creditsButton.onClick.AddListener(() => ToggleCredits(true));
        backButton.onClick.AddListener(() => ToggleCredits(false));
    }

    private void OnStartClicked()
    {
       
        if (startScreenCanvas != null)
            startScreenCanvas.SetActive(false);

        GameManager.Instance?.StartGame();
    }

    private void OnQuitClicked()
    {
#if UNITY_EDITOR
       
        EditorApplication.isPlaying = false;
#else
        // Quit the standalone build
        Application.Quit();
#endif
    }

    
    public void ToggleCredits(bool show = false)
    {
        showingCredits = show;

        if (creditsPanel != null)
        {
            creditsPanel.SetActive(showingCredits);
            
            creditsPanel.transform.SetAsLastSibling();
        }

      
        startButton.gameObject.SetActive(!showingCredits);
        quitButton.gameObject.SetActive(!showingCredits);
        creditsButton.gameObject.SetActive(!showingCredits);
    }

  
    public bool IsStartScreenActive()
    {
        return startScreenCanvas != null && startScreenCanvas.activeSelf;
    }
}