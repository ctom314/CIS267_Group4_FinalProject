using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseManager : MonoBehaviour
{
    // UI : Pause Menu
    public GameObject pauseMenu;
    public GameObject DarkBackground;

    public GameObject pauseSelectedButton;

    public bool isPaused;
    public bool onControlsPage;

    // Controls
    private PlayerControls controls;

    // Reference to Music Manager
    private MusicManager musicManager;

    // Start is called before the first frame update
    void Start()
    {
        isPaused = false;
        onControlsPage = false;

        // Get reference to the MusicManager
        musicManager = FindObjectOfType<MusicManager>();
    }

    // ================================================================================
    //                                CONTROLLER INPUT
    // ================================================================================

    private void Awake()
    {
        // Get Player Controls
        controls = new PlayerControls();

        // Pause the game
        controls.Player.Pause.performed += ctx => togglePause();
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

    public void togglePause()
    {
        if (!PersistentData.instance.isPlayerGameOver() && !ShopInteract.isShopOpen)
        {
            if (isPaused && !onControlsPage)
            {
                // Unpause the game
                Time.timeScale = 1;
                isPaused = false;
                pauseMenu.SetActive(false);
                DarkBackground.SetActive(false);

                // Resume the music
                if (musicManager != null)
                {
                    musicManager.ResumeMusic();
                }
            }
            else if (!isPaused && !onControlsPage)
            {
                // Pause the game
                Time.timeScale = 0;
                isPaused = true;
                pauseMenu.SetActive(true);
                DarkBackground.SetActive(true);

                // Pause the music
                if (musicManager != null)
                {
                    musicManager.PauseMusic();
                }

                // Setup pause first button
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(pauseSelectedButton);
            }
        }
    }
}
