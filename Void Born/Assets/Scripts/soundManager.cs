using UnityEngine;
using System;
using System.Runtime.CompilerServices;

public class soundManager : MonoBehaviour
{
   

    public static soundManager instance;

    [SerializeField] private AudioSource sound;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void playSound(AudioClip clip,Transform soundTransform,float volume)
    {
        AudioSource source = Instantiate(sound, soundTransform.position, Quaternion.identity);
        source.clip = clip;
        source.volume = volume;

        source.Play();
        float clipLength = source.clip.length;
        Destroy(source.gameObject,clipLength);
    }
}
