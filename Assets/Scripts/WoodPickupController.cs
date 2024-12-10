using UnityEngine;
//using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;

public class WoodPickupController : MonoBehaviour
{
    public float maxPickupDistance = 3f;  // Maximum distance to pick up objects
    public Camera playerCamera;           // Reference to the player's camera
    public float holdDistance = 1.5f;     // Distance to hold the object in front of the camera
    public float positionSmoothSpeed = 10f; // Speed for smoothing position movement
    public float rotationSmoothSpeed = 10f; // Speed for smoothing rotation movement

    private GameObject targetedObject = null; // The object currently being looked at
    private GameObject pickedObject = null;   // The object currently being held
    private Rigidbody pickedRigidbody = null; // Rigidbody of the picked object
    private bool isHolding = false;           // Is the player holding an object?
    private bool axeIsPickedUp; // Flag to track if the axe is picked up

    public AxeController axeScript; // Reference to the AxeController script

    // Automatically configure the interaction layer mask for layer 8 ("Wood")
    private LayerMask interactionLayerMask = 1 << 8;

    void Update()
    {
        UpdateInteractionTarget();
        HandlePickupAndDrop();
        UpdateHeldObjectPositionAndRotation();

        //Failsafe reset
        HandleFailsafeReset();
    }

    void UpdateInteractionTarget()
    {
        // Perform a raycast from the camera's position forward
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxPickupDistance, interactionLayerMask))
        {
            // Ensure the axe doesn't block interaction with wood
            if (hit.collider.CompareTag("Axe"))
            {
                Debug.Log("Ignoring axe in interaction check.");
                axeIsPickedUp = true; // Track that the axe is picked up
                targetedObject = null; // Reset targeted object
                return;
            }

            if (hit.collider.CompareTag("Wood") && !isHolding)
            {
                // Reset the axe's impact trigger using the instance reference
                if (axeScript != null)
                {
                    axeScript.ResetImpactTrigger();
                }
                else
                {
                    Debug.LogWarning("Axe script reference is missing!");
                }

                targetedObject = hit.collider.gameObject;
                Debug.Log($"Targeting object: {targetedObject.name}");
                return;
            }
        }

        // If no valid target is found, reset targetedObject
        targetedObject = null;
    }

    //Backup failsafe for picking up wood for edgecase that it stops triggering or flag fails
    private void PickupWood(GameObject wood)
    {
        if (wood == null)
        {
            Debug.LogWarning("Attempted to pick up a null wood object!");
            return;
        }

        isHolding = true; // Set the holding flag
        Debug.Log($"Picked up wood: {wood.name}");
        wood.SetActive(false); // Example logic for picking up (disable the object)

        // Schedule a reset to ensure the state doesn't remain stuck
        Invoke(nameof(ResetPickupState), 0.1f); // Reset state after a brief delay
    }




    void HandlePickupAndDrop()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Always allow picking up wood or other objects
            TryPickupObject();
        }

        if (Input.GetMouseButtonUp(0) && isHolding)
        {
            // Drop whatever the player is holding
            DropObject();
        }
    }


    void TryPickupObject()
    {
        if (targetedObject != null && pickedObject == null)
        {
            pickedObject = targetedObject;
            pickedRigidbody = pickedObject.GetComponent<Rigidbody>();

            if (pickedRigidbody != null)
            {
                // Reset physics state
                pickedRigidbody.velocity = Vector3.zero;
                pickedRigidbody.angularVelocity = Vector3.zero;
                pickedRigidbody.isKinematic = true; // Disable physics temporarily

                isHolding = true;
                Debug.Log($"Picked up {pickedObject.name}");
            }
            else
            {
                Debug.LogWarning("Targeted object does not have a Rigidbody.");
            }
        }


    }

    void DropObject()
    {
        if (pickedObject != null)
        {
            if (pickedRigidbody != null)
            {
                pickedRigidbody.isKinematic = false; // Re-enable physics
            }

            Debug.Log($"Dropped object: {pickedObject.name}");
            pickedObject = null;
            pickedRigidbody = null;
            isHolding = false;

            // Ensure we can interact with wood again
            targetedObject = null;
        }
    }


    void UpdateHeldObjectPositionAndRotation()
    {
        if (isHolding && pickedObject != null)
        {
            // Calculate the target position in front of the camera
            Vector3 targetPosition = playerCamera.transform.position + playerCamera.transform.forward * holdDistance;

            // Smoothly move the object to the target position
            pickedObject.transform.position = Vector3.Lerp(pickedObject.transform.position, targetPosition, Time.deltaTime * positionSmoothSpeed);

            // Align the object's rotation to face the camera's forward direction
            Quaternion targetRotation = Quaternion.LookRotation(playerCamera.transform.forward, playerCamera.transform.up);

            // Smoothly rotate the object to the target rotation
            pickedObject.transform.rotation = Quaternion.Slerp(pickedObject.transform.rotation, targetRotation, Time.deltaTime * rotationSmoothSpeed);
        }
    }

    private void ResetPickupState()
    {
        isHolding = false; // Allow picking up new objects
        axeIsPickedUp = false; // Reset axe pickup flag
        Debug.Log("Pickup state reset.");
    }

    private void HandleFailsafeReset()
    {
        // Reset wood pickup state when "R" is pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetPickupState();
            Debug.Log("Failsafe reset triggered.");
        }
    }
}
