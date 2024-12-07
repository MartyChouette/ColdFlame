using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AxeController : MonoBehaviour
{
    public Rigidbody axe;                // Rigidbody of the axe
    
    public AudioSource chopSound;        // AudioSource for the chop sound effect
    public Transform leftHand;          // The left hand object
    public Transform rightHand;         // The right hand object
    public Transform leftGrabPoint;     // The left grab point on the axe
    public Transform rightGrabPoint;    // The right grab point on the axe
    public Transform leftHandDefault;   // Default position for the left hand
    public Transform rightHandDefault;  // Default position for the right hand
    public float returnSpeed = 5f;      // Speed at which the hand returns
    public CustomCursorManager customCursor;   // Custom cursor reference
    public Canvas alertCanvas;          // Canvas for mode alerts
    public TMP_Text alertText;              // Text component for mode alerts
    public Transform crosshair;         // Crosshair object for crosshair mode
    public Camera playerCamera;         // Reference to the player's camera
    public float crosshairSensitivity = 2.0f; // Sensitivity for crosshair movement

    private FixedJoint leftJoint;       // Joint connecting left hand to axe
    private FixedJoint rightJoint;      // Joint connecting right hand to axe
    private bool leftEngaged = false;   // Is the left hand engaged
    private bool rightEngaged = false;  // Is the right hand engaged
    private bool screenSpaceMouse = false; // Current mode (true = screen space)
    private bool isAxePlaced = true;


    private void Awake()
    {
        {
            InitializeCursorMode();
        }
    }
    void Start()
    {
        // Debugging references on Start
        Debug.Log($"Axe assigned: {axe?.name}");
        Debug.Log($"Left hand assigned: {leftHand?.name}");
        Debug.Log($"Right hand assigned: {rightHand?.name}");
        Debug.Log($"Left grab point assigned: {leftGrabPoint?.name}");
        Debug.Log($"Right grab point assigned: {rightGrabPoint?.name}");
        Debug.Log($"Custom cursor assigned: {customCursor != null}");
        
    }

    void Update()
    {
        HandleModeSwitch(); // Handle switching between cursor modes
        HandCheck();        // Manage hand and axe interaction
        UpdateCrosshair();  // Update crosshair position in crosshair mode

        

    }
    void OnCollisionEnter(Collision collision)
    {
        // Check if the axe hit a wood object
        if (collision.gameObject.CompareTag("Wood"))
        {
            // Play the chop sound effect
            if (chopSound != null)
            {
                chopSound.Play();
            }
        }
    }
    void HandleModeSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            screenSpaceMouse = !screenSpaceMouse;

            // Display mode change alert
            if (alertCanvas != null && alertText != null)
            {
                alertCanvas.enabled = true;
                alertText.text = screenSpaceMouse ? "Screen Space Mouse Active" : "Crosshair Mouse Active";
                Invoke(nameof(HideAlert), 2.0f); // Hide the alert after 2 seconds
            }

            // Toggle cursor visibility
            Cursor.visible = screenSpaceMouse;
            Cursor.lockState = screenSpaceMouse ? CursorLockMode.None : CursorLockMode.Locked;

            // Toggle crosshair visibility
            if (crosshair != null)
            {
                crosshair.gameObject.SetActive(!screenSpaceMouse);
            }
        }
    }


    void InitializeCursorMode()
    {
        // Set initial cursor state
        Cursor.visible = screenSpaceMouse;
        Cursor.lockState = screenSpaceMouse ? CursorLockMode.None : CursorLockMode.Locked;

        // Set initial crosshair visibility
        if (crosshair != null)
        {
            crosshair.gameObject.SetActive(!screenSpaceMouse);
        }
    }

    void HideAlert()
    {
        if (alertCanvas != null)
        {
            alertCanvas.enabled = false;
        }
    }

    void UpdateCrosshair()
    {
        if (!screenSpaceMouse && crosshair != null && playerCamera != null)
        {
            // Move the crosshair based on mouse input
            float mouseX = Input.GetAxis("Mouse X") * crosshairSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * crosshairSensitivity;

            Vector3 crosshairPos = crosshair.localPosition;
            crosshairPos.x = Mathf.Clamp(crosshairPos.x + mouseX, -0.5f, 0.5f);
            crosshairPos.y = Mathf.Clamp(crosshairPos.y + mouseY, -0.5f, 0.5f);
            crosshair.localPosition = crosshairPos;

            // Rotate the camera to follow the crosshair
            playerCamera.transform.Rotate(-mouseY, mouseX, 0);
        }
    }

    void HandCheck()
    {
        // Check distance between hands and grab points
        float leftDistance = Vector3.Distance(leftHand.position, leftGrabPoint.position);
        float rightDistance = Vector3.Distance(rightHand.position, rightGrabPoint.position);

        Debug.Log($"Left distance: {leftDistance}, Right distance: {rightDistance}");
        // Engage left hand
        if (!leftEngaged && leftDistance < 0.5f && Input.GetMouseButtonDown(1))
        {
            AttachHand(leftHand, ref leftJoint, true); // Pass true for left hand
            leftEngaged = true;
        }

        // Engage right hand
        if (!rightEngaged && rightDistance < 0.5f && Input.GetMouseButtonDown(0))
        {
            AttachHand(rightHand, ref rightJoint, false); // Pass false for right hand
            rightEngaged = true;
        }

        // Update axe behavior based on hand engagement
        UpdateAxeBehavior();

        // Disengage left hand
        if (leftEngaged && Input.GetMouseButtonUp(1))
        {
            DetachHand(leftHand, ref leftJoint, leftHandDefault, true); // Pass true for left hand
            leftEngaged = false;

        }

        // Disengage right hand
        if (rightEngaged && Input.GetMouseButtonUp(0))
        {
            DetachHand(rightHand, ref rightJoint, rightHandDefault, false); // Pass false for right hand
            rightEngaged = false;
        }
    }

    void AttachHand(Transform hand, ref FixedJoint joint, bool isLeftHand)
    {
        if (hand == null || axe == null)
        {
            Debug.LogError("Hand or axe reference is missing!");
            return;
        }

        joint = hand.gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = axe;

        if (isAxePlaced)
        {
            // Clear constraints when picking up the axe
            axe.constraints = RigidbodyConstraints.None;
            isAxePlaced = false;
        }

        Debug.Log($"{hand.name} engaged!");
    }


    void DetachHand(Transform hand, ref FixedJoint joint, Transform defaultPosition, bool isLeftHand)
    {
        if (joint != null)
        {
            Destroy(joint);
            Debug.Log($"{hand.name} disengaged!");
        }

        // Restore natural physics when no hands are engaged
        if (!leftEngaged && !rightEngaged)
        {
            axe.constraints = RigidbodyConstraints.None;
        }

        StartCoroutine(ReturnHandToDefault(hand, defaultPosition));
    }


    System.Collections.IEnumerator ReturnHandToDefault(Transform hand, Transform defaultPosition)
    {
        while (Vector3.Distance(hand.position, defaultPosition.position) > 0.1f)
        {
            hand.position = Vector3.Lerp(hand.position, defaultPosition.position, Time.deltaTime * returnSpeed);
            yield return null;
        }
        Debug.Log($"{hand.name} returned to default position.");
    }

    void UpdateAxeBehavior()
    {
        if (leftEngaged && rightEngaged)
        {
            StabilizeAxe();
        }
        else
        {
            // Ensure no constraints are applied when using one hand
            axe.constraints = RigidbodyConstraints.None;
        }
    }

    void StabilizeAxe()
    {
        axe.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }


}