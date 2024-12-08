using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class WinManager : MonoBehaviour
{
    // UI : Win Screen
    public GameObject winScreen;
    public GameObject DarkBackground;

    public GameObject winSelectedButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Check if player has won (Day 7 passed in 1st Winter)
        if (PersistentData.instance.isPlayerWin() && !winScreen.activeSelf && !PersistentData.instance.isWin)
        {
            // Trigger win
            PersistentData.instance.isWin = true;

            Time.timeScale = 0;

            // Show win screen
            showWinScreen();

            // Setup win first button
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(winSelectedButton);
        }
    }

    // Win Screen
    private void showWinScreen()
    {
        winScreen.SetActive(true);
        DarkBackground.SetActive(true);
    }

    private void hideWinScreen()
    {
        winScreen.SetActive(false);
        DarkBackground.SetActive(false);
    }

    // Buttons
    public void continueGame()
    {
        // Continue in Endless Mode.
        hideWinScreen();

        Time.timeScale = 1;
    }

    public void returnToMainMenu()
    {
        // Reset all game data
        PersistentData.instance.ResetData();

        // Load main menu
        SceneManager.LoadScene("MainMenu");
    }
}
