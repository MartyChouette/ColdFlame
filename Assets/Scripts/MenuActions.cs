using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuActions : MonoBehaviour
{
    public void StartGame()
    {
        // Replace "GameScene" with the name of your game scene
        SceneManager.LoadScene("Main");
    }

    public void OpenControls()
    {
        // Show controls menu
        Debug.Log("Open Controls Menu");
    }

    public void OpenSettings()
    {
        // Show settings menu
        Debug.Log("Open Settings Menu");
    }

    public void QuitGame()
    {
        // Quit the application
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
