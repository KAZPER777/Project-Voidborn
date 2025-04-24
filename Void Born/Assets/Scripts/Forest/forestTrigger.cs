using UnityEngine;

public class forestTrigger : MonoBehaviour
{
    public GameObject section;

    

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Forest Section"))
        {
            Instantiate(section, new Vector3(100,0,0), Quaternion.identity);
        }
    }
}
