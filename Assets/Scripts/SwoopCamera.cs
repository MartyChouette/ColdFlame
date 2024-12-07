using UnityEngine;
using Cinemachine;

public class SwoopCamera : MonoBehaviour
{
    public CinemachineDollyCart dollyCart; // Reference to the dolly cart
    public CinemachineVirtualCamera dollyCamera; // Virtual camera on the dolly cart
    public CinemachineVirtualCamera firstPersonCamera; // First-person virtual camera
    public float transitionTime = 5f; // Duration of the dolly movement
    private float timer = 0f;

    void Update()
    {
        // Move the dolly cart along the track
        if (timer < transitionTime)
        {
            timer += Time.deltaTime;
            dollyCart.m_Position = Mathf.Lerp(0f, dollyCart.m_Path.PathLength, timer / transitionTime);

            // Automatically switch to first-person camera at the end of the dolly track
            if (timer >= transitionTime)
            {
                SwitchToFirstPerson();
            }
        }
    }

    void SwitchToFirstPerson()
    {
        dollyCamera.Priority = 0;
        firstPersonCamera.Priority = 10;
    }
}
