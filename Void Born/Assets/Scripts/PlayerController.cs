using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("Movement Settings")]
    [SerializeField, Range(1f, 5f)] private float walkSpeed;
    [SerializeField, Range(0.1f, .9f)] private float crouchSpeed;
    [SerializeField, Range(0.1f, .9f)] private float crawlSpeed;
    [SerializeField] private bool canMove = true;

    [Header("Sprint Timing")]
    [SerializeField] private float sprintRampUpTime = 1.0f;
    [SerializeField] private float buildUpMult = 1.25f;
    [SerializeField] private float fullSprintMult = 1.5f;

    [Header("Gravity Settings")]
    [SerializeField] private float gravity = 9.81f;

    [Header("Vault Settings")]
    [SerializeField] private float vaultRange = 1.5f;
    [SerializeField] private float vaultDuration = 2f;
    [SerializeField] private Transform vaultCheckOrigin;
    [SerializeField] private LayerMask vaultLayer;
    [SerializeField] private AnimationCurve vaultCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("CharacterController")]
    public CharacterController controller;
    [SerializeField] private Transform visualHolder;
    [SerializeField] private Animator visualAnimator;

    [Header("Capsule Heights")]
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float crouchHeight = 1.2f;
    [SerializeField] private float crawlHeight = 0.6f;

    [Header("Model Offsets")]
    [SerializeField] private float standingOffset = 0f;
    [SerializeField] private float crouchOffset = -0.4f;
    [SerializeField] private float crawlOffset = -0.8f;

    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    private enum MovementState { Standing, Crouching, Crawling }
    private MovementState currentState = MovementState.Standing;

    public bool isMoving;
    public bool isRunning;
    public bool isCrouching;
    public bool isCrawling;
    private bool isJogging = false;
    private bool isVaulting = false;

    private float baseWalkSpeed;
    private float sprintHoldTimer = 0f;

    private Vector3 moveDir;
    private Vector3 playerVel;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        baseWalkSpeed = walkSpeed;
        SetHeight(standingHeight);
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (controller.isGrounded && !isVaulting)
            playerVel.y = 0;

        if (!isVaulting)
        {
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

        if (visualAnimator != null)
        {
            float currentSpeed = moveDir.magnitude * walkSpeed;
            visualAnimator.SetFloat("Speed", currentSpeed);
        }
    }

    private void LateUpdate()
    {
        if (vaultCheckOrigin != null)
            vaultCheckOrigin.position = transform.position + Vector3.up * 1.4f;
    }

    private void Movement()
    {
        moveDir = (Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward);
        controller.Move(moveDir * walkSpeed * Time.deltaTime);
        isMoving = moveDir != Vector3.zero;
    }

    private void Sprint()
    {
        bool sprintKeyHeld = Input.GetButton("Sprint");
        float verticalInput = Input.GetAxis("Vertical");
        bool movingForward = verticalInput > 0.1f;
        bool movingBackward = verticalInput < -0.1f;
        bool canSprint = currentState == MovementState.Standing && isMoving;

        if (sprintKeyHeld && canSprint)
        {
            if (movingBackward)
            {
                isJogging = true;
                walkSpeed = baseWalkSpeed * 1.1f;
            } else if (movingForward)
            {
                sprintHoldTimer += Time.deltaTime;
                walkSpeed = sprintHoldTimer >= sprintRampUpTime ? baseWalkSpeed * fullSprintMult : baseWalkSpeed * buildUpMult;
                isRunning = true;
                isJogging = false;
            } else ResetSprintState();
        } else ResetSprintState();
    }

    private void HandleJumpOrStand()
    {
        if (currentState == MovementState.Crouching || currentState == MovementState.Crawling)
        {
            TransitionToState(MovementState.Standing, standingHeight, baseWalkSpeed);
        }
    }

    private void HandleCrouch()
    {
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

    private void ApplyGravity()
    {
        if (!controller.enabled) return;

        if (!controller.isGrounded)
        {
            playerVel.y -= gravity * Time.deltaTime;
            controller.Move(playerVel * Time.deltaTime);
        }
    }

    private void TryVault()
    {
        if (isVaulting) return;

        if (Physics.Raycast(vaultCheckOrigin.position, transform.forward, out RaycastHit hit, vaultRange, vaultLayer))
            StartCoroutine(PerformVault(hit));
    }

    private IEnumerator PerformVault(RaycastHit hit)
    {
        isVaulting = true;
        controller.enabled = false;

        visualAnimator?.SetTrigger("Vault");

        bool originalCanMove = canMove;
        canMove = false;

        Vector3 vaultStart = transform.position;
        Vector3 vaultEnd = hit.point + transform.forward * 0.15f;
        vaultEnd.y = hit.collider.bounds.max.y + 0.05f + (controller.height / 2f);

        float elapsed = 0f;
        float duration = vaultDuration;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float curvedT = vaultCurve.Evaluate(t);
            float arc = Mathf.Sin(curvedT * Mathf.PI) * 0.15f;

            Vector3 nextPos = Vector3.Lerp(vaultStart, vaultEnd, curvedT);
            nextPos.y += arc;

            transform.position = nextPos;
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = vaultEnd;
        controller.enabled = true;
        canMove = originalCanMove;
        isVaulting = false;
    }

    private void SetHeight(float height)
    {
        controller.height = height;
        controller.center = new Vector3(0f, height / 2f, 0f);

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

    private void ResetSprintState()
    {
        sprintHoldTimer = 0f;
        isRunning = false;
        isJogging = false;
        walkSpeed = baseWalkSpeed;
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
        if (currentHealth <= 0f) return;

        currentHealth -= amount;

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player has died");
        // Add death behavior here (e.g. disable movement)
    }
}
