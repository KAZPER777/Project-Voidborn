using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerController player;

    [Header("Target References")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform yawTarget;       // Usually PlayerRoot
    [SerializeField] private Transform pitchTarget;     // Usually CameraHolder
    [SerializeField] private Transform cameraHolder;    // Parent of Main Camera

    [Header("Camera Heights")]
    [SerializeField] private float standCamY = 1.6f;
    [SerializeField] private float crouchCamY = 1.0f;
    [SerializeField] private float crawlCamY = 0.6f;
    [SerializeField] private float camLerpSpeed = 7f;

    [Header("Leaning")]
    [SerializeField] private float leanAngle = 10f;
    [SerializeField] private float leanOffset = 0.25f;
    [SerializeField] private float leanLerpSpeed = 8f;

    [Header("Headbob (Optional)")]
    [SerializeField] private bool enableHeadbob = false;
    [SerializeField] private float bobAmount = 0.05f;
    [SerializeField] private float bobSpeed = 8f;
    private float bobTimer;

    [Header("Mouse Look")]
    public float mouseSens = 100f;
    public bool invertY;
    private float rotX;

    [Header("Animator (Optional)")]
    public Animator cameraAnimation;
    private const string IS_MOVING = "IsMoving";
    private const string IS_RUNNING = "IsRunning";
    private const string IS_LEANING_LEFT = "IsLeaningLeft";
    private const string IS_LEANING_RIGHT = "IsLeaningRight";
    private const string IS_CROUCHING = "IsCrouching";
    private const string IS_CRAWLING = "IsCrawling";

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        CameraMovement();
        HandleCameraHeight();
        HandleLeaning();
        HandleHeadbob();

        UpdateAnimatorBools();
    }

    private void CameraMovement()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;

        rotX += invertY ? mouseY : -mouseY;
        rotX = Mathf.Clamp(rotX, -90f, 90f);

        pitchTarget.localRotation = Quaternion.Euler(rotX, 0f, 0f);              // Up/Down (pitch)
        yawTarget.Rotate(Vector3.up * mouseX);                                   // Left/Right (yaw)
    }

    private void HandleCameraHeight()
    {
        float targetY = standCamY;
        if (player.isCrouching) targetY = crouchCamY;
        if (player.isCrawling) targetY = crawlCamY;

        Vector3 localPos = cameraHolder.localPosition;
        localPos.y = Mathf.Lerp(localPos.y, targetY, Time.deltaTime * camLerpSpeed);
        cameraHolder.localPosition = localPos;
    }

    private void HandleLeaning()
    {
        float targetAngle = 0f;
        float targetX = 0f;

        if (Input.GetKey(KeyCode.Q))
        {
            targetAngle = leanAngle;
            targetX = -leanOffset;
        } else if (Input.GetKey(KeyCode.E))
        {
            targetAngle = -leanAngle;
            targetX = leanOffset;
        }

        Quaternion targetRot = Quaternion.Euler(rotX, 0f, targetAngle);
        pitchTarget.localRotation = Quaternion.Lerp(pitchTarget.localRotation, targetRot, Time.deltaTime * leanLerpSpeed);

        Vector3 targetPos = new Vector3(targetX, cameraHolder.localPosition.y, cameraHolder.localPosition.z);
        cameraHolder.localPosition = Vector3.Lerp(cameraHolder.localPosition, targetPos, Time.deltaTime * leanLerpSpeed);
    }

    private void HandleHeadbob()
    {
        if (!enableHeadbob || !player.isMoving || !controller.isGrounded) return;

        bobTimer += Time.deltaTime * bobSpeed;
        float bobY = Mathf.Sin(bobTimer) * bobAmount;

        Vector3 camPos = cameraHolder.localPosition;
        camPos.y += bobY;
        cameraHolder.localPosition = camPos;
    }

    private void UpdateAnimatorBools()
    {
        if (!cameraAnimation) return;

        cameraAnimation.SetBool(IS_MOVING, player.isMoving);
        cameraAnimation.SetBool(IS_RUNNING, player.isRunning);
        cameraAnimation.SetBool(IS_CROUCHING, player.isCrouching);
        cameraAnimation.SetBool(IS_CRAWLING, player.isCrawling);

        bool leaningLeft = Input.GetKey(KeyCode.Q);
        bool leaningRight = Input.GetKey(KeyCode.E);

        cameraAnimation.SetBool(IS_LEANING_LEFT, leaningLeft);
        cameraAnimation.SetBool(IS_LEANING_RIGHT, leaningRight);
    }
}
