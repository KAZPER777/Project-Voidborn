using System.Collections;
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
    public GameObject eyesObject;
    public GameObject blackSpawnEffect;
    public GameObject blackSparks;

    [Header("Attack Settings")]
    public float attackSpeed = 100f;
    public float attackRange = 4f; 
    public int attackDamage = 100; 
    private bool isChasingPlayer = false;

    [Header("Audio")]
    public AudioSource stareAudioSource;
    public AudioSource jumpscareAudioSource;
    private bool isBeingLookedAt = false;


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
            PlayStareSound();

        }
        else
        {
            sanityTimer = 1f;
            PauseStareSound();
        }
    }

    private bool isFadingOut = false;

    void PlayStareSound()
    {
        if (stareAudioSource != null)
        {
            if (!stareAudioSource.isPlaying)
            {
                
                stareAudioSource.Play();
                StartCoroutine(FadeInSound(0f, 1f, 2f)); 
            }
            else if (isFadingOut) 
            {
                StopCoroutine(FadeOutSound(2f));
                isFadingOut = false;
                StartCoroutine(FadeInSound(stareAudioSource.volume, 1f, 2f)); 
            }
        }
    }

    void PauseStareSound()
    {
        if (stareAudioSource != null && stareAudioSource.isPlaying)
        {
            
            StartCoroutine(FadeOutSound(2f));
        }
    }

    
    IEnumerator FadeInSound(float startVolume, float targetVolume, float duration)
    {
        float timeElapsed = 0f;
        stareAudioSource.volume = startVolume;

        while (timeElapsed < duration)
        {
            stareAudioSource.volume = Mathf.Lerp(startVolume, targetVolume, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        stareAudioSource.volume = targetVolume; 
    }

    // Coroutine to gradually fade out the sound
    IEnumerator FadeOutSound(float duration)
    {
        isFadingOut = true;
        float startVolume = stareAudioSource.volume;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            stareAudioSource.volume = Mathf.Lerp(startVolume, 0f, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        stareAudioSource.volume = 0f;
        stareAudioSource.Stop(); 
        isFadingOut = false;
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
                eyesObject.SetActive(false);
                blackSparks.SetActive(true);
                blackSpawnEffect.SetActive(false);

                Debug.Log("Successful Chance!");

                if (Vector3.Distance(demonTransform.transform.position, teleportDestination) > 1f)
                {
                    // ** Decide the radius based on visibility **
                    float actualRadius = teleportSphereRadius;
                    if (!CheckVisibility())
                    {
                        actualRadius = teleportSphereRadius * 2.5f; // Double the radius if player can't see demon
                        Debug.Log("Player not looking. Using bigger teleport radius.");
                    }
                    else
                    {
                        Debug.Log("Player looking. Using normal teleport radius.");
                    }

                    teleportDestination = PickRoamDestination(actualRadius);
                    agent.Warp(teleportDestination);
                    teleportTimer = teleportDelay;

                    Debug.Log("Teleported!");
                }

            }
            else
            {
                bodyRenderer.enabled = true;
                eyesObject.SetActive(true);
                blackSparks.SetActive(false);
                blackSpawnEffect.SetActive(true);
            }
        }


    }


    public void OnSanityZero() //work in progress
    {
        if (player.currentSanity <= 0 && !isChasingPlayer)
        {
            player.currentSanity = 0;
            animate.SetBool(IS_FROZEN, false);
            animate.SetBool(IS_ATTACKING, true);
            isChasingPlayer = true;
            agent.acceleration = 100f;
            agent.speed = attackSpeed; 
            agent.stoppingDistance = attackRange;

            if (jumpscareAudioSource != null && !jumpscareAudioSource.isPlaying)
            {
                jumpscareAudioSource.Play();
            }
        }

        if (isChasingPlayer)
        {
            agent.SetDestination(playerTransform.transform.position);

            
            if (Vector3.Distance(demonTransform.transform.position, playerTransform.transform.position) <= attackRange)
            {
                AttackPlayer();
            }


        }
    }

    private void AttackPlayer()
    {
        if (!isAttacking) 
        {
            isAttacking = true;
            player.TakeDamage(attackDamage); 
            Debug.Log("Player has been attacked by the Demon!");
        }
    }

    Vector3 PickRoamDestination(float radius) 
    {

        Vector3 randomOffset = Random.insideUnitSphere * radius;
        Vector3 target = playerTransform.position + randomOffset;

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(target, out navHit, radius, NavMesh.AllAreas))
        {
            return navHit.position;
        }

        return demonTransform.transform.position;
    }


   




}
