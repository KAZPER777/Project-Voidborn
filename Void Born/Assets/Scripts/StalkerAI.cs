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

    public Color rageColor = Color.red;

    // Configurable
    public AudioClip rageSound;
    public float ragePauseDuration = 2.61f;

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
    }

    void Update()
    {
        // Check if we need to trigger rage
        if (!hasRaged && check.isVisible)
        {
            TriggerRage();
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

        // Constant chase logic
        if (isChasing)
        {
            Vector3 playerPos = playerTransform.position + Vector3.up * 1f;
            agent.SetDestination(playerPos);
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
            Color boostedRageColor = Color.red * 12f; // Brighter red
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
}
