using UnityEngine;
using UnityEngine.UI;

public class ReticleColorChanger : MonoBehaviour
{
    [Header("Reticle Settings")]
    public Image reticle; // The reticle UI element
    public Color defaultColor = Color.white; // Default reticle color
    public Color highlightColor = Color.red; // Highlight color for tagged objects

    [Header("Raycast Settings")]
    public float maxRayDistance = 10f; // Maximum distance to detect objects
    public LayerMask interactionLayerMask; // Layer mask to filter raycast targets

    private Camera playerCamera;

    void Start()
    {
        if (reticle == null)
        {
            Debug.LogError("Reticle is not assigned!");
            return;
        }

        playerCamera = Camera.main;
        reticle.color = defaultColor; // Set the initial color
    }

    void Update()
    {
        // Perform a raycast from the camera's position forward
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxRayDistance, interactionLayerMask))
        {
            // Check if the hit object has the correct tag
            if (hit.collider.CompareTag("Wood") || hit.collider.CompareTag("Axe"))
            {
                reticle.color = highlightColor; // Change the reticle color to the highlight color
                return;
            }
        }

        // Reset the reticle color if no valid object is hit
        reticle.color = defaultColor;
    }
}
