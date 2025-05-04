using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class JaidensPlayerController : MonoBehaviour, IDamageable
{
    [Header("Movement Settings")]
    [SerializeField, Range(1f, 10f)] private float walkSpeed;
    [SerializeField, Range(0.1f, .9f)] private float crouchSpeed;
    [SerializeField, Range(0.1f, .9f)] private float crawlSpeed;
    public bool canMove = true;
    private bool canMoveToo = true;
    private float canMoveTimer = 0.5f;
    private float sprintSoundTimer = 1f; 
    private float sprintSoundCooldown = 1f; 

    [Header("Sanity")]
    public float maxSanity;
    public float currentSanity;

    [Header("Sprint Timing")]
    [SerializeField] private float sprintRampUpTime = 1.0f;
    [SerializeField] private float buildUpMult = 1.25f;
    [SerializeField] private float fullSprintMult = 20f;

    [Header("Gravity Settings")]
    [SerializeField] private float gravity = 9.81f;

    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Vault Settings")]
    [SerializeField] private float vaultRange = 1.5f;
    [SerializeField] private float vaultDuration = 2f;
    [SerializeField] private Transform vaultCheckOrigin;
    [SerializeField] private LayerMask vaultLayer;
    [SerializeField] private AnimationCurve vaultCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Capsule Heights")]
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float crouchHeight = 1.2f;
    [SerializeField] private float crawlHeight = 0.6f;

    [Header("Model Offsets")]
    [SerializeField] private float standingOffset = 0f;
    [SerializeField] private float crouchOffset = -0.4f;
    [SerializeField] private float crawlOffset = -0.8f;

    [Header("Components")]
    public CharacterController controller;
    [SerializeField] private Transform visualHolder;
    [SerializeField] private Animator visualAnimator;
    public MeshRenderer meshrender;
    public Animator cameraAnimator;
    public AudioSource audios;

    // Movement state
    private enum MovementState { Standing, Crouching, Crawling }
    private MovementState currentState = MovementState.Standing;

    private float baseWalkSpeed;
    private float sprintHoldTimer = 0f;
    private bool isVaulting = false;
    private bool isJogging = false;

    public bool isCrouching;
    public bool isCrawling;
    [HideInInspector] public bool isMoving;
    [HideInInspector] public bool isRunning;

    public Vector3 moveDir;
    private Vector3 playerVel;

    //Sound
    [Header("Sound")]
    [SerializeField] private AudioClip walkClip;
    [SerializeField] private AudioClip sprintClip;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip breathClip;
    [SerializeField] private AudioClip hurtClip;


    private void Start()
    {
        controller = GetComponent<CharacterController>();
        audios = GetComponent<AudioSource>();
        meshrender.enabled = false;
        Application.runInBackground = true;

        baseWalkSpeed = walkSpeed;
        SetHeight(standingHeight);
        currentHealth = maxHealth;

        SpawnPlayer();
    }

    private void Update()
    {
        if (controller.isGrounded && !isVaulting)
            playerVel.y = 0;

        if (!isVaulting && canMove)
        {
            if (!canMove) return;

            Movement();
            Sprint();
            HandleCrouch();
            HandleCrawl();
            

            if (Input.GetButtonDown("Jump"))
            {
                HandleJumpOrStand();
                TryVault();
                
            }
        }

        ApplyGravity();
        cameraAnimator.SetFloat("Speed", moveDir.magnitude);
    }

    private void Movement()
    {
     
        if (!canMove) return;

        moveDir = (Input.GetAxis("Horizontal") * transform.right +
                   Input.GetAxis("Vertical") * transform.forward).normalized;

        controller.Move(moveDir * walkSpeed * Time.deltaTime);
        isMoving = moveDir != Vector3.zero;


        if (isMoving)
        {
            if (canMoveToo)
            {
               // soundManager.instance.playSound(walkClip, transform, 1f);
                canMoveToo = false;
                canMoveTimer = 1.5f; 
            }
        }

        if (!canMoveToo)
        {
            canMoveTimer -= Time.deltaTime;
            if (canMoveTimer <= 0)
            {
                canMoveToo = true;
            }
        }
    }

    private void Sprint()
    {
        bool sprintHeld = Input.GetButton("Sprint");
        float verticalInput = Input.GetAxis("Vertical");
        bool forward = verticalInput > 0.1f;
        bool backward = verticalInput < -0.1f;
        bool sprintable = currentState == MovementState.Standing && isMoving;

        if (sprintHeld && sprintable)
        {
            if (backward)
            {
                isJogging = true;
                walkSpeed = baseWalkSpeed * 1.1f;
            } else if (forward)
            {
                sprintHoldTimer += Time.deltaTime;
                walkSpeed = sprintHoldTimer >= sprintRampUpTime ? baseWalkSpeed * fullSprintMult : baseWalkSpeed * buildUpMult;
                isRunning = true;
                isJogging = false;
                sprintSoundTimer -= Time.deltaTime;
                if (sprintSoundTimer <= 0f)
                {
                    soundManager.instance.playSound(sprintClip, transform, 1f);
                    sprintSoundTimer = sprintSoundCooldown; // Reset timer
                }
            }
        }
        else
        {
            ResetSprintState();
            sprintSoundTimer = 1f; 
        }
    }

    private void ResetSprintState()
    {
        sprintHoldTimer = 0f;
        isRunning = false;
        isJogging = false;
        walkSpeed = baseWalkSpeed;
    }

    private void HandleJumpOrStand()
    {
        if (currentState == MovementState.Crouching || currentState == MovementState.Crawling && CanStandUp())
        {
            TransitionToState(MovementState.Standing, standingHeight, baseWalkSpeed);
        }
    }

    private void HandleCrouch()
    {
        if (!controller.enabled || isVaulting) return;
        if (!Input.GetButtonDown("Crouch")) return;

        switch (currentState)
        {
            case MovementState.Standing:
                TransitionToState(MovementState.Crouching, crouchHeight, crouchSpeed);
                break;
            case MovementState.Crouching:
                TransitionToState(MovementState.Standing, standingHeight, baseWalkSpeed);
                break;
            case MovementState.Crawling:
                TransitionToState(MovementState.Crouching, crouchHeight, crouchSpeed);
                break;
        }
    }

    private void HandleCrawl()
    {
        if (!controller.enabled || isVaulting) return;

        if (!Input.GetButtonDown("Crawl")) return;

        switch (currentState)
        {
            case MovementState.Standing:
            case MovementState.Crouching:
                TransitionToState(MovementState.Crawling, crawlHeight, crawlSpeed);
                break;
            case MovementState.Crawling:
                TransitionToState(MovementState.Crouching, crouchHeight, crouchSpeed);
                break;
        }
    }

    private void TryVault()
    {
        if (isVaulting) return;

        if (Physics.Raycast(vaultCheckOrigin.position, transform.forward, out RaycastHit hit, vaultRange, vaultLayer))
        {
            StartCoroutine(PerformVault(hit));
        }
    }

    private IEnumerator PerformVault(RaycastHit hit)
    {
        isVaulting = true;
        controller.enabled = false;
        canMove = false;

        visualAnimator?.SetTrigger("Vault");

        Vector3 start = transform.position;
        Vector3 end = hit.point + transform.forward * 0.15f;
        end.y = hit.collider.bounds.max.y + 0.05f + (controller.height / 2f);

        float elapsed = 0f;
        while (elapsed < vaultDuration)
        {
            float t = elapsed / vaultDuration;
            float yArc = Mathf.Sin(vaultCurve.Evaluate(t) * Mathf.PI) * 0.15f;

            transform.position = Vector3.Lerp(start, end, t) + Vector3.up * yArc;
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
        controller.enabled = true;
        canMove = true;
        isVaulting = false;
        audios.PlayOneShot(jumpClip, 1f);
    }

    private void ApplyGravity()
    {
        if (!controller.enabled || controller.isGrounded) return;

        playerVel.y -= gravity * Time.deltaTime;
        controller.Move(playerVel * Time.deltaTime);
    }

    private void SetHeight(float newHeight)
    {
        if (!controller.enabled || isVaulting) return;

        float previousHeight = controller.height;
        Vector3 bottom = transform.position - Vector3.up * (previousHeight * 0.5f - controller.center.y);

        // Apply new height and center while preserving foot position
        controller.height = newHeight;
        controller.center = new Vector3(0f, newHeight / 2f, 0f);

        // Move the controller so feet stay at the same world position
        transform.position = bottom + Vector3.up * (newHeight * 0.5f - controller.center.y);

        // Adjust visuals
        if (visualHolder != null)
        {
            float offsetY = currentState switch
            {
                MovementState.Crouching => crouchOffset,
                MovementState.Crawling => crawlOffset,
                _ => standingOffset,
            };

            visualHolder.localPosition = new Vector3(0f, offsetY, 0f);
        }
    }

    private bool CanStandUp()
    {
        Vector3 start = transform.position + Vector3.up * (controller.radius + 0.05f);
        float checkDistance = standingHeight - controller.height;
        return !Physics.SphereCast(start, controller.radius, Vector3.up, out _, checkDistance);
    }




    private void TransitionToState(MovementState newState, float newHeight, float newSpeed)
    {
        currentState = newState;
        SetHeight(newHeight);
        walkSpeed = newSpeed;
        isCrouching = newState == MovementState.Crouching;
        isCrawling = newState == MovementState.Crawling;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        //GameManager.Instance.playerHPBar.value = currentHealth / maxHealth;
        StartCoroutine(DamageFlash());

       // soundManager.instance.playSound(hurtClip, transform, 1f);
       

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            GameManager.Instance.youLoseScreen.SetActive(true);
            Die();
        }
    }

    private IEnumerator DamageFlash()
    {
        GameManager.Instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        GameManager.Instance.playerDamageScreen.SetActive(false);
    }

    public void Die()
    {
        
        Debug.Log("Player has died.");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
        canMove = false;
        controller.enabled = false;
        
    }

    public void SpawnPlayer()
    {
        if (GameManager.Instance != null && GameManager.Instance.playerSpawnPos != null)
        {
            transform.position = GameManager.Instance.playerSpawnPos.position;
            Debug.Log("[SpawnPlayer] Teleported to spawn position: " + transform.position);
        } else
        {
            Debug.LogWarning("[SpawnPlayer] Spawn point is missing!");
        }

        currentHealth = maxHealth;
        canMove = true;
        controller.enabled = true;

        if (GameManager.Instance != null && GameManager.Instance.playerHPBar != null)
            GameManager.Instance.playerHPBar.value = 1f;
    }

}
