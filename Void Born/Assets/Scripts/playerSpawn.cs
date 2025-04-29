using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class playerSpawn : MonoBehaviour
{

    public GameObject player;
    public GameObject spawn;

    private void Start()
    {
        if(spawn == null)
        {
            spawn = GameObject.FindGameObjectWithTag("spawnPoint");
        }

        if (spawn != null && player != null)
        {
            player.transform.position = spawn.transform.position;
        }
    }
}
