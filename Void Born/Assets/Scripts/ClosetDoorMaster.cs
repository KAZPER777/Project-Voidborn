using UnityEngine;
using System.Collections;

public class ClosetDoorMaster : MonoBehaviour
{
    [Header("Door")]
    public Transform openPosition;
    public Transform closedPosition;
    public float slideSpeed = 2f;

    [Header("Player")]
    public GameObject player;
    public Transform enterPoint;
    public Transform exitPoint;
    public Transform cameraRig;

    [Header("Entry/Exit")]
    public float moveSpeed = 3f;
    public float rotateSpeed = 5f;

    [Header("Peek Door Animation")]
    public float peekDoorOpenAmount = 0.15f;
    public float peekDoorSpeed = 2f;
    public float peekRotationAngle = 15f;
    public float peekRotationSpeed = 5f;
    
    [Header("Peek")]
    public float peekDistance = 0.5f;
    public float peekSpeed = 4f;
    private bool isPeeking = false;
    private Quaternion originalRotation;
    private Vector3 originalDoorPosition;


    [Header("Interaction Check")]
    public Transform triggerPoint;
    public float triggerDistance = 5f;

    [Header("Enemy AI")]
    [SerializeField] private StalkerAI stalker;
    public bool canWatcherMove = true;

    [Header("Prompt UI")]
    [SerializeField] private string promptText = "Press LMB to enter closet";

    private enum DoorState { Closed, Opening, OpenWaiting, Closing }
    private DoorState doorState = DoorState.Closed;

    private bool isPlayerInside = false;
    private bool isEntering = false;
    private bool isExiting = false;
    private bool isPendingEntry = false;

    private float doorLerpT = 0f;
    private float exitCooldown = 0f;

    private Vector3 cameraDefaultPos;
    private Vector3 cameraTargetPos;

    public static ClosetDoorMaster ActivePromptCloset;

    void Start()
    {
        cameraDefaultPos = cameraRig.localPosition;
        cameraTargetPos = cameraDefaultPos;
        originalRotation = player.transform.rotation;
        originalDoorPosition = transform.position;
    }

    void Update()
    {
        float distance = Vector3.Distance(player.transform.position, triggerPoint.position);
        bool isCloseEnough = distance <= triggerDistance;

        bool isLookingAtCloset = false;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 5f))
        {
            if (hit.transform.CompareTag("Closet") || hit.transform.IsChildOf(transform))
                isLookingAtCloset = true;
        }

        if (exitCooldown > 0f)
            exitCooldown -= Time.deltaTime;

        // Show or hide prompt
        if (isCloseEnough && isLookingAtCloset && !isPlayerInside && doorState == DoorState.Closed)
        {
            if (ActivePromptCloset == null || ActivePromptCloset == this)
            {
                ActivePromptCloset = this;
                ClosetPromptUIHandler.Instance?.ShowPrompt(promptText);
            }
        } else if (ActivePromptCloset == this)
        {
            ClosetPromptUIHandler.Instance?.HidePrompt();
            ActivePromptCloset = null;
        }

        // Input: Try Enter or Exit
        if (Input.GetMouseButtonDown(0) && exitCooldown <= 0f)
        {
            if (isPlayerInside)
            {
                if (!isEntering && !isExiting)
                    StartExit();
            } else if (isCloseEnough && isLookingAtCloset && doorState == DoorState.Closed)
            {
                doorLerpT = 0f;
                doorState = DoorState.Opening;
            }
        }

        // Door Animation
        if (doorState == DoorState.Opening)
        {
            doorLerpT += Time.deltaTime * slideSpeed;
            transform.position = Vector3.Lerp(closedPosition.position, openPosition.position, Mathf.Clamp01(doorLerpT));
            if (doorLerpT >= 1f)
            {
                doorState = DoorState.OpenWaiting;
                isPendingEntry = true;
                StartCoroutine(WaitAndCloseDoor());
            }
        } else if (doorState == DoorState.Closing)
        {
            doorLerpT += Time.deltaTime * slideSpeed * 1.5f;
            transform.position = Vector3.Lerp(openPosition.position, closedPosition.position, Mathf.Clamp01(doorLerpT));
            if (doorLerpT >= 1f)
            {
                doorState = DoorState.Closed;
            }
        }

        // Move Player
        if (isEntering)
        {
            MoveAndRotatePlayer(enterPoint);
            if (HasReached(enterPoint))
            {
                isEntering = false;
                isPlayerInside = true;
                EnablePlayerMovement(true);
                canWatcherMove = false;
                stalker?.CantSeePlayer();
            }
        }

        if (isExiting)
        {
            MoveAndRotatePlayer(exitPoint);
            if (HasReached(exitPoint))
            {
                isExiting = false;
                isPlayerInside = false;
                EnablePlayerMovement(true);
                exitCooldown = 0.5f;
                isPendingEntry = false;
                doorLerpT = 0f;
                doorState = DoorState.Closing;
            }
        }

        // Peeking
        if (isPlayerInside && !isEntering && !isExiting)
        {
            if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E))
            {
                isPeeking = true;

                // Slightly open the door
                Vector3 peekDoorTarget = originalDoorPosition + transform.right * peekDoorOpenAmount;
                transform.position = Vector3.Lerp(transform.position, peekDoorTarget, Time.deltaTime * peekDoorSpeed);

                // Rotate player to look forward
                Quaternion peekRot = Quaternion.LookRotation(transform.forward);
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, peekRot, Time.deltaTime * peekRotationSpeed);

                // Shift camera
                if (Input.GetKey(KeyCode.Q))
                    cameraTargetPos = cameraDefaultPos + Vector3.left * peekDistance;
                else if (Input.GetKey(KeyCode.E))
                    cameraTargetPos = cameraDefaultPos + Vector3.right * peekDistance;

                cameraRig.localPosition = Vector3.Lerp(cameraRig.localPosition, cameraTargetPos, Time.deltaTime * peekSpeed);
            } else if (isPeeking)
            {
                isPeeking = false;

                // Reset door
                transform.position = Vector3.Lerp(transform.position, originalDoorPosition, Time.deltaTime * peekDoorSpeed);

                // Reset player rotation
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, originalRotation, Time.deltaTime * peekRotationSpeed);

                // Reset camera
                cameraRig.localPosition = Vector3.Lerp(cameraRig.localPosition, cameraDefaultPos, Time.deltaTime * peekSpeed);
            }
        }

    }

    IEnumerator WaitAndCloseDoor()
    {
        yield return new WaitForSeconds(0.2f);

        if (!isExiting && isPendingEntry)
        {
            StartEntry();

            while (isEntering)
                yield return null;

            yield return new WaitForSeconds(0.1f);

            if (isPlayerInside)
            {
                doorLerpT = 0f;
                doorState = DoorState.Closing;
            }
        }
    }

    void StartEntry()
    {
        isEntering = true;
        EnablePlayerMovement(false);
    }

    void StartExit()
    {
        isExiting = true;
        isPendingEntry = false;
        EnablePlayerMovement(false);
    }

    void MoveAndRotatePlayer(Transform target)
    {
        player.transform.position = Vector3.MoveTowards(player.transform.position, target.position, moveSpeed * Time.deltaTime);
        Quaternion targetRot = Quaternion.LookRotation(target.forward);
        player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
    }

    bool HasReached(Transform target)
    {
        float dist = Vector3.Distance(player.transform.position, target.position);
        float angle = Quaternion.Angle(player.transform.rotation, Quaternion.LookRotation(target.forward));
        return dist < 0.05f && angle < 2f;
    }

    void EnablePlayerMovement(bool canMove)
    {
        var controller = player.GetComponent<JaidensPlayerController>();
        if (controller != null)
            controller.canMove = canMove;
    }

    public bool GetIsPlayerInside() => isPlayerInside;
}
