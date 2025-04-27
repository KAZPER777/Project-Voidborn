using UnityEngine;

public class PlaySoundExit : StateMachineBehaviour
{
    [SerializeField] private soundManager.soundType sound;
    [Range(0, 1)][SerializeField] private float volume = 1;

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        soundManager.playSound(sound, volume);

    }


}
