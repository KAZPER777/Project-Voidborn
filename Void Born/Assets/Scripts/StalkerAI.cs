using System.Runtime.CompilerServices;
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


    public Color rageColor = Color.red;

    // Configurable
    public AudioClip rageSound;
    public float ragePauseDuration = 2.61f;
    public float stareDurationThreshold = 3f;
    private float currentStareTime = 0f;


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


    // State tracking
    private bool hasRaged = false;
    private bool isChasing = false;
    private float rageTimer = 0f;

    // Animator parameter keys
    private const string IS_ENRAGED = "isEnraged";
    private const string IS_RUNNING = "isRunning";

    void Start()
    {
        // Optional, in case not assigned in inspector
        agent = GetComponent<NavMeshAgent>();
        source = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();

        if (monsterCollider == null)
            monsterCollider = GetComponent<Collider>();
    }

    void Update()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // center of screen
        RaycastHit hit;

        bool isPlayerStaring = false;

        if (Physics.Raycast(ray, out hit, 200f)) // distance threshold
        {
            if (hit.collider == monsterCollider)
            {
                Debug.Log("staring at monster");
                isPlayerStaring = true;
            }
        }

        Vector3 origin = Camera.main.transform.position;
        Vector3 direction = Camera.main.transform.forward;

       

        if (!hasRaged)
        {
            if (isPlayerStaring)
            {
                currentStareTime += Time.deltaTime;

                if (currentStareTime >= stareDurationThreshold)
                {
                    animator.SetBool("isMoving", false);
                    TriggerRage();
                }
            }
            else
            {
                Debug.Log("not staring");
                currentStareTime = 0f; //reset
            }
        }

        // Handle rage pause timer
        if (hasRaged && !isChasing)
        {
            rageTimer -= Time.deltaTime;
            if (rageTimer <= 0f)
            {
                StartChasing();
            }
        }

        if (isChasing)
        {
            agent.speed = 15;
            Vector3 playerPos = playerTransform.position + Vector3.up * -1f;
            agent.SetDestination(playerPos);

            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= damageRange && damageTimer <= 0f)
            {
                animator.SetBool("isAttacking", true);
                damageTimer = damageCooldown;

                Debug.Log("Monster started attack animation.");
            }

            damageTimer -= Time.deltaTime;
        }

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

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            animator.SetBool("isMoving", false);
        }
    }

    public void OnAttackAnimationEnd()
    {
        if (playerTransform.TryGetComponent<IDamageable>(out var damageable))
        {

            damageable.TakeDamage(damageAmount);
            animator.SetBool("isAttacking", false);
            animator.SetBool("isEnraged", false);
            
        }
    }

    void TriggerRage()
    {
        
        hasRaged = true;
        rageTimer = ragePauseDuration;

        // Stop movement
        agent.isStopped = true;

        // Set animation triggers
        animator.SetBool(IS_ENRAGED, true);
        animator.SetBool(IS_RUNNING, true);

        if (eyeRenderer != null)
        {
            Color boostedRageColor = Color.red * 10f; // Brighter red
            eyeRenderer.material.SetColor("_EmissionColor", boostedRageColor);
            DynamicGI.SetEmissive(eyeRenderer, boostedRageColor); // Optional but useful
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


    Vector3 PickRoamDestination()
    {
        Vector3 target;

        if (Random.value < playerSeekChance)
        {
            // Pick a point near the player
            Vector3 randomOffset = Random.insideUnitSphere * roamRadius * 0.5f;
            target = playerTransform.position + randomOffset;
        }
        else
        {
            // Pick a point near current position
            Vector3 randomOffset = Random.insideUnitSphere * roamRadius;
            target = transform.position + randomOffset;
        }

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(target, out navHit, roamRadius, NavMesh.AllAreas))
        {
            return navHit.position;
        }
        else
        {
            return transform.position; // fallback
        }
    }


}
