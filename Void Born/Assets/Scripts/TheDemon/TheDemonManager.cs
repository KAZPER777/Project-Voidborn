using UnityEngine;
using UnityEngine.UIElements;

public class TheDemonManager : MonoBehaviour //The purpose of this class is to be a manager for the demon so we can write cleaner code
{
    public static TheDemonManager instance { get; private set; }
    public Animator animate;
    public Transform playerTransform;
    public JaidensPlayerController player;
    public CheckVisiblity visiblity { get; private set; }

    public int teleportSphereRadius;

    public const string IS_WALKING = "isWalking";
    public const string IS_ATTACKING = "isAttacking";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Initialize Components
        animate = GetComponent<Animator>();
        visiblity = GetComponent<CheckVisiblity>();
        //player = GetComponent<JaidensPlayerController>();

    }

    
    public void LookingAtDemon()
    {
        if(visiblity.isVisible)
        {

        }
    }









}
