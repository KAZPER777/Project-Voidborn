using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public class CheckVisiblity: MonoBehaviour
{
   
    
    public bool isVisible;

    public void OnBecameVisible()
    {
       
            isVisible = true;
        
        
       
     
        
    }

    public void OnBecameInvisible()
    {
        isVisible = false;

        
       
       
        
    }


}
