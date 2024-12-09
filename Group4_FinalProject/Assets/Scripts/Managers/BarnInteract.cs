using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class BarnInteract : MonoBehaviour
{
    public GameObject sleepMenu; // The sleep menu UI
    public GameObject darkBackground; // Background dim for sleep menu
    public TimeManager timeManager; // Reference to the TimeManager
    public CanvasGroup blackOverlay; // Reference to Black Overlay for fade
    public float fadeDuration = 1.0f; // Duration of the fade
    public float blackHoldTime = 2.0f; // Time the screen stays black

    public GameObject barnFirstButton;

    private MusicManager musicManager; // Reference to Music Manager
    private bool isPlayerNearby = false; // Tracks if the player is near the barn

    private PlayerControls controls;

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

    // ================================================================================
    //                                CONTROLLER INPUT
    // ================================================================================
    private void Awake()
    {
        // Setup player controls
        controls = new PlayerControls();

        // Open sleep menu
        controls.Player.Interact.performed += ctx => OpenSleepMenu();

        // Back button to close in-game menus
        controls.Player.Back.performed += ctx => CloseSleepMenu();
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    // ================================================================================

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
        if (isPlayerNearby && timeManager.isDay)
        {
            // Disable jab
            PitchforkController.canJab = false;

            // Activate the sleep menu UI
            sleepMenu.SetActive(true);
            darkBackground.SetActive(true);

            // Disable player movement
            PlayerMovement.canMove = false;

            // Prevent pausing
            PauseManager.canPause = false;

            // Setup first button
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(barnFirstButton);
        }
    }

    public void CloseSleepMenu()
    {
        // Enable jab
        PitchforkController.canJab = true;

        // Deactivate the sleep menu UI
        sleepMenu.SetActive(false);
        darkBackground.SetActive(false);

        // Enable player movement
        PlayerMovement.canMove = true;

        // Allow pausing
        PauseManager.canPause = true;
    }

    public void SleepTillNight()
    {
        // Close the menu and start the fade transition to night
        CloseSleepMenu();

        // Disable ability to pause
        PauseManager.canPause = false;

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

        // Re-enable pausing
        PauseManager.canPause = true;

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