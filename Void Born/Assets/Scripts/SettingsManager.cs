using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;  

public class SettingsManager : MonoBehaviour
{
    [Header("Language")]
    public TMP_Dropdown languageDropdown;
    

    private readonly List<string> languages = new List<string>
    {
        "English",
        "Spanish",
        "French",
       
        
    };

    private const string PrefKey = "LanguageIndex";

    void Awake()
    {
      
        languageDropdown.ClearOptions();
        languageDropdown.AddOptions(languages);

       
        int saved = PlayerPrefs.GetInt(PrefKey, 0);
        languageDropdown.value = Mathf.Clamp(saved, 0, languages.Count - 1);

        languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
    }

    private void OnLanguageChanged(int idx)
    {
        // Save the user’s choice
        PlayerPrefs.SetInt(PrefKey, idx);
        PlayerPrefs.Save();


        Debug.Log($"Language changed to: {languages[idx]}");
    }
}
