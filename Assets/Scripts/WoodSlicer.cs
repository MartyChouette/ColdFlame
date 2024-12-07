using UnityEngine;
using EzySlice;

public class WoodSlicer : MonoBehaviour
{
    public Material crossSectionMaterial; // Material for the sliced face
    public float velocityThreshold = 5.0f; // Minimum velocity required to trigger slicing
    public float downwardAngleThreshold = -0.5f; // Threshold for downward motion (dot product with Vector3.down)
    private bool isSlicing = false;        // Flag to prevent multiple slices per collision

    void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object is tagged as "Wood"
        if (collision.gameObject.CompareTag("Wood"))
        {
            // Calculate the relative velocity of the collision
            float relativeVelocity = collision.relativeVelocity.magnitude;

            // Check if the axe is moving downward
            Vector3 axeDirection = transform.up; // Direction the axe handle is pointing
            float downwardMotion = Vector3.Dot(axeDirection, Vector3.down);

            // Only perform the slice if the velocity exceeds the threshold, the motion is downward, and not already slicing
            if (relativeVelocity >= velocityThreshold && downwardMotion > downwardAngleThreshold && !isSlicing)
            {
                isSlicing = true; // Set slicing flag to true
                PerformSlice(collision.gameObject);
            }
        }
    }

    void PerformSlice(GameObject target)
    {
        // Ensure the target has a valid MeshFilter
        MeshFilter meshFilter = target.GetComponent<MeshFilter>();
        if (meshFilter == null || meshFilter.sharedMesh == null)
        {
            Debug.LogWarning("Target object does not have a valid mesh for slicing.");
            isSlicing = false; // Allow subsequent slices
            return;
        }

        // Get the center of the target for the slicing position
        Vector3 slicePosition = target.GetComponent<Renderer>().bounds.center;

        // Determine slice direction based on the target's bounding box
        Vector3 sliceDirection = GetDominantAxis(target);

        // Debug: Visualize the slicing plane
        Debug.DrawRay(slicePosition, sliceDirection * 5, Color.red, 2.0f);

        // Perform the slicing
        SlicedHull slicedObject = target.Slice(slicePosition, sliceDirection, crossSectionMaterial);

        if (slicedObject != null)
        {
            // Create the sliced halves
            GameObject upperHull = slicedObject.CreateUpperHull(target, crossSectionMaterial);
            GameObject lowerHull = slicedObject.CreateLowerHull(target, crossSectionMaterial);

            // Add physics and re-tag the sliced pieces
            AddPhysicsAndTag(upperHull);
            AddPhysicsAndTag(lowerHull);

            // Check sizes to prevent slicing tiny pieces
            if (IsTooSmall(upperHull) || IsTooSmall(lowerHull))
            {
                Destroy(upperHull);
                Destroy(lowerHull);
            }

            // Destroy the original object
            Destroy(target);
        }
        else
        {
            Debug.LogWarning("Slicing failed: No valid SlicedHull created.");
        }

        // Reset the slicing flag after a short delay
        Invoke(nameof(ResetSlicingFlag), 0.1f);
    }

    void AddPhysicsAndTag(GameObject obj)
    {
        // Add physics components
        var meshCollider = obj.AddComponent<MeshCollider>();
        meshCollider.convex = true; // Ensure the collider is convex
        obj.AddComponent<Rigidbody>(); // Add Rigidbody for physics interaction

        // Re-tag the object as "Wood"
        obj.tag = "Wood";

        // Re-layer the object as "Wood" layer 8
        obj.layer = 8;
    }

    Vector3 GetDominantAxis(GameObject obj)
    {
        // Get the size of the object's bounding box
        Bounds bounds = obj.GetComponent<Renderer>().bounds;

        // Determine the dominant axis based on the largest bounding box dimension
        if (bounds.size.x > bounds.size.y && bounds.size.x > bounds.size.z)
        {
            return Vector3.right; // Slice along the X-axis
        }
        else if (bounds.size.z > bounds.size.y)
        {
            return Vector3.forward; // Slice along the Z-axis
        }
        else
        {
            return Vector3.up; // Slice along the Y-axis
        }
    }

    bool IsTooSmall(GameObject obj)
    {
        // Check if the object's size is below a certain threshold
        return obj.GetComponent<Renderer>().bounds.size.magnitude < 0.1f;
    }

    void ResetSlicingFlag()
    {
        isSlicing = false;
    }
}
