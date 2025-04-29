using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class playerSpawn : MonoBehaviour
{

    public GameObject player;
    public GameObject spawn;

     void Awake()
    {
        if (spawn == null)
        {
            spawn = GameObject.FindGameObjectWithTag("Spawn");
        }

        if (spawn != null && player != null)
        {
            player.transform.position = spawn.transform.position;
        }
    }
}
