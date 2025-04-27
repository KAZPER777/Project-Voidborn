using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    public GameManager playerTrig;

    // Update is called once per frame
    void Update()
    {
        playerTrig.WinGame();
    }
}
