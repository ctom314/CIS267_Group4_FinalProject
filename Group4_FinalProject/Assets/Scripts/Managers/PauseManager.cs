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

    public static bool isPaused;
    public static bool canPause;

    // Controls
    private PlayerControls controls;

    // Managers
    private MusicManager musicManager;
    private GameButtonHandler gbh;

    // Start is called before the first frame update
    void Start()
    {
        isPaused = false;
        canPause = true;

        // Get Managers
        musicManager = FindObjectOfType<MusicManager>();
        gbh = FindObjectOfType<GameButtonHandler>();
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
        controls.Player.Back.performed += ctx => closeMenus();
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
            if (isPaused && canPause)
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
            else if (!isPaused && canPause)
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

    // Used with Back button
    public void closeMenus()
    {
        if (isPaused && canPause)
        {
            if (!gbh.controlsMenu.activeSelf)
            {
                // Close Pause Menu and unpause the game
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
            else
            {
                // Close controls page
                gbh.hideControlsMenu();
            }
        }
    }
}
