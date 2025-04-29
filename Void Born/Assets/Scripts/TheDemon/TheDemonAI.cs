using UnityEngine;
using UnityEngine.UIElements;

public class TheDemonAI : MonoBehaviour
{

    private float rotationSpeed = 10;

    private bool isAttackingPlayer = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (TheDemonManager.instance == null)
            return;

        TheDemonManager.instance.LookingAtDemon(10);
        //TheDemonManager.instance.DrawRay(); // Optional
        TheDemonManager.instance.OnSanityZero();
        TheDemonManager.instance.TeleportDemon();

        FacePlayer();

       





    }

    void FacePlayer()
    {
        Transform player = TheDemonManager.instance.playerTransform;

        if (player == null)
            return;

        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f; //Makes it so the demon doesnt tilt

        if (direction.sqrMagnitude > 0f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
