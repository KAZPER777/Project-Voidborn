using UnityEngine;
using UnityEngine.SceneManagement; // ← Add this at the top!

public class DoorsWithLock : MonoBehaviour
{
    [Header("Door References")]
    public Animator door;
    public GameObject openText;
    public GameObject KeyINV;

    [Header("Audio")]
    public AudioSource doorSound;
    public AudioSource lockedSound;

    [Header("Final Door Settings")]
    public bool isFinalDoor = false;
    public string sceneToLoad; // <- add this to specify the scene to load

    private bool inReach = false;
    private bool unlocked = false;
    private bool hasKey = false;
    private bool hasTriggeredTransition = false; // <- renamed to match new behavior

    void Start()
    {
        inReach = false;
        unlocked = false;
        hasKey = false;

        Debug.Log("[DoorsWithLock] Initialized. Door is locked.");
    }

    void Update()
    {
        if (hasTriggeredTransition) return;

        bool interactPressed = Input.GetButtonDown("Interact");
        hasKey = KeyINV != null && KeyINV.activeInHierarchy;

        if (hasKey && !unlocked)
            Debug.Log("[DoorsWithLock] Player has the key.");

        Debug.Log("[DoorsWithLock] Debug State → inReach: " + inReach + " | hasKey: " + hasKey + " | unlocked: " + unlocked + " | InteractPressed: " + interactPressed);

        if (!unlocked && hasKey && inReach && interactPressed)
        {
            Debug.Log("[DoorsWithLock] Unlocking and opening the door.");
            unlocked = true;
            OpenDoor();
        } else if (!unlocked && inReach && interactPressed)
        {
            Debug.Log("[DoorsWithLock] Door is locked. Playing locked sound.");
            lockedSound?.Play();
        } else if (unlocked && inReach && interactPressed)
        {
            Debug.Log("[DoorsWithLock] Toggling door.");
            ToggleDoor();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("[DoorsWithLock] Trigger Entered by: " + other.name + " | Tag: " + other.tag);

        if (other.CompareTag("Player"))
        {
            inReach = true;
            openText.SetActive(true);
            Debug.Log("[DoorsWithLock] Player entered reach zone. inReach = true.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inReach = false;
            openText.SetActive(false);
            Debug.Log("[DoorsWithLock] Player exited reach zone. inReach = false.");

            if (isFinalDoor && unlocked && !hasTriggeredTransition)
            {
                hasTriggeredTransition = true;
                Debug.Log("[DoorsWithLock] Final door exited. Loading scene...");
                CloseDoor();
                LoadNextScene();
            }
        }
    }

    private void OpenDoor()
    {
        Debug.Log("[DoorsWithLock] Setting Animator: Open = true, Closed = false");
        if (door != null)
        {
            door.SetBool("Open", true);
            door.SetBool("Closed", false);
        }
        doorSound?.Play();
    }

    private void CloseDoor()
    {
        Debug.Log("[DoorsWithLock] Setting Animator: Open = false, Closed = true");
        if (door != null)
        {
            door.SetBool("Open", false);
            door.SetBool("Closed", true);
        }
        doorSound?.Play();
    }

    private void ToggleDoor()
    {
        if (door != null)
        {
            bool isOpen = door.GetBool("Open");
            if (isOpen) CloseDoor();
            else OpenDoor();
        }
    }

    private void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        } else
        {
            Debug.LogError("[DoorsWithLock] No scene name specified to load!");
        }
    }
}
