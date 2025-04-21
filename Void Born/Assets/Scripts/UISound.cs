using UnityEngine;
using System;
using System.Runtime.CompilerServices;

public class UIsound : MonoBehaviour
{
    public enum soundType
    {
        //For the sound added, have it in the same order 
        Doors,
        pauseMenu,
        //mainMenu (if it is added)
        menuSelection,
        menuScroll,
        randomNoise,
        airVents,
        Music, //Doing elevator music because that will be funny
        
        //Player UI
        lowHealth,
        healthRegen
        
    }

    [SerializeField] private UIList[] UI;

    private static UIsound instance;
    private AudioSource audioSource;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public static void playSound(soundType sound, float volume)
    {
        AudioClip[] clips = instance.UI[(int)sound].uiSounds;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
        instance.audioSource.PlayOneShot(randomClip, volume);
    }

}

[Serializable]
public struct UIList
{
    public AudioClip[] uiSounds { get => UIsounds; }

    [SerializeField] private AudioClip[] UIsounds;
}