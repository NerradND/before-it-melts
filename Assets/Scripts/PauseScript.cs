using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject pauseMenuCanvas; 

    [Header("Player Reference")]
    [Tooltip("Drag your Ayam_Player object here to disable movement when paused")]
    public MonoBehaviour playerScript; // This holds your player controller script!

    [Header("Scene Settings")]
    public string mainMenuSceneName = "MainMenu"; 

    private bool isPaused = false;

    void Start()
    {
        ResumeGame();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenuCanvas.SetActive(true);  
        Time.timeScale = 0f;              
        isPaused = true;

        // DISABLE PLAYER CONTROLLER so it stops fighting over the cursor
        if (playerScript != null) playerScript.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        pauseMenuCanvas.SetActive(false); 
        Time.timeScale = 1f;              
        isPaused = false;

        // RE-ENABLE PLAYER CONTROLLER
        if (playerScript != null) playerScript.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(mainMenuSceneName);
    }
}