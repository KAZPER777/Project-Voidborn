using UnityEngine;
using System;
using System.Runtime.CompilerServices;

public class soundManager : MonoBehaviour
{
    public enum soundType
    {
        //For the sound added, have it in the same order 
        Footstep,
        Sprint,
        Jump,
    }

    [SerializeField] private soundList[] soundList;

    private static soundManager instance;
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
        AudioClip[] clips = instance.soundList[(int)sound].Sounds; 
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
        instance.audioSource.PlayOneShot(randomClip, volume);
    }

}

[Serializable]
public struct soundList
{
    public AudioClip[] Sounds { get => sounds; }
   
    [SerializeField] private AudioClip[] sounds;
}
