using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField, Range(1f, 5f)] private float walkSpeed;
    [SerializeField, Range(1.1f, 3f)] private float sprintMult;
    [SerializeField, Range(1.5f, 10.7f)] private float sprintMax;
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
    private bool isBuildUpSprinting = false;
    private bool isFullSprinting = false;

    private Vector3 moveDir;
    private Vector3 playerVel;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        baseWalkSpeed = walkSpeed;
        SetHeight(standingHeight);
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

        // ✅ Update Speed parameter for blend tree
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

        //Sound
        if (isMoving)
        {
            soundManager.playSound(soundManager.soundType.Footstep, 0);
        }

    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (currentState == MovementState.Crawling || currentState == MovementState.Crouching)
            {
                currentState = MovementState.Standing;
                SetHeight(standingHeight);
                walkSpeed = baseWalkSpeed;
                isCrawling = false;
                isCrouching = false;
                return;
            }

            if (controller.isGrounded && jumpsAmount < jumpsMax && currentState == MovementState.Standing)
            {
                jumpsAmount++;
                playerVel.y = Mathf.Sqrt(2 * gravity * jumpHeight);

                //Sound
                soundManager.playSound(soundManager.soundType.Jump, 1);
            }
        }

        playerVel.y -= gravity * Time.deltaTime;
        controller.Move(playerVel * Time.deltaTime);
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
                ResetSprintSpeed(baseWalkSpeed * 1.1f);
            } else if (movingForward)
            {
                sprintHoldTimer += Time.deltaTime;

                if (sprintHoldTimer >= sprintRampUpTime)
                {
                    ResetSprintSpeed(baseWalkSpeed * fullSprintMult);
                    isFullSprinting = true;
                    isBuildUpSprinting = false;
                } else
                {
                    ResetSprintSpeed(baseWalkSpeed * buildUpMult);
                    isBuildUpSprinting = true;
                    isFullSprinting = false;
                }

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

        //Sound
        if (isRunning)
        {
            soundManager.playSound(soundManager.soundType.Sprint, 1);
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

        // Trigger vault animation
        if (visualAnimator != null)
        {
            visualAnimator.SetTrigger("Vault");
        }

        // Optional: disable input or movement
        bool originalCanMove = canMove;
        canMove = false;

        Vector3 vaultStart = transform.position;

        // Adjust vault target position to be just past the obstacle
        Vector3 vaultEnd = hit.point + transform.forward * 0.15f;
        vaultEnd.y = hit.collider.bounds.max.y + 0.05f + (controller.height / 2f);

        float elapsed = 0f;
        float duration = vaultDuration; // you can set this equal to your animation clip length

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float curvedT = vaultCurve.Evaluate(t);

            // Arc effect: small vertical offset using sine wave
            float arc = Mathf.Sin(curvedT * Mathf.PI) * 0.15f;

            Vector3 nextPos = Vector3.Lerp(vaultStart, vaultEnd, curvedT);
            nextPos.y += arc;

            transform.position = nextPos;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Snap to final position
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
            float offsetY = 0f;

            switch (currentState)
            {
                case MovementState.Standing:
                    offsetY = standingOffset;
                    break;
                case MovementState.Crouching:
                    offsetY = crouchOffset;
                    break;
                case MovementState.Crawling:
                    offsetY = crawlOffset;
                    break;
            }

            visualHolder.localPosition = new Vector3(0f, offsetY, 0f);
        }
    }


    private void ResetSprintSpeed(float speed)
    {
        walkSpeed = speed;
    }

    private void ResetSprintState()
    {
        sprintHoldTimer = 0f;
        isRunning = false;
        isBuildUpSprinting = false;
        isFullSprinting = false;
        isJogging = false;
        walkSpeed = baseWalkSpeed;
    }

    private void TransitionToState(MovementState newState, float newHeight, float newSpeed)
    {
        currentState = newState;
        SetHeight(newHeight);
        walkSpeed = newSpeed;
        isCrouching = (newState == MovementState.Crouching);
        isCrawling = (newState == MovementState.Crawling);
    }
}
