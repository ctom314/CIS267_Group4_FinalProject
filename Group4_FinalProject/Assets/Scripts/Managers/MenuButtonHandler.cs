using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuButtonHandler : MonoBehaviour
{
    // Menu
    public GameObject mainMenu;
    public GameObject menuFirstButton;

    // Menu : Controls
    public GameObject controlsMenu;
    public GameObject controlsFirstButton;
    public GameObject controlsClosedButton;

    // Menu : Credits
    public GameObject creditsMenu;
    public GameObject creditsFirstButton;
    public GameObject creditsClosedButton;

    private PlayerControls controls;

    // Start is called before the first frame update
    void Start()
    {
        // Setup first selected button
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(menuFirstButton);
    }

    // ================================================================================
    //                                  CONTROLLER INPUT
    // ================================================================================
    private void Awake()
    {
        // Get Player Controls
        controls = new PlayerControls();

        // Close menus
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

    // Update is called once per frame
    void Update()
    {
        
    }

    // Buttons
    public void startGame()
    {
        SceneManager.LoadScene("SpringMap");

        Time.timeScale = 1;

        // Reinitialize TimeManager and MusicManager
        PersistentData.instance.ResetData();
    }

    // ================================================================================
    //                                  CREDITS
    // ================================================================================
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

    // ================================================================================
    //                                  CONTROLS
    // ================================================================================

    public void viewControls()
    {
        // Hide main menu
        mainMenu.SetActive(false);

        // Show controls menu
        controlsMenu.SetActive(true);

        // Setup controls first button
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(controlsFirstButton);
    }

    public void closeControls()
    {
        // Hide controls menu
        controlsMenu.SetActive(false);

        // Show main menu
        mainMenu.SetActive(true);

        // Setup controls closed button
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(controlsClosedButton);
    }

    // ================================================================================

    // Used with Back button
    private void closeMenus()
    {
        if (controlsMenu.activeSelf)
        {
            // Close controls page
            closeControls();
        }
        else if (creditsMenu.activeSelf)
        {
            // Close credits page
            closeCredits();
        }
    }

        public void exitGame()
    {
        Application.Quit();
    }
}
