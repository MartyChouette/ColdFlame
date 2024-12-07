using UnityEngine;

public class WoodPickupController : MonoBehaviour
{
    public float maxPickupDistance = 3f;  // Maximum distance to pick up objects
    public LayerMask interactionLayerMask; // Layer mask for interactable objects
    public Camera playerCamera;           // Reference to the player's camera
    public float holdDistance = 1.5f;     // Distance to hold the object in front of the camera
    public float positionSmoothSpeed = 10f; // Speed for smoothing position movement
    public float rotationSmoothSpeed = 10f; // Speed for smoothing rotation movement

    private GameObject targetedObject = null; // The object currently being looked at
    private GameObject pickedObject = null;   // The object currently being held
    private Rigidbody pickedRigidbody = null; // Rigidbody of the picked object
    private bool isHolding = false;           // Is the player holding an object?

    void Update()
    {
        UpdateInteractionTarget();
        HandlePickupAndDrop();
        UpdateHeldObjectPositionAndRotation();
    }

    void UpdateInteractionTarget()
    {
        // Perform a raycast from the camera's position forward
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxPickupDistance, interactionLayerMask))
        {
            if (hit.collider.CompareTag("Wood") && !isHolding) // Ensure not already holding an object
            {
                targetedObject = hit.collider.gameObject;
                return;
            }
        }

        targetedObject = null;
    }

    void HandlePickupAndDrop()
    {
        if (Input.GetMouseButtonDown(0) && !isHolding) // Left mouse button to pick up
        {
            TryPickupObject();
        }

        if (Input.GetMouseButtonUp(0) && isHolding) // Release with left mouse button
        {
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

                // Center the object on its mesh center
                CenterTransformOnMesh(pickedObject);

                // Move to the center of the camera
                ResetTransformToCameraCenter();

                isHolding = true;
                Debug.Log($"Picked up {pickedObject.name}");
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

            pickedObject = null;
            pickedRigidbody = null;
            isHolding = false;

            Debug.Log("Dropped object.");
        }
    }

    void UpdateHeldObjectPositionAndRotation()
    {
        if (isHolding && pickedObject != null)
        {
            // Calculate the target position in front of the camera
            Vector3 targetPosition = playerCamera.transform.position + playerCamera.transform.forward * holdDistance;

            // Restrict the Y position to a maximum height
            float maxYPosition = 2f; // Adjust this value as needed
            targetPosition.y = Mathf.Min(targetPosition.y, maxYPosition);

            // Smoothly move the object to the target position
            pickedObject.transform.position = Vector3.Lerp(pickedObject.transform.position, targetPosition, Time.deltaTime * positionSmoothSpeed);

            // Align the object's rotation to face the camera's forward direction
            Quaternion targetRotation = Quaternion.LookRotation(playerCamera.transform.forward, playerCamera.transform.up);

            // Smoothly rotate the object to the target rotation
            pickedObject.transform.rotation = Quaternion.Slerp(pickedObject.transform.rotation, targetRotation, Time.deltaTime * rotationSmoothSpeed);
        }
    }


    void ResetTransformToCameraCenter()
    {
        if (pickedObject != null)
        {
            // Move the object to the exact center of the camera's view
            pickedObject.transform.position = playerCamera.transform.position + playerCamera.transform.forward * holdDistance;

            // Reset the object's rotation to align with the camera
            pickedObject.transform.rotation = Quaternion.LookRotation(playerCamera.transform.forward, Vector3.up);

            Debug.Log($"Object {pickedObject.name} reset to camera center.");
        }
    }

    void CenterTransformOnMesh(GameObject obj)
    {
        MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            // Calculate the center of the mesh
            Vector3 meshCenter = meshFilter.sharedMesh.bounds.center;

            // Adjust the transform to move the object's origin to the mesh's center
            obj.transform.position += obj.transform.TransformVector(meshCenter);

            // Recalculate local position of children (if any) to preserve visual integrity
            foreach (Transform child in obj.transform)
            {
                child.localPosition -= meshCenter;
            }

            Debug.Log($"Object {obj.name} centered on its mesh.");
        }
    }

}
