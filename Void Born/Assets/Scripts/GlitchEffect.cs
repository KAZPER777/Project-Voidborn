using UnityEngine;
using UnityEngine.UI;

public class GlitchEffect : MonoBehaviour
{
    public float flickerSpeed = 0.1f;
    private Image glitchImage;

    private void Start()
    {
        glitchImage = GetComponent<Image>();
        InvokeRepeating(nameof(Flicker), flickerSpeed, flickerSpeed);
    }

    private void Flicker()
    {
        if (glitchImage != null)
        {
            glitchImage.enabled = !glitchImage.enabled;
        }
    }
}
