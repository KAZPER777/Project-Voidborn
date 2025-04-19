using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickUp : MonoBehaviour
{
    public GameObject pickUpOB;
    public GameObject player;
    public GameObject pickUpText;
    public GameObject cannotPickUpText;
    public float addHealth = 25f;
    private float currentHealth;

    public AudioSource healthPickUpSound;

    public GameObject screenFX;

    public bool inReach;


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = true;
            pickUpText.SetActive(true);

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = false;
            pickUpText.SetActive(false);
            cannotPickUpText.SetActive(false);
        }
    }

    void Start()
    {
        currentHealth = player.GetComponent<PlayerHealth>().health;
        cannotPickUpText.SetActive(false);
        pickUpText.SetActive(false);

        screenFX.SetActive(false);

        inReach = false;
    }

    void Update()
    {
        if(inReach && Input.GetButtonDown("Interact") && player.GetComponent<PlayerHealth>().health < 100)
        {
            inReach = false;
            healthPickUpSound.Play();
            player.GetComponent<PlayerHealth>().health += addHealth;
            screenFX.SetActive(true);
            pickUpOB.GetComponent<BoxCollider>().enabled = false;
            pickUpOB.GetComponent<MeshRenderer>().enabled = false;
            pickUpText.SetActive(false);
            StartCoroutine(TurnScreenFXOFF());
        }

        else if (inReach && Input.GetButtonDown("Interact") && player.GetComponent<PlayerHealth>().health == 100)
        {
            pickUpText.SetActive(false);
            cannotPickUpText.SetActive(true);
        }

    }

    IEnumerator TurnScreenFXOFF()
    {
        yield return new WaitForSeconds(1.25f);
        screenFX.SetActive(false);
        pickUpOB.SetActive(false);
    }
}
