using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField, Range(3f, 7f)] private float walkSpeed = 5f;
    [SerializeField, Range(1.1f, 3f)] private float sprintMult = 1.5f;
    [SerializeField, Range(8f, 14f)] private float sprintMax = 10f;
    [SerializeField, Range(0.1f, 5f)] private float crouchSpeed = 2f;
    [SerializeField, Range(0.1f, 2f)] private float crawlSpeed = 1f;

    private float baseWalkSpeed;

    [Header("Jump Settings")]
    [SerializeField] private int jumpHeight = 5;
    [SerializeField] private float gravity = 9.81f;

    [Header("Character Controller")]
    public CharacterController controller;
    public MeshRenderer meshrender;

    [Header("Capsule Heights")]
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float crouchHeight = 1.2f;
    [SerializeField] private float crawlHeight = 0.6f;

    // Movement States
    private enum MovementState { Standing, Crouching, Crawling }
    private MovementState currentState = MovementState.Standing;

    // Jumping
    private const int jumpsMax = 1;
    private int jumpsAmount;

    // Runtime Movement
    public bool isMoving;
    public bool isRunning;
    public bool isCrouching;
    public bool isCrawling;

    public Vector3 moveDir;
    private Vector3 playerVel;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        baseWalkSpeed = walkSpeed;
        Application.runInBackground = true;
        SetHeight(standingHeight);
    }

    private void Update()
    {
        if (controller.isGrounded)
        {
            jumpsAmount = 0;
            playerVel.y = 0;
        }

        Movement();
        Sprint();
        Jump();
        Crouch();
        Crawl();
    }

    private void Movement()
    {
        moveDir = (Input.GetAxis("Horizontal") * transform.right +
                   Input.GetAxis("Vertical") * transform.forward);

        controller.Move(moveDir * walkSpeed * Time.deltaTime);
        isMoving = moveDir != Vector3.zero;
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
            }
        }

        playerVel.y -= gravity * Time.deltaTime;
        controller.Move(playerVel * Time.deltaTime);
    }

    private void Sprint()
    {
        if (Input.GetButtonDown("Sprint") && walkSpeed < sprintMax && currentState == MovementState.Standing)
        {
            walkSpeed *= sprintMult;
            isRunning = true;
        } else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            walkSpeed = baseWalkSpeed;
            isRunning = false;
        } else
        {
            isRunning = false;
        }
    }

    private void Crouch()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            if (currentState == MovementState.Standing)
            {
                currentState = MovementState.Crouching;
                SetHeight(crouchHeight);
                walkSpeed = crouchSpeed;
                isCrouching = true;
                isCrawling = false;
            } else if (currentState == MovementState.Crouching)
            {
                currentState = MovementState.Standing;
                SetHeight(standingHeight);
                walkSpeed = baseWalkSpeed;
                isCrouching = false;
            } else if (currentState == MovementState.Crawling)
            {
                currentState = MovementState.Crouching;
                SetHeight(crouchHeight);
                walkSpeed = crouchSpeed;
                isCrawling = false;
                isCrouching = true;
            }
        }
    }

    private void Crawl()
    {
        if (Input.GetButtonDown("Crawl"))
        {
            if (currentState == MovementState.Standing || currentState == MovementState.Crouching)
            {
                currentState = MovementState.Crawling;
                SetHeight(crawlHeight);
                walkSpeed = crawlSpeed;
                isCrawling = true;
                isCrouching = false;
            } else if (currentState == MovementState.Crawling)
            {
                currentState = MovementState.Crouching;
                SetHeight(crouchHeight);
                walkSpeed = crouchSpeed;
                isCrawling = false;
                isCrouching = true;
            }
        }
    }

    private void SetHeight(float height)
    {
        controller.height = height;
        controller.center = new Vector3(0f, height / 2f, 0f); // Keeps bottom flush to ground
    }
}
