using UnityEngine;

public class soundManager : MonoBehaviour
{
    public enum soundType
    {
        Footstep,
        Sprint,
        Jump,
    }
    
    [SerializeField] private AudioClip[] soundList;
    

    private static soundManager instance;
    private AudioSource audioSource;


    private void Awake()
    {
        instance = this;
    }

    public static void playSound(soundType sound,[SerializeField] float volume)
    {
        instance.audioSource.PlayOneShot(instance.soundList[(int)sound], volume);
    }
}
