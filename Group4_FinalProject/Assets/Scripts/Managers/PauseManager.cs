using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    // UI : Pause Menu
    public GameObject pauseMenu;
    public GameObject DarkBackground;

    public bool isPaused;

    // Controls
    private PlayerControls controls;

    // Start is called before the first frame update
    void Start()
    {
        isPaused = false;
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
        if (isPaused)
        {
            // Unpause the game
            Time.timeScale = 1;
            isPaused = false;
            pauseMenu.SetActive(false);
            DarkBackground.SetActive(false);
        }
        else
        {
            // Pause the game
            Time.timeScale = 0;
            isPaused = true;
            pauseMenu.SetActive(true);
            DarkBackground.SetActive(true);
        }
    }
}
