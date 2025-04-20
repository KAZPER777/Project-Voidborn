using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Panels")]
    public GameObject pauseMenu;
    public GameObject hud;

    [Header("Battery")]
    public Image batteryBar;
    public TMP_Text batteryText;

    void Awake()
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

    public void UpdateBattery(float value)
    {
        batteryBar.fillAmount = value;
        batteryText.text = Mathf.RoundToInt(value * 100f) + "%";
    }

    public void ShowPauseMenu(bool show)
    {
        pauseMenu.SetActive(show);
        hud.SetActive(!show);
    }
}