using UnityEngine;
using TMPro;

public class PagePickup : MonoBehaviour
{
    [Header("Prompt UI")]
    [SerializeField] private TextMeshProUGUI promptText;

    private bool playerInRange = false;

    private void Start()
    {
        if (promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            GameManager.Instance.CollectPage();

            if (promptText != null)
            {
                promptText.gameObject.SetActive(false);
            }

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (promptText != null)
            {
                promptText.text = "Press [E] to collect page";
                promptText.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (promptText != null)
            {
                promptText.gameObject.SetActive(false);
            }
        }
    }
}
