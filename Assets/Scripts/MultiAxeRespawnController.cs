using UnityEngine;

public class MultiAxeRespawnController : MonoBehaviour
{
    public GameObject[] axes;      // Array of all axes in the scene
    public Transform player;       // Reference to the player's Transform
    public float maxDistance = 30f; // Maximum allowed distance before respawn
    public Vector3 respawnOffset = new Vector3(0, 1, 1); // Offset for respawn position relative to the player

    void Update()
    {
        // Iterate through all axes
        foreach (GameObject axe in axes)
        {
            if (axe == null) continue;

            // Check the distance between the axe and the player
            float distance = Vector3.Distance(axe.transform.position, player.position);
            if (distance > maxDistance)
            {
                RespawnAxe(axe);
            }
        }
    }

    void RespawnAxe(GameObject axe)
    {
        // Set the axe's position near the player
        axe.transform.position = player.position + respawnOffset;

        // Reset the axe's Rigidbody physics
        Rigidbody axeRb = axe.GetComponent<Rigidbody>();
        if (axeRb != null)
        {
            axeRb.velocity = Vector3.zero;
            axeRb.angularVelocity = Vector3.zero;
        }

        Debug.Log($"Axe {axe.name} has been respawned near the player.");
    }
}
