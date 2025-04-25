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

    [Header("Peek")]
    public float peekDistance = 0.5f;
    public float peekSpeed = 4f;

    [Header("Interaction Check")]
    public Transform triggerPoint;
    public float triggerDistance = 2.5f;

    private enum DoorState { Closed, Opening, OpenWaiting, Closing }
    private DoorState doorState = DoorState.Closed;
    private float doorLerpT = 0f;

    private bool isPlayerInside = false;
    private bool isEntering = false;
    private bool isExiting = false;
    private bool entryCoroutineRunning = false;
    private bool isTransitioning = false;

    private Vector3 cameraDefaultPos;
    private Vector3 cameraTargetPos;

    void Start()
    {
        cameraDefaultPos = cameraRig.localPosition;
        cameraTargetPos = cameraDefaultPos;
        transform.localPosition = closedPosition.localPosition; // start closed
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(player.transform.position, triggerPoint.position);
        bool isCloseEnough = distanceToPlayer <= triggerDistance;

        bool isLookingAtCloset = false;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 3f))
        {
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                isLookingAtCloset = true;
            }
        }

        // Mouse 1 to Enter or Exit
        if (!isEntering && !isExiting && !isTransitioning && Input.GetMouseButtonDown(0))

        {
            if (isPlayerInside)
            {
                if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E)) return;
                StartExit();
            } else if (isCloseEnough && isLookingAtCloset && doorState == DoorState.Closed)
            {
                doorLerpT = 0f;
                doorState = DoorState.Opening;
            }
        }

        // === Door Movement ===
        if (doorState == DoorState.Opening)
        {
            doorLerpT += Time.deltaTime * slideSpeed;
            float t = Mathf.Clamp01(doorLerpT);
            transform.localPosition = Vector3.Lerp(closedPosition.localPosition, openPosition.localPosition, t);

            if (t >= 1f)
            {
                doorLerpT = 0f;
                doorState = DoorState.OpenWaiting;
                StartCoroutine(WaitAndCloseDoorAfterEntry());
            }
        } else if (doorState == DoorState.Closing)
        {
            doorLerpT += Time.deltaTime * slideSpeed * 1.5f;
            float t = Mathf.Clamp01(doorLerpT);
            transform.localPosition = Vector3.Lerp(openPosition.localPosition, closedPosition.localPosition, t);

            if (t >= 1f)
            {
                doorLerpT = 0f;
                doorState = DoorState.Closed;
            }
        }

        // === Player Movement ===
        if (isEntering)
        {
            MoveAndRotatePlayer(enterPoint);
            if (HasReached(enterPoint))
            {
                isEntering = false;
                isPlayerInside = true;
                EnablePlayerMovement(true);
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
            }
        }

        // === Peeking ===
        if (isPlayerInside && !isEntering && !isExiting)
        {
            if (Input.GetKey(KeyCode.Q))
                cameraTargetPos = cameraDefaultPos + Vector3.left * peekDistance;
            else if (Input.GetKey(KeyCode.E))
                cameraTargetPos = cameraDefaultPos + Vector3.right * peekDistance;
            else
                cameraTargetPos = cameraDefaultPos;

            cameraRig.localPosition = Vector3.Lerp(cameraRig.localPosition, cameraTargetPos, Time.deltaTime * peekSpeed);
        }
    }

    IEnumerator WaitAndCloseDoorAfterEntry()
    {
        entryCoroutineRunning = true;

        yield return new WaitForSeconds(0.2f);

        if (!isExiting)
            StartEntry();

        while (isEntering)
            yield return null;

        yield return new WaitForSeconds(0.1f);

        if (!isExiting)
        {
            doorLerpT = 0f;
            doorState = DoorState.Closing;
        }

        entryCoroutineRunning = false;
        entryCoroutineRunning = false;
        isTransitioning = false;
    }

    IEnumerator WaitAndCloseDoorAfterExit()
    {
        while (isExiting)
            yield return null;

        yield return new WaitForSeconds(0.1f);

        doorLerpT = 0f;
        doorState = DoorState.Closing;
        doorLerpT = 0f;
        doorState = DoorState.Closing;

        yield return new WaitUntil(() => doorState == DoorState.Closed);

        isTransitioning = false;

    }


    void StartEntry()
    {
        isTransitioning = true;
        isEntering = true;
        isPlayerInside = true;
        EnablePlayerMovement(false);
    }


    void StartExit()
    {
        if (entryCoroutineRunning)
        {
            StopAllCoroutines();
            entryCoroutineRunning = false;
            isEntering = false;
        }

        isTransitioning = true;
        isExiting = true;
        isPlayerInside = false;
        EnablePlayerMovement(false);

        doorLerpT = 0f;
        doorState = DoorState.Opening;

        StartCoroutine(WaitAndCloseDoorAfterExit());
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

    void EnablePlayerMovement(bool state)
    {
        var controller = player.GetComponent<JaidensController>();
        if (controller != null)
            controller.canMove = state;
    }

    public bool GetIsPlayerInside() => isPlayerInside;
}
