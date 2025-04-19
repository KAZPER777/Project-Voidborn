using UnityEngine;
using UnityEngine.AI;

public class CheckVisiblity: MonoBehaviour
{
   

    public bool isVisible;

    public void OnBecameVisible()
    {
        isVisible = true;
       
     
        Debug.Log("Became visible");
    }

    public void OnBecameInvisible()
    {
        isVisible = false;
       
       
        Debug.Log("Out of view!");
    }


}
