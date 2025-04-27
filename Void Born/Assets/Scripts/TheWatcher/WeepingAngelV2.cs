using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Hierarchy;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

public class WeepingAngelV2 : MonoBehaviour
{
    //references
    public NavMeshAgent agent;
    public Animator animate;
    public Transform playerTransform;
    public CheckWatcher visibilityCheck;
    public AudioSource audioSource;
    public AudioClip[] watcherSounds;
    


    [Header("Damage")]
    private float damageAmount = 33.4f;
    private float damageTime;
    [SerializeField]private float damageDelay;
    private bool canDamage = true;

    private const string IS_ATTACKING = "isAttacking";
    private const string IS_MOVING = "isMoving";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animate = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0f;
        damageTime = damageDelay;
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
                animate.SetBool(IS_MOVING, false); //idle animation
                canDamage = false;
                
            }
            else
            {
                //start moving
                agent.isStopped = false;
                animate.SetBool(IS_MOVING, true); //running animation
                agent.SetDestination(player);
                DealDamage();
            }

           
       }
       
    }


    private void DealDamage()
    {
        if (!canDamage)
        {
            damageTime -= Time.deltaTime;
            if (damageTime <= 0)
            {
                canDamage = true;
                damageTime = 0;
            }

            
            animate.SetBool(IS_ATTACKING, false);

            return; 
        }

        if (playerTransform != null)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);

            if (distance <= 2f)
            {
                if (playerTransform.TryGetComponent<IDamageable>(out var damageable) && canDamage)
                {
                    damageable.TakeDamage(damageAmount);

                    animate.SetBool(IS_ATTACKING, true);

                    canDamage = false;
                    damageTime = damageDelay;
                }
            }
            else
            {
                // Player is out of range, make sure enemy isn't attacking
                animate.SetBool(IS_ATTACKING, false);
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
