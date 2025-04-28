using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    public GameManager playerTrig;

    // Update is called once per frame
    void Update()
    {
        
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("level exit"))
        {
            playerTrig.WinGame();
        }
    }
}
