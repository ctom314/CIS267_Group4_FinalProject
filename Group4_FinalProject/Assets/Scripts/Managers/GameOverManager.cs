using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameOverManager : MonoBehaviour
{
    // UI : Game Over Screen
    public GameObject gameOverScreen;
    public GameObject DarkBackground;

    public GameObject gameOverSelectedButton;

    // Update is called once per frame
    void Update()
    {
        // Check if player is dead
        if (PersistentData.instance.isPlayerGameOver())
        {
            // Game over
            showGOScreen();

            Time.timeScale = 0;

            // Setup game over first button
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(gameOverSelectedButton);
        }
    }

    // Game Over Screen
    // TODO: Fix Not being able to navigate to Main Menu button on Game Over screen.
    private void showGOScreen()
    {
        gameOverScreen.SetActive(true);
        DarkBackground.SetActive(true);
    }

    // Buttons
    public void gameOverMainMenu()
    {
        // Reset all game data
        PersistentData.instance.ResetData();

        // Load main menu
        SceneManager.LoadScene("MainMenu");
    }

    public void restartGame()
    {
        // Reset all game data
        PersistentData.instance.ResetData();

        // Load spring
        SceneManager.LoadScene("SpringMap");

        Time.timeScale = 1;
    }
}
