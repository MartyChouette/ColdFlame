using UnityEngine;

public class ChopTarget : MonoBehaviour
{
    public GameObject brokenPrefab; // Pre-sliced version of the object
    public float chopForceThreshold = 10f;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > chopForceThreshold)
        {
            Chop();
        }
    }

    void Chop()
    {
        Instantiate(brokenPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
