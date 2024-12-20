using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    // UI : Game Over Screen
    public GameObject gameOverScreen;
    public GameObject DarkBackground;

    public TextMeshProUGUI daysSurvivedTxt;

    public GameObject gameOverSelectedButton;

    // Reference to MusicManager
    private MusicManager musicManager;

    private void Start()
    {
        // Find the MusicManager in the scene
        musicManager = FindObjectOfType<MusicManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if player is dead
        if (PersistentData.instance.isPlayerGameOver() && !gameOverScreen.activeSelf)
        {
            // Pause music
            if (musicManager != null)
            {
                musicManager.PauseMusic();
            }

            Time.timeScale = 0;

            // Game over
            showGOScreen();

            // Setup game over first button
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(gameOverSelectedButton);
        }
    }

    // Game Over Screen
    private void showGOScreen()
    {
        gameOverScreen.SetActive(true);
        DarkBackground.SetActive(true);

        // Display days survived
        daysSurvivedTxt.text = "Days Survived: " + PersistentData.instance.getDay();
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

        // Resume music after restarting
        if (musicManager != null)
        {
            musicManager.ResumeMusic();
        }

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Animator animator = player.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool("isWalking", false);
                animator.SetFloat("InputX", 0);
                animator.SetFloat("InputY", 0);
                animator.SetFloat("LastInputX", 0);
                animator.SetFloat("LastInputY", -1); // Default to facing down, for example
            }
        }
    }
}
