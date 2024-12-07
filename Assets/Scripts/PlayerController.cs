using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;               // Speed of movement
    public float crouchSpeed = 2.5f;           // Speed while crouching
    public float lookSpeed = 2f;               // Speed of camera look
    public Transform playerCamera;             // Reference to the player camera

    [Header("Physics Settings")]
    public float gravity = -9.81f;             // Gravity force
    public float jumpHeight = 2f;              // Height of the jump
    public float groundCheckDistance = 0.2f;   // Distance to check for ground
    public LayerMask groundLayer;              // Layer mask for ground objects

    [Header("Crouch Settings")]
    public float crouchHeight = 1f;            // Height of the CharacterController when crouching
    public float standingHeight = 2f;          // Height of the CharacterController when standing
    public float crouchTransitionSpeed = 5f;   // Speed of height transition during crouch

    private CharacterController characterController; // Reference to the CharacterController
    private Vector3 velocity;                  // Current velocity
    private bool isGrounded;                   // Is the player on the ground?
    private float xRotation = 0f;              // Camera rotation around the x-axis
    private bool isCrouching = false;          // Is the player crouching?

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        if (!characterController)
        {
            Debug.LogError("CharacterController component is missing from this GameObject.");
        }

        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMovement();
        HandleCameraLook();
        HandleCrouch();
    }

    void HandleMovement()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundLayer);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Ensure the player stays grounded
        }

        // Calculate movement direction
        float speed = isCrouching ? crouchSpeed : moveSpeed;
        Vector3 move = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
        characterController.Move(move * speed * Time.deltaTime);

        // Jump
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Apply vertical velocity to the character controller
        characterController.Move(velocity * Time.deltaTime);
    }

    void HandleCameraLook()
    {
        // Mouse input
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        // Rotate camera vertically
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotate player horizontally
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = !isCrouching;
        }

        // Smoothly adjust the CharacterController height
        float targetHeight = isCrouching ? crouchHeight : standingHeight;
        characterController.height = Mathf.Lerp(characterController.height, targetHeight, crouchTransitionSpeed * Time.deltaTime);

        // Adjust camera position relative to the crouch state
        Vector3 cameraPosition = playerCamera.localPosition;
        cameraPosition.y = targetHeight - 0.5f; // Adjust camera to align with the CharacterController height
        playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, cameraPosition, crouchTransitionSpeed * Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        // Visualize the ground check
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, groundCheckDistance);
    }
}
