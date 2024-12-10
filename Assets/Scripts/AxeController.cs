using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AxeController : MonoBehaviour
{
    public Rigidbody axe;                // Rigidbody of the axe
    public Transform leftHand;           // The left hand object
    public Transform rightHand;          // The right hand object
    public Transform leftGrabPoint;      // The left grab point on the axe
    public Transform rightGrabPoint;     // The right grab point on the axe
    public Transform leftHandDefault;    // Default position for the left hand
    public Transform rightHandDefault;   // Default position for the right hand
    public float returnSpeed = 5f;       // Speed at which the hand returns
    public CustomCursorManager customCursor; // Custom cursor reference
    public Canvas alertCanvas;           // Canvas for mode alerts
    public TMP_Text alertText;           // Text component for mode alerts
    public Transform crosshair;          // Crosshair object for crosshair mode
    public Camera playerCamera;          // Reference to the player's camera
    public float crosshairSensitivity = 2.0f; // Sensitivity for crosshair movement
    public float interactionDistance = 3f;    // Maximum distance for interaction
    public LayerMask interactionLayerMask;    // Layer mask for interactable objects

    private FixedJoint leftJoint;        // Joint connecting left hand to axe
    private FixedJoint rightJoint;       // Joint connecting right hand to axe
    private bool leftEngaged = false;    // Is the left hand engaged
    private bool rightEngaged = false;   // Is the right hand engaged
    private bool screenSpaceMouse = true; // Current mode (true = screen space)


    //AUDIO
    private AudioSource audioSource;
    public AudioClip chopSoundClip; // Chop sound 
    bool canPlaySound = true; // Separate flag for controlling the sound effect
    //JUICE
    public ParticleSystem chopParticleEffect; // Particle effect to trigger
    public float timePauseDuration = 0.1f; // Duration to pause time
    public float collisionCooldown = 0.6f; // Cooldown time between impacts
    private bool canTriggerImpact = true; // Flag to prevent multiple triggers
    bool isHoldingAxe = false; // Tracks if the axe is being held

    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // assign audio source to name

        Debug.Log($"Axe assigned: {axe?.name}");
        Debug.Log($"Custom cursor assigned: {customCursor != null}");
    }

    void Update()
    {
        HandleModeSwitch(); // Handle switching between cursor modes
        HandCheck();        // Manage hand and axe interaction
        UpdateCrosshair();  // Update crosshair position in crosshair mode
        DetectAxeInteraction(); // Check if the player is looking at the axe
    }
    void OnCollisionEnter(Collision collision)
    {
        if (isHoldingAxe && canTriggerImpact && collision.gameObject.CompareTag("Wood"))
        {
            // Prevent further impacts during the cooldown
            canTriggerImpact = false;

            // Play chop sound if allowed
            if (canPlaySound)
            {
                PlayChopSound();
                StartCoroutine(SoundCooldown());
            }

            // Trigger particle effect at the point of collision
            Vector3 collisionPoint = collision.contacts[0].point;
            ParticleSystem particles = Instantiate(chopParticleEffect, collisionPoint, Quaternion.identity);
            particles.Play();

            // Destroy the particle system after it's done
            Destroy(particles.gameObject, particles.main.duration + particles.main.startLifetime.constant);

            // Pause time for a split second
            StartCoroutine(PauseTime());

            // Start collision cooldown
            StartCoroutine(CollisionCooldown());
        }
    }

    public void PlayChopSound()
    {
        if (audioSource != null && chopSoundClip != null)
        {
            // Randomize pitch between 0.95 and 1.1
            audioSource.pitch = Random.Range(0.90f, 1.1f);

            // Play the sound
            audioSource.PlayOneShot(chopSoundClip);
        }
        else
        {
            Debug.LogWarning("AudioSource or ChopSoundClip is missing.");
        }
    }

    void DetectAxeInteraction()
    {
        // Perform a SphereCast to check if the player is near the axe
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        float sphereRadius = 1.5f; // Radius of the sphere for the SphereCast

        if (Physics.SphereCast(ray, sphereRadius, out hit, interactionDistance, interactionLayerMask))
        {
            // Check if the axe is within the interaction range
            if (hit.collider.gameObject == axe.gameObject)
            {
                customCursor.SetLookingAtInteractable(true); // Show interaction reticle
                return;
            }
        }

        // Reset reticle if not looking at the axe
        customCursor.SetLookingAtInteractable(false);
    }



    void HandleModeSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            screenSpaceMouse = !screenSpaceMouse;

            if (alertCanvas != null && alertText != null)
            {
                alertCanvas.enabled = true;
                alertText.text = screenSpaceMouse ? "Screen Space Mouse Active" : "Crosshair Mouse Active";
                Invoke(nameof(HideAlert), 2.0f);
            }

            Cursor.visible = screenSpaceMouse;
            Cursor.lockState = screenSpaceMouse ? CursorLockMode.None : CursorLockMode.Locked;

            if (crosshair != null)
            {
                crosshair.gameObject.SetActive(!screenSpaceMouse);
            }
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
            float mouseX = Input.GetAxis("Mouse X") * crosshairSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * crosshairSensitivity;

            Vector3 crosshairPos = crosshair.localPosition;
            crosshairPos.x = Mathf.Clamp(crosshairPos.x + mouseX, -0.5f, 0.5f);
            crosshairPos.y = Mathf.Clamp(crosshairPos.y + mouseY, -0.5f, 0.5f);
            crosshair.localPosition = crosshairPos;

            playerCamera.transform.Rotate(-mouseY, mouseX, 0);
        }
    }

    void HandCheck()
    {
        float leftDistance = Vector3.Distance(leftHand.position, leftGrabPoint.position);
        float rightDistance = Vector3.Distance(rightHand.position, rightGrabPoint.position);

        if (!leftEngaged && leftDistance < 0.5f && Input.GetMouseButtonDown(0))
        {
            AttachHand(leftHand, ref leftJoint, true);
            leftEngaged = true;
        }

        if (!rightEngaged && rightDistance < 0.5f && Input.GetMouseButtonDown(1))
        {
            AttachHand(rightHand, ref rightJoint, false);
            rightEngaged = true;
        }

        UpdateAxeBehavior();

        if (leftEngaged && Input.GetMouseButtonUp(0))
        {
            DetachHand(leftHand, ref leftJoint, leftHandDefault, true);
            leftEngaged = false;
        }

        if (rightEngaged && Input.GetMouseButtonUp(1))
        {
            DetachHand(rightHand, ref rightJoint, rightHandDefault, false);
            rightEngaged = false;
        }

        // Update holding status based on both hands
        isHoldingAxe = (leftEngaged || rightEngaged);
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

        if (customCursor != null)
        {
            if (isLeftHand)
                customCursor.SetLeftHandAttached(true);
            else
                customCursor.SetRightHandAttached(true);
        }
    }

    void DetachHand(Transform hand, ref FixedJoint joint, Transform defaultPosition, bool isLeftHand)
    {
        if (joint != null)
        {
            Destroy(joint);
        }

        StartCoroutine(ReturnHandToDefault(hand, defaultPosition));

        if (customCursor != null)
        {
            if (isLeftHand)
                customCursor.SetLeftHandAttached(false);
            else
                customCursor.SetRightHandAttached(false);
        }
    }

    System.Collections.IEnumerator ReturnHandToDefault(Transform hand, Transform defaultPosition)
    {
        while (Vector3.Distance(hand.position, defaultPosition.position) > 0.1f)
        {
            hand.position = Vector3.Lerp(hand.position, defaultPosition.position, Time.deltaTime * returnSpeed);
            yield return null;
        }
    }

   //  Cooldown for the sound effect
    private System.Collections.IEnumerator SoundCooldown()
    {
        canPlaySound = false;
        yield return new WaitForSeconds(0.05f); // Adjust cooldown duration as needed
        canPlaySound = true;
    }


    private System.Collections.IEnumerator PauseTime()
    {
        Time.timeScale = 0f; // Pause time
        yield return new WaitForSecondsRealtime(timePauseDuration); // Wait for the pause duration
        Time.timeScale = 1f; // Resume time
    }

    private System.Collections.IEnumerator CollisionCooldown()
    {
        if (!canTriggerImpact) yield break; // Prevent overlapping cooldowns

        Debug.Log("Collision cooldown started");
        yield return new WaitForSeconds(collisionCooldown); // Wait for cooldown duration
        canTriggerImpact = true; // Re-enable impact triggering
        Debug.Log("Collision cooldown ended");
    }


    void UpdateAxeBehavior()
    {
        if (leftEngaged && rightEngaged)
        {
            StabilizeAxe();
        }
        else
        {
            axe.constraints = RigidbodyConstraints.None;
        }
    }

    public void ResetImpactTrigger()
    {
        // Manual reset method for canTriggerImpact
        canTriggerImpact = true;
    }

    void StabilizeAxe()
    {
        axe.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    // Call this method when the player picks up or drops the axe
    public void SetHoldingAxe(bool isHolding)
    {
        isHoldingAxe = isHolding;
    }
}
