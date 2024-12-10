using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WoodCounter : MonoBehaviour
{
    public TMP_Text countText;           // TextMeshPro component to display the wood count
    private int woodCount = 0;           // Tracks the number of wood pieces collected
    private HashSet<GameObject> woodObjects = new HashSet<GameObject>(); // Tracks collected wood objects

    public CanvasGroup fadeCanvasGroup;  // Reference to the CanvasGroup for the fade effect
    public float fadeDuration = 12f;     // Duration of the fade-out effect
    public string creditsSceneName = "Credits"; // Name of the credits scene
    public int woodTarget = 25;          // Number of wood pieces needed to trigger the credits

    public AudioSource backgroundAudio;  // Reference to the background audio source
    private bool isFading = false;       // Prevent multiple triggers during fade

    private void Start()
    {
        UpdateCount();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isFading) return; // Prevent collection during fading

        if (other.CompareTag("Wood") && !woodObjects.Contains(other.gameObject))
        {
            woodObjects.Add(other.gameObject);
            woodCount++;
            UpdateCount();

            // Check if the player has collected enough wood
            if (woodCount >= woodTarget)
            {
                StartCoroutine(FadeToCredits());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Wood") && woodObjects.Contains(other.gameObject))
        {
            woodObjects.Remove(other.gameObject);
            woodCount--;
            UpdateCount();
        }
    }

    private void UpdateCount()
    {
        if (countText != null)
        {
            countText.text = $"{woodCount}";
        }
    }

    private IEnumerator FadeToCredits()
    {
        isFading = true;

        float elapsed = 0f;
        float initialVolume = backgroundAudio != null ? backgroundAudio.volume : 1f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            // Fade out the screen
            if (fadeCanvasGroup != null)
            {
                fadeCanvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            }

            // Fade out the audio
            if (backgroundAudio != null)
            {
                backgroundAudio.volume = Mathf.Lerp(initialVolume, 0f, elapsed / fadeDuration);
            }

            yield return null;
        }

        // Ensure the audio is completely muted
        if (backgroundAudio != null)
        {
            backgroundAudio.volume = 0f;
        }

        // Load the credits scene
        SceneManager.LoadScene(creditsSceneName);
    }
}
