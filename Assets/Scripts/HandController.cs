using UnityEngine;

public class HandController : MonoBehaviour
{
    public float handSpeed = 10f;

    void Update()
    {
        if (Input.GetMouseButton(0) && gameObject.name == "LeftHand")
        {
            MoveHand();
        }

        if (Input.GetMouseButton(1) && gameObject.name == "RightHand")
        {
            MoveHand();
        }
    }

    void MoveHand()
    {
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 2f);
        GetComponent<Rigidbody>().velocity = (targetPos - transform.position) * handSpeed;
    }
}
