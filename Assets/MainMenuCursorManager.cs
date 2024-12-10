using UnityEngine;

public class MainMenuCursorManager : MonoBehaviour
{
    void Start()
    {
        // Unlock and show the cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
