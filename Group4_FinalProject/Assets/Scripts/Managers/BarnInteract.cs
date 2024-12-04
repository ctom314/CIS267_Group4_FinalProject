using System.Collections;
using UnityEngine;

public class BarnInteract : MonoBehaviour
{
    public GameObject sleepMenu; // The sleep menu UI
    public GameObject darkBackground; // Background dim for sleep menu
    public TimeManager timeManager; // Reference to the TimeManager
    public CanvasGroup blackOverlay; // Reference to Black Overlay for fade
    public float fadeDuration = 1.0f; // Duration of the fade
    public float blackHoldTime = 2.0f; // Time the screen stays black

    private MusicManager musicManager; // Reference to Music Manager
    private bool isPlayerNearby = false; // Tracks if the player is near the barn

    private void Start()
    {
        // Find the MusicManager dynamically if it's not assigned
        if (musicManager == null)
        {
            musicManager = FindObjectOfType<MusicManager>();
            if (musicManager == null)
            {
                Debug.LogError("MusicManager not found! Ensure it exists in the scene.");
            }
        }
    }

    private void Update()
    {
        // Check if the player is nearby and presses the interaction key
        if (isPlayerNearby && (Input.GetMouseButtonDown(1) || Input.GetButtonDown("Submit")) && timeManager.isDay)
        {
            OpenSleepMenu();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Detect if the player enters the barn's trigger zone
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Detect if the player exits the barn's trigger zone
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }

    public void OpenSleepMenu()
    {
        // Activate the sleep menu UI
        sleepMenu.SetActive(true);
        darkBackground.SetActive(true);
    }

    public void CloseSleepMenu()
    {
        // Deactivate the sleep menu UI
        sleepMenu.SetActive(false);
        darkBackground.SetActive(false);
    }

    public void SleepTillNight()
    {
        // Close the menu and start the fade transition to night
        CloseSleepMenu();

        // Disable player movement
        PlayerMovement.canMove = false;

        // Start the transition coroutine
        StartCoroutine(FadeToNight());
    }

    private IEnumerator FadeToNight()
    {
        // Fade out current music if MusicManager exists
        if (musicManager != null)
        {
            musicManager.FadeOutMusic(fadeDuration);
        }

        // Fade to black
        yield return StartCoroutine(Fade(0, 1, fadeDuration));

        // Hold the black screen for a moment
        yield return new WaitForSeconds(blackHoldTime);

        // Skip to night via TimeManager
        if (timeManager != null && timeManager.isDay)
        {
            timeManager.SkipToNight();
        }

        // Fade in night music if MusicManager exists
        if (musicManager != null)
        {
            musicManager.FadeInMusic(musicManager.nightMusic, musicManager.nightMusicVolume, fadeDuration);
        }

        // Fade back in from black
        yield return StartCoroutine(Fade(1, 0, fadeDuration));

        // Re-enable player movement
        PlayerMovement.canMove = true;
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        // Smoothly transition the alpha of the black overlay
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            blackOverlay.alpha = alpha; // Update the overlay's alpha
            yield return null;
        }

        blackOverlay.alpha = endAlpha; // Ensure it ends at the exact value
    }
}