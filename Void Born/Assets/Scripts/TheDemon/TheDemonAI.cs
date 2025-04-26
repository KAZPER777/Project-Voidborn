using UnityEngine;

public class TheDemonAI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (TheDemonManager.instance != null)
        {
            TheDemonManager.instance.LookingAtDemon(10);
            TheDemonManager.instance.DrawRay();

            if (TheDemonManager.instance.CheckVisibility())
            {
               
                Debug.Log("Is true");
            }

            TheDemonManager.instance.OnSanityZero();
            TheDemonManager.instance.TeleportDemon();

        }


        

       

        

    }
}
