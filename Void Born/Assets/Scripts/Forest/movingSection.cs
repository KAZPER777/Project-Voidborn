using UnityEngine;

public class movingSection : MonoBehaviour
{
   

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(-20, 0, 0) * Time.deltaTime;
    }

   
}
