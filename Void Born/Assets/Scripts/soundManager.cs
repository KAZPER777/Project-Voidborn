using UnityEngine;

public class soundManager : MonoBehaviour
{
    public static soundManager instance;

    [SerializeField] private AudioSource soundPrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Optional but recommended
        } else
        {
            Destroy(gameObject);
        }
    }

    public void playSound(AudioClip clip, Transform soundTransform, float volume)
    {
        if (soundPrefab == null)
        {
            Debug.LogError("❌ Sound Prefab is not assigned in SoundManager!");
            return;
        }

        AudioSource source = Instantiate(soundPrefab, soundTransform.position, Quaternion.identity);
        source.clip = clip;
        source.volume = volume;

        source.Play();
        Destroy(source.gameObject, source.clip.length);
    }
}
