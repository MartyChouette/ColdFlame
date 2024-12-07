using UnityEngine;
using Cinemachine;

public class BackgroundColorBlender : MonoBehaviour
{
    public Camera mainCamera; // Reference to the Main Camera
    public CinemachineVirtualCamera virtualCamera1;
    public CinemachineVirtualCamera virtualCamera2;
    public Color colorForVC1 = Color.blue;
    public Color colorForVC2 = Color.red;
    public float blendSpeed = 2.0f; // Speed of color blending

    void Update()
    {
        if (virtualCamera1.Priority > virtualCamera2.Priority)
        {
            mainCamera.backgroundColor = Color.Lerp(mainCamera.backgroundColor, colorForVC1, Time.deltaTime * blendSpeed);
        }
        else if (virtualCamera2.Priority > virtualCamera1.Priority)
        {
            mainCamera.backgroundColor = Color.Lerp(mainCamera.backgroundColor, colorForVC2, Time.deltaTime * blendSpeed);
        }
    }
}
