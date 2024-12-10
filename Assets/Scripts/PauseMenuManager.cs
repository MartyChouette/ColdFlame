using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenuPanel;    // Pause Menu panel
    public GameObject settingsPanel;    // Settings panel
    public GameObject controlsPanel;    // Controls panel

    public MonoBehaviour cameraControlScript; // Reference to the camera control script
    public MonoBehaviour customCursorScript;  // Reference to the custom cursor script

    public SettingsManager settingsManager; // Reference to the SettingsManager

    private bool isPaused = false; // Tracks if the game is paused

    void Start()
    {
        // Ensure all panels are hidden at start
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        controlsPanel.SetActive(false);

        if (settingsManager != null)
        {
            // Update sliders to match saved settings
            settingsManager.volumeSlider.onValueChanged.AddListener(delegate { settingsManager.OnVolumeSliderChanged(); });
            settingsManager.musicSlider.onValueChanged.AddListener(delegate { settingsManager.OnMusicSliderChanged(); });
            settingsManager.sfxSlider.onValueChanged.AddListener(delegate { settingsManager.OnSFXSliderChanged(); });
            settingsManager.dialogueSlider.onValueChanged.AddListener(delegate { settingsManager.OnDialogueSliderChanged(); });
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                // If any sub-panel is active, go back to the pause menu
                if (settingsPanel.activeSelf || controlsPanel.activeSelf)
                {
                    BackToPauseMenu();
                }
                else
                {
                    ResumeGame();
                }
            }
            else
            {
                PauseGame();
            }
        }
    }


    public void PauseGame()
    {
        if (isPaused) return; // Prevent re-pausing if already paused

        isPaused = true;
        Time.timeScale = 0f; // Freeze game time
        pauseMenuPanel.SetActive(true);

        // Disable gameplay-related scripts
        if (cameraControlScript != null)
        {
            cameraControlScript.enabled = false;
        }

        if (customCursorScript != null)
        {
            customCursorScript.enabled = false;
        }

        // Cursor settings
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Ensure the cursor is visible
    }

    public void ResumeGame()
    {
        Debug.Log("Resume Button Pressed"); // Add this to verify the method is called
        if (!isPaused) return; // Prevent resuming if not paused

        Debug.Log("Resuming game...");
        isPaused = false;
        Time.timeScale = 1f; // Resume game time
        Debug.Log($"Time Scale: {Time.timeScale}");

        pauseMenuPanel.SetActive(false);

        // Re-enable gameplay-related scripts
        if (cameraControlScript != null)
        {
            cameraControlScript.enabled = true;
        }

        if (customCursorScript != null)
        {
            customCursorScript.enabled = true;
        }

        // Cursor settings
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; // Hide the cursor for gameplay
    }




    public void ShowSettings()
    {
        pauseMenuPanel.SetActive(false);
        controlsPanel.SetActive(false);
        settingsPanel.SetActive(true);

        // Update cursor settings
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor for menu
        Cursor.visible = true; // Ensure the cursor is visible
    }

    public void ShowControls()
    {
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        controlsPanel.SetActive(true);

        // Update cursor settings
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor for menu
        Cursor.visible = true; // Ensure the cursor is visible
    }

    public void BackToPauseMenu()
    {
        settingsPanel.SetActive(false);
        controlsPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);

        // Update cursor settings
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor for menu
        Cursor.visible = true; // Ensure the cursor is visible
    }


    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Time.timeScale = 1f; // Ensure time resumes if quitting
        Application.Quit();
    }
}
