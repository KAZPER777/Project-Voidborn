using UnityEngine;

public class WeepingAngelAI : MonoBehaviour
{
    //Player Transform
    public Transform playerTransform;

    //WeepingAngel movement
    public float enemySpeed;



    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FollowPlayer();
    }

    void FollowPlayer()
    {
        Vector3 targetPosition = playerTransform.transform.position + Vector3.up * 1f;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, enemySpeed * Time.deltaTime);
    }
}
