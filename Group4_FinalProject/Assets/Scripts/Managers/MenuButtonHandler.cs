using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuButtonHandler : MonoBehaviour
{
    // Menu : Buttons
    public GameObject menuFirstButton;
    public GameObject creditsFirstButton;
    public GameObject creditsClosedButton;

    // Menu : Credits
    public GameObject mainMenu;
    public GameObject creditsMenu;


    // Start is called before the first frame update
    void Start()
    {
        // Setup first selected button
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(menuFirstButton);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Buttons
    public void startGame()
    {
        SceneManager.LoadScene("SpringMap");

        Time.timeScale = 1;
    }

    public void viewCredits()
    {
        // Hide main menu
        mainMenu.SetActive(false);

        // Show credits menu
        creditsMenu.SetActive(true);

        // Setup credits first button
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(creditsFirstButton);
    }

    public void closeCredits()
    {
        // Hide credits menu
        creditsMenu.SetActive(false);

        // Show main menu
        mainMenu.SetActive(true);

        // Setup credits closed button
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(creditsClosedButton);
    }

    public void exitGame()
    {
        Application.Quit();
    }
}
