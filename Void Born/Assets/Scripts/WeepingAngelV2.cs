using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class WeepingAngelV2 : MonoBehaviour
{
    //references
    public NavMeshAgent agent;
    public Animator animate;
    public Transform playerTransform;
    public CheckVisiblity visibilityCheck;
    public AudioSource audioSource;
    public AudioClip[] watcherSounds;

    
    private const string WEEPING_MOVING = "isWeepingMoving";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animate = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 player = playerTransform.transform.position + Vector3.up * -1f;
        float distanceToPlayer = Vector3.Distance(transform.position, player);

       if(agent != null)
       {
            if(IsVisible()) //keeps checking if isVisible is true
            {
                //stop moving
                agent.isStopped = true;
                animate.SetBool(WEEPING_MOVING, false); //idle animation
                
                
            }
            else
            {
                //start moving
                agent.isStopped = false;
                animate.SetBool(WEEPING_MOVING, true); //running animation
                agent.SetDestination(player);

            }
       }
       
    }


    private bool IsVisible()
    {
        if(visibilityCheck.isVisible)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
   

}
