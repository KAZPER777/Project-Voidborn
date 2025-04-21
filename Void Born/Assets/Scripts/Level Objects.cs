using System.Security.Cryptography;
using UnityEngine;


public class LevelObjects : MonoBehaviour
{
    public GameObject onlight;
    public GameObject offlight;

    public GameObject LightText;

    public GameObject LightObject;

    public bool lightsAreOn;
    public bool lightsAreOff;
    public bool inReach;
    void Start()
    {
        inReach = false;
        lightsAreOn = false;
        lightsAreOff = true;
        onlight.SetActive(false);
        offlight.SetActive(true);
        LightObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = true;
            LightText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = false;
            LightText.SetActive(false);
        }
    }

    void Update()
    {
        if (lightsAreOn && inReach && Input.GetButtonDown("Interact"))
        {
            LightObject.SetActive(false);
            onlight.SetActive(false);
            offlight.SetActive(true);
            lightsAreOff = true;
            lightsAreOn = false;
        }

        else if (lightsAreOff && inReach && Input.GetButtonDown("Interact"))
        {
            LightObject.SetActive(true);
            onlight.SetActive(true);
            offlight.SetActive(false);
            lightsAreOff = false;
            lightsAreOn = true;
        }


    }
}
