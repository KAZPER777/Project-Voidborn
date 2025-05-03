using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class buttonFunctions : MonoBehaviour
{
    public void resume()
    {
        GameManager.Instance.ResumeGame();
    }

    public void restart()
    {
        GameManager.Instance.ResumeGame();
        StartCoroutine(RestartScene());
    }

    private IEnumerator RestartScene()
    {
        if(GameManager.Instance.wonGame == true)
        {
            if (SceneManager.GetActiveScene().name == "Forest")
            {

                SceneManager.LoadScene("Mansion");
            }
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            yield return null; // Wait for the next frame

            // Wait until the player script is assigned again
            yield return new WaitUntil(() => GameManager.Instance.playerScript != null);

            GameManager.Instance.playerScript.SpawnPlayer();
        }


    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    public void respawn()
    {
        if (GameManager.Instance.playerScript != null)
        {
            GameManager.Instance.playerScript.SpawnPlayer();
            GameManager.Instance.ResumeGame();
        }
    }

    public void startGame()
    {
        GameManager.Instance.ResumeGame();
        StartCoroutine(LoadGameplayScene());
    }

    private IEnumerator LoadGameplayScene()
    {
        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene("Mansion");
    }
}
