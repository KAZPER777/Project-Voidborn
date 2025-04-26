using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class TheDemonManager : MonoBehaviour //The purpose of this class is to be a manager for the demon so we can write cleaner code
{
    [Header("References")]
    public static TheDemonManager instance { get; private set; }
    public NavMeshAgent agent;
    public Animator animate;
    public Transform playerTransform;
    public Transform demonTransform;
    public JaidensPlayerController player;
    public CharacterController playerController;
    public CheckVisiblity visiblity;
    public SkinnedMeshRenderer bodyRenderer;


    [Header("Sanity Timers")]
    public float sanityTimer;
    public float attackTimer;

    [Header("Teleportation")]
    public int teleportSphereRadius;
    public float teleportChance;
    public float teleportTimer;
    private Vector3 teleportDestination;
    private float teleportDelay = 2f;


    //Const strings for animations
    public const string IS_FROZEN = "isFrozen";
    public const string IS_ATTACKING = "isAttacking";
    private bool isAttacking = false;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        player.currentSanity = player.maxSanity;
    }

    public bool CheckVisibility()
    {
        Vector3 monsterHeight = demonTransform.transform.position + Vector3.up * -0.5f;
        Vector3 playerHeight = playerTransform.transform.position + Vector3.up * 0.5f;
        Vector3 directionToPlayer = (playerHeight - monsterHeight).normalized;
        RaycastHit hit;

        if (Physics.Raycast(monsterHeight, directionToPlayer, out hit, 20f))
        {
            if (hit.transform == playerTransform)
            {
                demonTransform.transform.LookAt(playerTransform);
                Debug.Log("Raycast returned true");
                return true;
                
            }
            else
            {
                Debug.Log("Blocked View");
                return false;
            }
        }
        Debug.Log("returned false");
        return false;

    }
    public void DrawRay()
    {
        Vector3 monsterHeight = demonTransform.transform.position + Vector3.up * 0.5f;
        Vector3 playerHeight = playerTransform.transform.position + Vector3.up * 0.5f;
        Vector3 directionToPlayer = (playerHeight - monsterHeight).normalized;
        Debug.DrawRay(monsterHeight, directionToPlayer * 20f, Color.blue);
        Debug.Log("Drawing Ray!");
    }

    public void LookingAtDemon(int amount)
    {
        if (visiblity.isVisible && CheckVisibility())
        {
            sanityTimer -= Time.deltaTime;

            if (sanityTimer <= 0)
            {
                sanityTimer = 1f;
                player.currentSanity -= amount;
                GameManager.Instance.sanityBar.fillAmount = (float)player.currentSanity / player.maxSanity;
            }

        }
        else
        {
            sanityTimer = 1f;
        }
    }

    public void TeleportDemon()
    {
        
        teleportTimer -= Time.deltaTime;

        if (bodyRenderer.enabled == false)
        {
            sanityTimer = 1f;
        }

        if (teleportTimer <= 0f)
        {
            teleportTimer = 2f;
            if (Random.value < teleportChance)
            {
                bodyRenderer.enabled = false;
                
                Debug.Log("Successful Chance!");

                if (Vector3.Distance(demonTransform.transform.position, teleportDestination) > 1f)
                { 
                    agent.speed = 100f;
                    agent.acceleration = 100f;
                    demonTransform.transform.LookAt(playerTransform);
                    demonTransform.transform.forward = playerTransform.transform.forward;
                    teleportDestination = PickRoamDestination();
                    agent.SetDestination(teleportDestination);
                    teleportTimer = teleportDelay;

                    Debug.Log("Teleported!");
                }
                

            }
            else
            {
                bodyRenderer.enabled = true;

            }


        }
       

    }


    public void OnSanityZero() //work in progress
    {
        if (player.currentSanity <= 0) //if sanity is zero then attack the player
        {
            player.currentSanity = 0;
            animate.SetBool(IS_FROZEN, false);
            animate.SetBool(IS_ATTACKING, true);

            attackTimer -= Time.deltaTime; //decrement timer 
            if (attackTimer <= 0)
            {
                attackTimer = 2f; //reset attack timer

            }

        }
    }

    Vector3 PickRoamDestination() //pick teleport destination
    { 

            Vector3 randomOffset = Random.insideUnitSphere * teleportSphereRadius;
            Vector3 target = playerTransform.position + randomOffset;

            // Check if it's a valid NavMesh position
            NavMeshHit navHit;
            if (NavMesh.SamplePosition(target, out navHit, teleportSphereRadius, NavMesh.AllAreas))
            {
                return navHit.position;
            }
        

        // If chance failed or no valid point, stay in current position
        return demonTransform.transform.position;
    }

}
