using System.Collections;
using System.Data;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Player Movement
    [SerializeField, Range(3f, 7f)] private float walkSpeed;
    [SerializeField, Range(1.1f, 3f)] private float sprintMult;
    [SerializeField, Range(8f, 14f)] private float sprintMax;
    [SerializeField, Range(0.1f, 5f)] private float crouchSpeed = 2f;
    [SerializeField, Range(0.1f, 2f)] private float crawlSpeed = 1f;

    private float baseWalkSpeed = 5f;

    //Player Jumping (incase we add it anyway)
    [SerializeField] private int jumpHeight;
    [SerializeField] private float gravity;

    //Animation
    public Animator cameraAnimator; // for animations via camera
   

    //Jumps
    private const int jumpsMax = 1;
    private int jumpsAmount;

    public bool isMoving;
    public bool isRunning;
    public bool isCrouching;
    public bool isCrawling;

    //Character Controller Component
    public CharacterController controller;
    public MeshRenderer meshrender;


    //physics based movement
   public Vector3 moveDir;
    Vector3 playerVel;

    //Movement States
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float crouchHeight = 1.2f;
    [SerializeField] private float crawlHeight = 0.6f;
    private enum MovementState { Standing, Crouching, Crawling }
    private MovementState currentState = MovementState.Standing;


    private void Start()
    {
        controller = GetComponent<CharacterController>();
        // meshrender.enabled = false;
        Application.runInBackground = true;

    }

   
    public void Update()
    {
        if(controller.isGrounded)
        {
            jumpsAmount = 0;
            playerVel.y = 0;
        }

        Movement();
        Sprint();
        Jump();
        Crouch();
        Crawl();
        
       
        cameraAnimator.SetFloat("Speed", moveDir.magnitude);
    }

    public bool IsMoving()
    {
        return isMoving;
    }

    public bool IsRunning()
    {
        return isRunning;
    }

    public bool IsCrouching()
    {
        return isCrouching;
    }

    public bool IsCrawling()
    {
        return isCrawling;
    }

    private void Movement()
    {
        moveDir = (Input.GetAxis("Horizontal") * transform.right +
                  (Input.GetAxis("Vertical") * transform.forward));

        controller.Move(moveDir * walkSpeed * Time.deltaTime);

        isMoving = moveDir != Vector3.zero;

    }

    private void Jump()
    {
        if(Input.GetButtonUp("Jump"))
        {
            if (currentState == MovementState.Crawling || currentState == MovementState.Crouching)
            {
                currentState = MovementState.Standing;
                SetHeight(standingHeight);
                walkSpeed = baseWalkSpeed;
                return;
            }

            if (jumpsAmount < jumpsMax && currentState == MovementState.Standing)
            {
                jumpsAmount++;
                playerVel.y = jumpHeight;
            }
        }


        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;
    }

    private void Sprint()
    {
        if (Input.GetButtonDown("Sprint") && walkSpeed < sprintMax && currentState == MovementState.Standing)
        {
            walkSpeed *= sprintMult;
            isRunning = true;
        } 
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            walkSpeed /= sprintMult;
            isRunning = false;
        } 
        else
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
                controller.height = crouchHeight;
                SetHeight(crouchHeight);
                walkSpeed = crouchSpeed;
            } 
            else if (currentState == MovementState.Crouching)
            {
                currentState = MovementState.Standing;
                controller.height = standingHeight;
                SetHeight(standingHeight);
                walkSpeed = baseWalkSpeed;
            } 
            else if (currentState == MovementState.Crawling)
            {
                currentState = MovementState.Crouching;
                controller.height = crouchHeight;
                SetHeight(crouchHeight);
                walkSpeed = crouchSpeed;
            }
        }
    }

    private void Crawl()
    {
        if (Input.GetButtonDown("Crawl"))
        {

            if (currentState == MovementState.Standing)
            {
                currentState = MovementState.Crawling;
                controller.height = crawlHeight;
                SetHeight(crawlHeight);
                walkSpeed = crawlSpeed;
            }
            else if (currentState == MovementState.Crouching)
            {
                currentState = MovementState.Crawling;
                controller.height = crawlHeight;
                SetHeight(crawlHeight);
                walkSpeed = crawlSpeed;
            } 
            else if (currentState == MovementState.Crawling)
            {
                currentState = MovementState.Crouching;
                controller.height = crouchHeight;
                SetHeight(crouchHeight);
                walkSpeed = crouchSpeed;
            }
        }
    }

    // Helper Functions
    private void SetHeight(float height)
    {
        controller.height = height;
        controller.center = new Vector3(0f, standingHeight / 2f - height / 2f, 0f);
    }
            

    

}
