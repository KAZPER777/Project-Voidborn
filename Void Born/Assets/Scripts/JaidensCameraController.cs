using Unity.Cinemachine;
using UnityEngine;

public class JaidensCameraController : MonoBehaviour
{
    public JaidensPlayerController player;
    [SerializeField] CharacterController controller;

    // Animator
    public Animator cameraAnimation;
    private const string IS_MOVING = "IsMoving";
    private const string IS_RUNNING = "IsRunning";

    // Camera Movement
    public float mouseSens;
    public bool cameraBob;
    public bool invertY;
    private float rotX;

    // Camera Offsets
    [Header("Camera Vertical Offsets")]
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private float standingY = 1.0f;
    [SerializeField] private float crouchingY = 0.8f;
    [SerializeField] private float crawlingY = 0.5f;
    [SerializeField] private float camLerpSpeed = 7f;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cameraAnimation = GetComponent<Animator>();
    }

    private void Update()
    {
        CameraMovement();
        HandleCameraHeight();
        PlayerMoving();
        PlayerRunning();

        if (player.isMoving && cameraBob)
        {
            CameraBobbing(); // still empty for now
        }
    }

    private void CameraMovement()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;

        rotX += invertY ? mouseY : -mouseY;
        rotX = Mathf.Clamp(rotX, -90, 90);

        transform.localRotation = Quaternion.Euler(rotX, 0f, 0f);
        transform.parent.parent.Rotate(Vector3.up * mouseX);
    }

    private void HandleCameraHeight()
    {
        if (!cameraHolder) return;

        float targetY = standingY;

        if (player.isCrawling) targetY = crawlingY;
        else if (player.isCrouching) targetY = crouchingY;

        Vector3 localPos = cameraHolder.localPosition;
        localPos.y = Mathf.Lerp(localPos.y, targetY, Time.deltaTime * camLerpSpeed);
        cameraHolder.localPosition = localPos;
    }

    private void PlayerMoving()
    {
        cameraAnimation.SetBool(IS_MOVING, player.isMoving);
    }

    private void PlayerRunning()
    {
        cameraAnimation.SetBool(IS_RUNNING, player.isRunning);
    }

    public void CameraBobbing()
    {
        // Optional: implement bobbing effect here
    }
}
