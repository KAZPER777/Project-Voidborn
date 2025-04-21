using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerController player;

    [Header("Camera Position Offsets")]
    [SerializeField] private Vector3 standOffset = new Vector3(0f, 1.6f, 0f);
    [SerializeField] private Vector3 crouchOffset = new Vector3(0f, 1.2f, 0.05f); // slight forward
    [SerializeField] private Vector3 crawlOffset = new Vector3(0f, 0.75f, 0.1f);   // further forward

    [Header("Target References")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform yawTarget;       // PlayerRoot
    [SerializeField] private Transform pitchTarget;     // CameraPitch
    [SerializeField] private Transform cameraHolder;    // CameraHolder under VisualHolder

    [Header("Camera Heights")]
    [SerializeField] private float standCamY = 1.6f;
    [SerializeField] private float crouchCamY = 1.0f;
    [SerializeField] private float crawlCamY = 0.6f;
    [SerializeField] private float camLerpSpeed = 7f;
    private Vector3 originalCamPos;
    private float targetCamHeight;

    [Header("Leaning")]
    [SerializeField] private float leanAngle = 10f;
    [SerializeField] private float leanOffset = 0.25f;
    [SerializeField] private float leanLerpSpeed = 8f;

    [Header("Headbob (Optional)")]
    [SerializeField] private bool enableHeadbob = false;
    [SerializeField] private float bobAmount = 0.05f;
    [SerializeField] private float bobSpeed = 8f;
    private float bobTimer;
    private Vector3 originalLocalPos;

    [Header("Mouse Look")]
    public float mouseSens = 100f;
    public bool invertY;
    private float rotX;

    [Header("Animator")]
    [SerializeField] private Animator cameraAnimation;
    private const string IS_MOVING = "IsMoving";
    private const string IS_RUNNING = "IsRunning";
    private const string IS_LEANING_LEFT = "IsLeaningLeft";
    private const string IS_LEANING_RIGHT = "IsLeaningRight";
    private const string IS_CROUCHING = "IsCrouching";
    private const string IS_CRAWLING = "IsCrawling";
    private const string IS_VAULTING = "IsVaulting";

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (cameraHolder != null)
            originalCamPos = cameraHolder.localPosition;

        targetCamHeight = standCamY;

        // ✅ Reset pitch Y-position to correct height
        Vector3 pitchStart = pitchTarget.localPosition;
        pitchStart.y = standCamY;
        pitchTarget.localPosition = pitchStart;
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

        // Pitch
        rotX += invertY ? mouseY : -mouseY;
        rotX = Mathf.Clamp(rotX, -45f, 61f);
        pitchTarget.localRotation = Quaternion.Euler(rotX, 0f, 0f);

        // Yaw
        yawTarget.Rotate(Vector3.up * mouseX);
    }

    private void HandleCameraHeight()
    {
        Vector3 targetOffset = standOffset;

        if (player.isCrawling)
            targetOffset = crawlOffset;
        else if (player.isCrouching)
            targetOffset = crouchOffset;

        cameraHolder.localPosition = Vector3.Lerp(
            cameraHolder.localPosition,
            targetOffset,
            Time.deltaTime * camLerpSpeed
        );
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

        Vector3 currentPos = cameraHolder.localPosition;
        Vector3 targetPos = new Vector3(targetX, currentPos.y, currentPos.z);
        cameraHolder.localPosition = Vector3.Lerp(currentPos, targetPos, Time.deltaTime * leanLerpSpeed);
    }

    private void HandleHeadbob()
    {
        if (!enableHeadbob || !player.isMoving || !controller.isGrounded)
            return;

        bobTimer += Time.deltaTime * bobSpeed;
        float bobY = Mathf.Sin(bobTimer) * bobAmount;

        Vector3 newPos = originalLocalPos;
        newPos.y = targetCamHeight + bobY;
        cameraHolder.localPosition = newPos;
    }

    private void UpdateAnimatorBools()
    {
        if (!cameraAnimation) return;

        cameraAnimation.SetBool(IS_MOVING, player.isMoving);
        cameraAnimation.SetBool(IS_RUNNING, player.isRunning);
        cameraAnimation.SetBool(IS_CROUCHING, player.isCrouching);
        cameraAnimation.SetBool(IS_CRAWLING, player.isCrawling);
        cameraAnimation.SetBool(IS_LEANING_LEFT, Input.GetKey(KeyCode.Q));
        cameraAnimation.SetBool(IS_LEANING_RIGHT, Input.GetKey(KeyCode.E));

        if (player.controller.velocity.y > 0.2f && !player.controller.enabled)
        {
            cameraAnimation.SetTrigger(IS_VAULTING);
        }
    }
}
