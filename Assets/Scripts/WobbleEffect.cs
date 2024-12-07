using UnityEngine;

public class WobbleEffect : MonoBehaviour
{
    public float wobbleIntensity = 5f;

    void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb)
        {
            Vector3 force = collision.contacts[0].normal * -wobbleIntensity;
            rb.AddForce(force, ForceMode.Impulse);
        }
    }
}
