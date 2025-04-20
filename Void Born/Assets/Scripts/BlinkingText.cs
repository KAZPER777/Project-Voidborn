using UnityEngine;
using TMPro;

public class BlinkingText : MonoBehaviour
{
    public float blinkSpeed = 0.5f;
    private TMP_Text text;
    private float timer;

    void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    void Update()
    {
        timer += Time.unscaledDeltaTime; // unscaled so it works while paused

        if (timer >= blinkSpeed)
        {
            text.enabled = !text.enabled;
            timer = 0f;
        }
    }
}
