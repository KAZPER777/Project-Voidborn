using UnityEngine;

public class Destroy : MonoBehaviour
{
    public GameObject terrian;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Destroy"))
        {
            Destroy(terrian);
        }
    }
}
