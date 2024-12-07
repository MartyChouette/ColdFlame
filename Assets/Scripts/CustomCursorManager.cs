using UnityEngine;

public class CustomCursorManager : MonoBehaviour
{
    [Header("Cursor Textures")]
    public Texture2D defaultCursor;
    public Texture2D leftHandCursor;
    public Texture2D rightHandCursor;
    public Texture2D bothHandsCursor;
    public Texture2D interactCursor;

    private bool leftAttached = false;
    private bool rightAttached = false;
    private bool isLookingAtInteractable = false;

    private Texture2D currentCursorTexture;
    private Vector2 cursorPosition;

    [Header("Raycasting")]
    public LayerMask axeLayer; // LayerMask for Axe Layer
    public LayerMask woodLayer; // LayerMask for Wood Layer
    public float interactRange = 5f; // Range of interaction

    void Start()
    {
        Cursor.visible = false;
        currentCursorTexture = defaultCursor;
    }

    void Update()
    {
        UpdateCursorPosition();
        CheckForInteractable();
        UpdateCursorTexture();
    }

    void OnGUI()
    {
        Rect cursorRect = new Rect(
            cursorPosition.x - currentCursorTexture.width / 2,
            Screen.height - cursorPosition.y - currentCursorTexture.height / 2,
            currentCursorTexture.width,
            currentCursorTexture.height
        );

        GUI.DrawTexture(cursorRect, currentCursorTexture);
    }

    void UpdateCursorPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.x = Mathf.Clamp(mousePos.x, 0, Screen.width);
        mousePos.y = Mathf.Clamp(mousePos.y, 0, Screen.height);
        cursorPosition = mousePos;
    }

    void CheckForInteractable()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            // Check if the object is on the Axe Layer or Wood Layer
            if (((1 << hit.collider.gameObject.layer) & axeLayer) != 0 ||
                ((1 << hit.collider.gameObject.layer) & woodLayer) != 0)
            {
                isLookingAtInteractable = true;
                return;
            }
        }

        // If no interactable object is detected
        isLookingAtInteractable = false;
    }

    void UpdateCursorTexture()
    {
        Texture2D newCursorTexture;

        if (isLookingAtInteractable)
        {
            newCursorTexture = interactCursor;
        }
        else if (leftAttached && rightAttached)
        {
            newCursorTexture = bothHandsCursor;
        }
        else if (leftAttached)
        {
            newCursorTexture = leftHandCursor;
        }
        else if (rightAttached)
        {
            newCursorTexture = rightHandCursor;
        }
        else
        {
            newCursorTexture = defaultCursor;
        }

        if (currentCursorTexture != newCursorTexture)
        {
            currentCursorTexture = newCursorTexture;
        }
    }

    public void SetLeftHandAttached(bool attached)
    {
        leftAttached = attached;
    }

    public void SetRightHandAttached(bool attached)
    {
        rightAttached = attached;
    }

    public void SetLookingAtInteractable(bool lookingAt)
    {
        isLookingAtInteractable = lookingAt;
    }
}
