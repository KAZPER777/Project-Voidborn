using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpKey : MonoBehaviour
{
    public GameObject keyOB;         
    public GameObject invOB;         
    public GameObject pickUpText;     
    public AudioSource keySound;      

    public float interactDistance = 2.1f; 

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
