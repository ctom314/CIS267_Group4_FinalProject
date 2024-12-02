using System.Collections;
using UnityEngine;

public class BarnInteract : MonoBehaviour
{
    public GameObject sleepMenu;
    public GameObject darkBackground;
    public TimeManager timeManager;
    public CanvasGroup blackOverlay; // Reference to Black Overlay for fade
    public MusicManager musicManager; // Reference to Music Manager
    public float fadeDuration = 1.0f; // Duration of the fade
    public float blackHoldTime = 2.0f; // Time the screen stays black

    private bool isPlayerNearby = false;

    private void Update()
    {
        if (isPlayerNearby && (Input.GetMouseButtonDown(1) || Input.GetButtonDown("Submit")) && timeManager.isDay)
        {
            OpenSleepMenu();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }

    public void OpenSleepMenu()
    {
        sleepMenu.SetActive(true);
        darkBackground.SetActive(true);
    }

    public void CloseSleepMenu()
    {
        sleepMenu.SetActive(false);
        darkBackground.SetActive(false);
    }

    public void SleepTillNight()
    {
        // Close the menu
        CloseSleepMenu();

        // Start the fade transition
        StartCoroutine(FadeToNight());
    }

    private IEnumerator FadeToNight()
    {
        // Fade out current music
        musicManager.FadeOutMusic(fadeDuration);

        // Fade to black
        yield return StartCoroutine(Fade(0, 1, fadeDuration));

        // Hold black screen
        yield return new WaitForSeconds(blackHoldTime);

        // Skip to night
        if (timeManager != null && timeManager.isDay)
        {
            timeManager.SkipToNight();
        }

        // Fade in night music
        musicManager.FadeInMusic(musicManager.nightMusic, musicManager.nightMusicVolume, fadeDuration);

        // Fade back in
        yield return StartCoroutine(Fade(1, 0, fadeDuration));
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
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