using UnityEngine;

public class FlashlightCheck : MonoBehaviour
{

    public Light playerFlashlight;
    public Transform monsterPosition;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerFlashlight = GetComponent<Light>();
    }


    public bool CheckInFlashLightRange()
    {
        Vector3 toMonster = monsterPosition.transform.position - playerFlashlight.transform.position;
        float distance = Vector3.Distance(playerFlashlight.transform.position, monsterPosition.transform.position);

        if(distance > playerFlashlight.range)
        {
            Debug.Log("Monster Not Within FlashLight Range");
            return false;
        }
       
        // Check angle
        float angle = Vector3.Angle(playerFlashlight.transform.forward, toMonster);
        if (angle > playerFlashlight.spotAngle)
            return false;

        // Optional: Check for obstacles (line of sight)
        if (Physics.Raycast(playerFlashlight.transform.position, toMonster.normalized, out RaycastHit hit, playerFlashlight.range))
        {
            if (hit.transform != monsterPosition)
                return false; // Something is blocking view
        }

        if(playerFlashlight.enabled == false)
        {
            return false;
        }


        return true; // Monster is in the cone and visible


    }


}
