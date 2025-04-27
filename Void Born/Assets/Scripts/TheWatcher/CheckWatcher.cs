using UnityEngine;

public class CheckWatcher : MonoBehaviour
{
    public bool isVisible;


    private void OnBecameVisible()
    {
        isVisible = true;
    }


    private void OnBecameInvisible()
    {
        isVisible = false;
    }




}
