using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerController player;
    [SerializeField] CharacterController controller;

    //Animator
    public Animator cameraAnimation;

    private const string IS_MOVING = "IsMoving";
    private const string IS_RUNNING = "IsRunning";
    private const string IS_LEANING_LEFT = "IsLeaningLeft";
    private const string IS_LEANING_RIGHT = "IsLeaningRight";
    private const string IS_CROUCHING = "IsCrouching";
    private const string IS_CRAWLING = "IsCrawling";




    //Camera Movement
    public float mouseSens;

    public bool cameraBob;
    public bool invertY;
    private float rotX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cameraAnimation = GetComponent<Animator>();
       

    }

    // Update is called once per frame
    private void Update()
    {

        CameraMovement();
        PlayerMoving();
        PlayerRunning();
        HandleLeaning();

        if (cameraBob && player.isMoving)
        {

            CameraBobbing();
        }

    }

    private void PlayerMoving()
    {
        if (player.isMoving)
        {
            cameraAnimation.SetBool(IS_MOVING, player.isMoving);
        }
    }

    private void PlayerRunning()
    {
        if (player.isMoving && player.isRunning)
        {
            cameraAnimation.SetBool(IS_RUNNING, player.isRunning);
        }
    }

    private void PlayerCrouching()
    {
        if (player.isCrouching)
        {
            cameraAnimation.SetBool(IS_CROUCHING, player.isCrouching);
        }
    }

    private void PlayerCrawling()
    {
        if (player.isCrawling)
        {
            cameraAnimation.SetBool(IS_CRAWLING, player.isCrawling);
        }
    }

    //Controlls camera movement (might switch it to public because there may be a mechanic altering this)
    private void CameraMovement()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;


        if (invertY)
            rotX += mouseY;
        else
            rotX -= mouseY;

        rotX = Mathf.Clamp(rotX, -90, 90);
        transform.localRotation = Quaternion.Euler(rotX, 0f, 0f);
        transform.parent.Rotate(Vector3.up * mouseX);
    }

    public void CameraBobbing()
    {
        
        
    }

    public void HandleLeaning()
    {
        if (Input.GetButton("Lean Left"))
        {
            cameraAnimation.SetBool(IS_LEANING_LEFT, true);
            cameraAnimation.SetBool(IS_LEANING_RIGHT, false);
        } else if (Input.GetButton("Lean Right"))
        {
            cameraAnimation.SetBool(IS_LEANING_LEFT, false);
            cameraAnimation.SetBool(IS_LEANING_RIGHT, true);
        } else
        {
            cameraAnimation.SetBool(IS_LEANING_LEFT, false);
            cameraAnimation.SetBool(IS_LEANING_RIGHT, false);
        }
    }
}
