using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    public Slider volumeSlider;
    public TMP_Text volumeLabel;

    void Start()
    {
        volumeSlider.onValueChanged.AddListener(SetVolume);
        volumeSlider.value = AudioListener.volume;
        UpdateVolumeLabel(volumeSlider.value);
    }

    void SetVolume(float value)
    {
        AudioListener.volume = value;
        UpdateVolumeLabel(value);
    }

    void UpdateVolumeLabel(float value)
    {
        if (volumeLabel != null)
            volumeLabel.text = "Volume: " + Mathf.RoundToInt(value * 100f) + "%";
    }
}
