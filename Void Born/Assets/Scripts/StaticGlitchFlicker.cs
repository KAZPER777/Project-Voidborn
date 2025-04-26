using UnityEngine;
using UnityEngine.UI;

public class StaticGlitchFlicker : MonoBehaviour
{
    public RawImage glitchImage;
    public float flickerRate = 0.05f; 

    void Start()
    {
        if (glitchImage == null)
            glitchImage = GetComponent<RawImage>();

        InvokeRepeating("ToggleGlitch", 0, flickerRate);
    }

    void ToggleGlitch()
    {
        glitchImage.enabled = !glitchImage.enabled;
    }
}
