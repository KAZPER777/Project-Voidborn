using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class PickUpKey : MonoBehaviour
{
    public GameObject keyOB;          // The key object in the scene
    public GameObject invOB;          // The key's icon in the inventory
    public GameObject pickUpText;     // UI text shown when looking at the key
    public AudioSource keySound;      // Sound that plays when key is picked up

    public float interactDistance = 3f; // How far the player can interact from

    private bool keyPickedUp = false;

    void Start()
    {
        pickUpText.SetActive(false);
        invOB.SetActive(false);
    }

    void Update()
    {
        if (keyPickedUp) return;

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 6f, Color.blue);
        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.collider.gameObject == keyOB)
            {
                pickUpText.SetActive(true);

                if (Input.GetButtonDown("Interact"))
                {
                    keyOB.SetActive(false);
                    keySound.Play();
                    invOB.SetActive(true);
                    pickUpText.SetActive(false);
                    keyPickedUp = true;

                    Debug.Log("[PickUpKey] Key picked up!");
                }
            }
            else
            {
                pickUpText.SetActive(false);
            }
        }
        else
        {
            pickUpText.SetActive(false);
        }
    }
}
