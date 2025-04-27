using JetBrains.Annotations;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public GameManager goal;

    private void OnTriggerEnter(Collider other)
    {
        goal.WinGame();
        
    }
}
