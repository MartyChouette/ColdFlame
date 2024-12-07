using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI buttonText;
    private Color originalColor;

    void Start()
    {
        // Get the TMP Text component
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        originalColor = buttonText.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Set transparency to 255 (fully opaque)
        Color hoverColor = originalColor;
        hoverColor.a = 1f; // Alpha = 255 in 0-1 scale
        buttonText.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Revert to the original transparency
        buttonText.color = originalColor;
    }
}
