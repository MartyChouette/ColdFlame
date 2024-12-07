using UnityEngine;

public class CameraSway : MonoBehaviour
{
    public float swayAmount = 0.5f; // Maximum amount of sway in each direction
    public float swaySpeed = 1.0f; // Speed of the sway

    private Vector3 originalPosition;

    void Start()
    {
        // Store the original position of the camera
        originalPosition = transform.position;
    }

    void Update()
    {
        // Calculate the sway offset
        float offsetX = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
        float offsetY = Mathf.Cos(Time.time * swaySpeed * 0.5f) * swayAmount;

        // Apply the offset to the camera's position
        transform.position = originalPosition + new Vector3(offsetX, offsetY, 0);
    }
}
