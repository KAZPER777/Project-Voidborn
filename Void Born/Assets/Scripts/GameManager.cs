using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;


public class gamemanager : MonoBehaviour
{
    public static gamemanager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] TMP_Text gameGoalCountText;



    public GameObject playerSpawnPos;
    public Image playerHPBar;
    public GameObject playerdamagescreen;

    public GameObject player;
    public JaidensController playerScript;
    public GameObject checkpointPopup;


    public bool isPaused;

    int goalCount;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
       // playerScript = player.GetComponent<JaidensController>();
       // playerSpawnPos = GameObject.FindWithTag("Player Spawn PoS"); // not implemented in the game yet. No checkpoints
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                StatePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause)
            {
                StateUnpause();
            }
        }
    }

    public void StatePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void StateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
        menuActive.SetActive(false);
        menuActive = null;
    }


   /* public void updateGameGoal(int amount)
    {
        goalCount += amount;
        gameGoalCountText.text = goalCount.ToString("F0");      The purpose of this is to update the game goal. This will be used later to update the objective UI

        if (goalCount <= 0)
        {
            statePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }


    }*/

    public void YouLose()
    {
        StatePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

}
