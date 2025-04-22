using UnityEngine;
using System;
using System.Runtime.CompilerServices;

public class enemySound : MonoBehaviour
{
    public enum soundType
    {
        //For the sound added, have it in the same order 
        Footstep,
        Sprint,
        Breathing,
        Screeching,
        deathNoise,
        jumpScare
     
    }

    [SerializeField] private enemyList[] enemy;

    private static enemySound instance;
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
        AudioClip[] clips = instance.enemy[(int)sound].enemySounds;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
        instance.audioSource.PlayOneShot(randomClip, volume);
    }

}

[Serializable]
public struct enemyList
{
    public AudioClip[] enemySounds { get => Enemysounds; }

    [SerializeField] private AudioClip[] Enemysounds;
}
