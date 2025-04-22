using System.Collections;
using UnityEngine;

public class checkpoint : MonoBehaviour
{

    [SerializeField] Renderer model;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && GameManager.Instance.playerSpawnPos.transform.position != transform.position)
        {
            GameManager.Instance.playerSpawnPos.transform.position = transform.position;
            StartCoroutine(checkpointfeedback());
        }
    }

    IEnumerator checkpointfeedback()
    {
        model.material.color = Color.red;
        GameManager.Instance.checkpointPopup.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        model.material.color = Color.white;
        GameManager.Instance.checkpointPopup.SetActive(false);
    }



}
