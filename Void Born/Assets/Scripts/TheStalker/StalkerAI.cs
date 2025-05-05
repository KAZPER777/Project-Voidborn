using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Analytics;

using UnityEngine;
using UnityEngine.AI;

public class StalkerAI : MonoBehaviour
{

    // References
    public NavMeshAgent agent;
    public AudioSource source;
    public Transform playerTransform;
    public Animator animator;
    public CheckVisiblity check;
    public Renderer eyeRenderer;
    public Collider monsterCollider;
    public JaidensPlayerController jaidensPlayer;
    public CharacterController playerController;
    public LayerMask player;
    private Color originalEmissionColor;
    public FlashlightCheck checkFlashlight;


    public Color rageColor = Color.red;

    // Configurable
    public AudioClip rageSound;
    public float ragePauseDuration = 2.61f;
    public float stareDurationThreshold = 3f;
    private float currentStareTime = 0f;
    [SerializeField]private float stalkTimer = 4f;
    private float animationReset = 1.5f;

    //Damage
    public float damageAmount = 100f;
    public float damageRange = 1.8f;
    public float damageCooldown = 1f;

    private float damageTimer = 0f;

    // Roaming settings
    public float roamRadius = 10f;
    [SerializeField] float playerSeekChance = 0.3f; // 30% chance to drift toward player
    public float roamDelay = 3f;

    private float roamTimer = 0f;
    private Vector3 roamDestination;

    //Footsteps
    public AudioClip[] footstepSounds;
    public float footstepInterval = 0.8f; // time between footsteps
    private float footstepTimer = 0f;
    private int footstepIndex = 0;
    //Raged Footsteps
    public float rageFootstepInterval = 0.3f;
    private float rageFootstepTimer = 0f;
    private int rageFootstepIndex = 0;


    // State tracking
    private bool hasRaged = false;
    private bool isChasing = false;
    private float rageTimer = 0f;

    // Animator parameter keys
    private const string IS_ENRAGED = "isEnraged";
    private const string IS_RUNNING = "isRunning";

    void Start()
    {
        //Get Components to automatically assign
        agent = GetComponent<NavMeshAgent>();
        source = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();

        if (monsterCollider == null)
            monsterCollider = GetComponent<Collider>();

        if (eyeRenderer != null)
        {
            originalEmissionColor = eyeRenderer.material.GetColor("_EmissionColor");
        }
    }

    void Update()
    {
        CanSeePlayer();
        CanSeePlayerEnraged();
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); //Where raycast should point
        RaycastHit hit;

        bool isPlayerStaring = false;

        if (Physics.Raycast(ray, out hit, 60f)) // distance threshold for how far player can trigger Stalker Rage
        {
            if (hit.collider == monsterCollider)
            {
                Debug.Log("staring at monster");
                isPlayerStaring = true;
            }
        }

        Vector3 origin = Camera.main.transform.position;
        Vector3 direction = Camera.main.transform.forward;

       
        //if hasnt raged, checks if the player is staring and increments time if they are
        if (!hasRaged)
        {
          
            /* if(checkFlashlight.CheckInFlashLightRange())
           {
                
                Debug.Log("Working?");
                hasRaged = true;
           }
           else
           {
                Debug.Log("Not Working");
           }*/

            if (isPlayerStaring)
            {
                currentStareTime += Time.deltaTime;
                Debug.Log("Player is staring...");
                if (currentStareTime >= stareDurationThreshold)
                {
                    animator.SetBool("isMoving", false);
                    TriggerRage();
                }
            }
            else
            {
                Debug.Log("not staring");
                currentStareTime = 0f; //reset timer
            }
        }

        // Pauses to finish animation then start chasing
        if (hasRaged && !isChasing)
        {
            rageTimer -= Time.deltaTime;
            if (rageTimer <= 0f)
            {
                StartChasing();
            }
        }

        //Logic for when stalker starts chasing
        if (isChasing)
        {
            agent.speed = 6;
            Vector3 playerPos = playerTransform.position + Vector3.up * 1f;
            agent.SetDestination(playerPos);
            if(!CanSeePlayerEnraged()  && isChasing)
            {
                stalkTimer -= Time.deltaTime;
                if(stalkTimer <= 0f)
                {

                    CantSeePlayer();
                }
            }
            
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= damageRange && damageTimer <= 0f)
            {
                animator.SetBool("isAttacking", true);
                damageTimer = damageCooldown;

                Debug.Log("Monster started attack animation.");
            }

            damageTimer -= Time.deltaTime;


        }

        //checks if rage and chasing isnt true. Allows Stalker to roam to a random desination
        if (!hasRaged && !isChasing)
        {
            roamTimer -= Time.deltaTime;

            if (roamTimer <= 0f || Vector3.Distance(transform.position, roamDestination) < 1f)
            {
                roamDestination = PickRoamDestination();
                animator.SetBool("isMoving", true);
                agent.SetDestination(roamDestination);
                roamTimer = roamDelay;

                Debug.Log("Monster is roaming to: " + roamDestination);
            }
           
        }

        //If stalker is not moving (reached its destination after roaming, set moving to false)
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance) 
        {
            animator.SetBool("isMoving", false);
        }


        // Play footstep sounds if moving
        if (agent.velocity.magnitude > 0.1f && agent.remainingDistance > agent.stoppingDistance && !hasRaged)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f && footstepSounds.Length > 0)
            {
                source.PlayOneShot(footstepSounds[footstepIndex], 0.6f);

                footstepIndex = (footstepIndex + 1) % footstepSounds.Length; // cycle through 0 - 7
                footstepTimer = footstepInterval;
            }
        }

        if (agent.velocity.magnitude > 0.1f && agent.remainingDistance > agent.stoppingDistance && hasRaged)
        {
            rageFootstepTimer -= Time.deltaTime;
            if (rageFootstepTimer <= 0f && footstepSounds.Length > 0)
            {
                source.PlayOneShot(footstepSounds[rageFootstepIndex], 0.8f);

                rageFootstepIndex = (rageFootstepIndex + 1) % footstepSounds.Length; // cycle through 0 - 7
                rageFootstepTimer = rageFootstepInterval;
            }
        }

    }


    //When attack animation ends it executes this method
    public void OnAttackAnimationEnd()
    {
        if (playerTransform.TryGetComponent<IDamageable>(out var damageable))
        {

            damageable.TakeDamage(damageAmount);
            GameManager.Instance.playerHPBar.fillAmount = jaidensPlayer.currentHealth / jaidensPlayer.maxHealth;
        }
        animator.SetBool("isAttacking", false);
        animator.SetBool("isEnraged", false);
    }

    void TriggerRage()
    {
        
        hasRaged = true;
        rageTimer = ragePauseDuration;

        // Stop Stalker movement 
        agent.isStopped = true;

        // Set animation triggers to true (triggering the rage)
        animator.SetBool(IS_ENRAGED, true);
        animator.SetBool(IS_RUNNING, true);

        if (eyeRenderer != null)
        {
            Color boostedRageColor = Color.red * 10f; // Brighter red for eye color when enraged
            eyeRenderer.material.SetColor("_EmissionColor", boostedRageColor);
            DynamicGI.SetEmissive(eyeRenderer, boostedRageColor); // Sets the emissions renderer of the eye to boostedRageColor (red)
        }

        // Play rage sound
        if (rageSound != null)
        {
            source.PlayOneShot(rageSound);
        }

        Debug.Log("Monster has entered Rage mode.");
    }

    void StartChasing()
    {
        isChasing = true;
        agent.isStopped = false;

        Debug.Log("Monster is now chasing the player.");
    }






    private bool CanSeePlayer()
    {
        RaycastHit hit;
        Vector3 monsterHeight = transform.position + Vector3.up * 1f;
        Vector3 playerHeight = playerTransform.transform.position + Vector3.up * 1f;
        Vector3 directionToPlayer = (playerHeight - monsterHeight).normalized;
        Debug.DrawRay(monsterHeight, directionToPlayer * 6f, Color.yellow);
        if (Physics.Raycast(monsterHeight, directionToPlayer, out hit, 6f))
        {
            // Check if the raycast hit the player
            if (hit.transform == playerTransform)
            {
                Debug.Log("Monster can see player (nothing blocking view)");
                return true;
            }
            else
            {
                Debug.Log("Something is blocking the view: " + hit.transform.name);
            }
        }

        Debug.Log("Monster cannot see player");
        return false;
    }

    //The purpose of this raycast is to check the range of which the stalker can see the player while enraged. *2 range increase
    private bool CanSeePlayerEnraged()
    {
        RaycastHit hit;
        Vector3 monsterHeight = transform.position + Vector3.up * 1f;
        Vector3 playerHeight = playerTransform.transform.position + Vector3.up * 1f;
        Vector3 directionToPlayer = (playerHeight - monsterHeight).normalized;
        Debug.DrawRay(monsterHeight, directionToPlayer * 20f, Color.red);
        if (Physics.Raycast(monsterHeight, directionToPlayer, out hit, 20f))
        {
            // Check if the raycast hit the player
            if (hit.transform == playerTransform)
            {
                Debug.Log("Monster can see player while Enraged! (nothing blocking view)");
                return true;
            }
            else
            {
                Debug.Log("Something is blocking the view.. Monster will lose aggro soon.");
            }
        }

        Debug.Log("Monster cannot see player.. Monster will lose aggro soon.");
        return false;
    }

    public void CantSeePlayer()
    {
        hasRaged = false;
        isChasing = false;
        stalkTimer = 10f;
        if (agent.velocity.magnitude > 0.1f && agent.remainingDistance > agent.stoppingDistance)
        {
            agent.speed = 4;
            animator.SetBool(IS_ENRAGED, false);
            animator.SetBool(IS_RUNNING, false);
        }
        if (eyeRenderer != null)
        {
            eyeRenderer.material.SetColor("_EmissionColor", originalEmissionColor);
            DynamicGI.SetEmissive(eyeRenderer, originalEmissionColor);
        }
    }
    //Stalker roam logic 
    Vector3 PickRoamDestination()
    {
        Vector3 target;

        //If playerSeekChance is higher than Random.Value then pick a point near the player to travel to
        if (Random.value < playerSeekChance)
        {
           
            Vector3 randomOffset = Random.insideUnitSphere * roamRadius * 0.5f;
            target = playerTransform.position + randomOffset;
        }
        else
        {
           //If Random.Value greater than playerSeekChance then roam 
            Vector3 randomOffset = Random.insideUnitSphere * roamRadius;
            target = transform.position + randomOffset;
        }

        //just to check if the ai target area is walking somewhere on the navmesh
        NavMeshHit navHit;
        if (NavMesh.SamplePosition(target, out navHit, roamRadius, NavMesh.AllAreas))
        {
            return navHit.position;
        }
        else
        {
            //if not stay in place
            return transform.position; 
        }
    }


}
