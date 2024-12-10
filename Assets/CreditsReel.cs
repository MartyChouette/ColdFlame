using UnityEngine;
using UnityEngine.UI;

public class CreditsReel : MonoBehaviour
{
    public RectTransform creditsText;      // Reference to the RectTransform of the credits text
    public RectTransform titleText;       // Reference to the RectTransform of the title text
    public float scrollSpeed = 50f;       // Speed of the scrolling text
    public float stopPosition = 1000f;    // Y position where the credits stop scrolling
    public AudioSource backgroundMusic;   // Reference to the AudioSource for background music
    public CanvasGroup blackScreenCanvas; // CanvasGroup for the black screen overlay
    public float fadeDuration = 2f;       // Duration for fade-in/out animations

    private Vector2 creditsStartPosition;
    private Vector2 titleStartPosition;

    private void Start()
    {
        // Store the starting positions of the text and title
        creditsStartPosition = creditsText.anchoredPosition;
        titleStartPosition = titleText.anchoredPosition;

        // Ensure the black screen starts transparent and music is off
        blackScreenCanvas.alpha = 0f;
        backgroundMusic.volume = 0f;

        // Fade in the music
        StartCoroutine(FadeInMusic());
    }

    private void Update()
    {
        // Scroll the credits and title text
        creditsText.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
        titleText.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

        // Fade out and end when the credits reach the stop position
        if (creditsText.anchoredPosition.y >= stopPosition)
        {
            StartCoroutine(FadeOutMusicAndScreen());
        }
    }

    private System.Collections.IEnumerator FadeInMusic()
    {
        float elapsed = 0f;

        // Gradually increase the music volume
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            backgroundMusic.volume = t;

            yield return null;
        }
    }

    private System.Collections.IEnumerator FadeOutMusicAndScreen()
    {
        float elapsed = 0f;

        // Gradually fade to black and decrease the music volume
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            blackScreenCanvas.alpha = t;        // Black screen fades in
            backgroundMusic.volume = 1f - t;   // Music fades out

            yield return null;
        }

        // Transition to another scene or take another action
        UnityEngine.SceneManagement.SceneManager.LoadScene("menu");
    }
}
