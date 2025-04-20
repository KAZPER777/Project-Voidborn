using System.Collections;
using System.Data;
using UnityEngine;

public class JaidensController : MonoBehaviour, IDamageable
{
    //Player Movement
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintMult;
    [SerializeField] private float sprintMax;

    //Player Jumping (incase we add it anyway)
    [SerializeField] private int jumpHeight;
    [SerializeField] private float gravity;

    [Header("Health Settings")]
     public float maxHealth = 100f;
     public float currentHealth;

    //Animation
    public Animator cameraAnimator; // for animations via camera


    //Jumps
    private const int jumpsMax = 1;
    private int jumpsAmount;

    
    [HideInInspector]
    public bool isMoving; //unhide if you want to see in the inspector
    [HideInInspector]
    public bool isRunning; //unhide if you want to see in the inspector

    //Character Controller Component
    public CharacterController controller;
    public MeshRenderer meshrender;


    //physics based movement
    public Vector3 moveDir;
    Vector3 playerVel;



    private void Start()
    {
        controller = GetComponent<CharacterController>();
        meshrender.enabled = false;
        Application.runInBackground = true;
        currentHealth = maxHealth;
       
    }


    public void Update()
    {
        if (controller.isGrounded)
        {
            jumpsAmount = 0;
            playerVel.y = 0;
        }

        Movement();
        Sprint();
        Jump();


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

    private void Movement()
    {
        moveDir = (Input.GetAxis("Horizontal") * transform.right +
                  (Input.GetAxis("Vertical") * transform.forward));

        moveDir = moveDir.normalized;

        controller.Move(moveDir * walkSpeed * Time.deltaTime);

        isMoving = moveDir != Vector3.zero;



    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumpsAmount < 1)
        {
            jumpsAmount++;
            playerVel.y = jumpHeight;
        }
        controller.Move(playerVel * Time.deltaTime);

        playerVel.y -= gravity * Time.deltaTime;

    }

    private void Sprint()
    {
        if (Input.GetKey(KeyCode.LeftShift) && walkSpeed < sprintMax)
        {
            walkSpeed *= sprintMult;
            isRunning = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            walkSpeed /= sprintMult;
            isRunning = false;
        }
    }


    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        UpdatePlayerUI();
        StartCoroutine(flashDamageScreen());

        if (currentHealth <= 0)
        {
            gamemanager.instance.YouLose();
            Die();
        }
    }

    IEnumerator flashDamageScreen()
    {
        gamemanager.instance.playerdamagescreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gamemanager.instance.playerdamagescreen.SetActive(false);
    }

    public void Die()
    {
        Debug.Log("Player has died");
        // Add death behavior here 
    }

    public void SpawnPlayer()
    {
        controller.transform.position = gamemanager.instance.playerSpawnPos.transform.position;
        maxHealth = currentHealth;
        UpdatePlayerUI();
    }

    public void UpdatePlayerUI()
    {
        gamemanager.instance.playerHPBar.fillAmount = (float)currentHealth / maxHealth;
    }

}
