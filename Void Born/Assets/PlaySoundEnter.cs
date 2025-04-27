using System.Runtime.CompilerServices;
using UnityEngine;

public class PlaySoundEnter : StateMachineBehaviour
{
    [SerializeField] private soundManager.soundType sound;
    [Range(0,1)] [SerializeField] private float volume = 1;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        soundManager.playSound(sound, volume);

    }

    
}
