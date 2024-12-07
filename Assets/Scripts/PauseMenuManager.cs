using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenuPanel;   // Pause Menu panel
    public GameObject controlsPanel;   // Controls panel
    public GameObject settingsPanel;   // Settings panel

    private bool isPaused = false;     // Tracks if the game is paused

    void Awake()
    {
        pauseMenuPanel.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape key pressed"); // Check if the input is detected
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        Debug.Log("Pausing game..."); // Add debug log
        isPaused = true;
        Time.timeScale = 0f;
        pauseMenuPanel.SetActive(true);
    }


    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Resume the game
        pauseMenuPanel.SetActive(false);
    }

    public void ShowControls()
    {
        pauseMenuPanel.SetActive(false);
        controlsPanel.SetActive(true);
    }

    public void ShowSettings()
    {
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Time.timeScale = 1f; // Ensure time resumes if quitting
        Application.Quit();
    }

    public void BackToPauseMenu()
    {
        controlsPanel.SetActive(false);
        settingsPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }
}
