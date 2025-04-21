using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickUp1 : MonoBehaviour
{
    public GameObject pickUpOB;
    public GameObject player;
    public GameObject pickUpText;
    public GameObject cannotPickUpText;
    public JaidensController Player;
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
        currentHealth = player.GetComponent<JaidensController>().currentHealth;
        cannotPickUpText.SetActive(false);
        pickUpText.SetActive(false);

        screenFX.SetActive(false);

        inReach = false;
    }

    void Update()
    {
        if(inReach && Input.GetButtonDown("Interact") && player.GetComponent<JaidensController>().currentHealth < 100)
        {
            inReach = false;
            healthPickUpSound.Play();
            player.GetComponent<JaidensController>().currentHealth += addHealth;
            screenFX.SetActive(true);
            pickUpOB.GetComponent<BoxCollider>().enabled = false;
            pickUpOB.GetComponent<MeshRenderer>().enabled = false;
            pickUpText.SetActive(false);
            StartCoroutine(TurnScreenFXOFF());
        }

        else if (inReach && Input.GetButtonDown("Interact") && player.GetComponent<JaidensController>().currentHealth == 100)
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
