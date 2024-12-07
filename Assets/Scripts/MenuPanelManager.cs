using UnityEngine;

public class MenuPanelManager : MonoBehaviour
{
    public GameObject mainMenuPanel;   // Reference to Main Menu panel
    public GameObject settingsPanel;  // Reference to Settings panel
    public GameObject controlsPanel;  // Reference to Controls panel

    // Show Main Menu
    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        controlsPanel.SetActive(false);
    }

    // Show Settings Menu
    public void ShowSettingsMenu()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
        controlsPanel.SetActive(false);
    }

    // Show Controls Menu
    public void ShowControlsMenu()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        controlsPanel.SetActive(true);
    }

    // Quit Game
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
