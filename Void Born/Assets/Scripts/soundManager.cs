using UnityEngine;

public class soundManager : MonoBehaviour
{
    public static soundManager instance;

    [SerializeField] private string soundPrefabPath = "Prefabs/SoundPrefab";
    private AudioSource soundPrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            LoadSoundPrefab(); // Load prefab once
        } else
        {
            Destroy(gameObject);
        }
    }

    private void LoadSoundPrefab()
    {
        GameObject prefabGO = Resources.Load<GameObject>(soundPrefabPath);
        if (prefabGO != null)
        {
            soundPrefab = prefabGO.GetComponent<AudioSource>();
        }

        if (soundPrefab == null)
        {
            Debug.LogError("Failed to load soundPrefab from Resources!");
        }
    }

    public void playSound(AudioClip clip, Transform soundTransform, float volume)
    {
        if (soundPrefab == null)
        {
            Debug.LogError("Sound Prefab is not assigned or failed to load in SoundManager!");
            return;
        }

        AudioSource source = Instantiate(soundPrefab, soundTransform.position, Quaternion.identity);
        source.clip = clip;
        source.volume = volume;

        source.Play();
        Destroy(source.gameObject, source.clip.length);
    }
}
