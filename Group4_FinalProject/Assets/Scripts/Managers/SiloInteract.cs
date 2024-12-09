using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SiloInteract : MonoBehaviour
{
    public GameObject siloMenu; // Reference to the Silo Menu UI
    public GameObject siloFirstButton;

    // Silo Money Display
    public TextMeshProUGUI siloMoneyTxt;

    // Normal Money UI
    public GameObject moneyTxt;
    public GameObject moneyIcon;

    public static bool isSiloOpen = false;

    private bool isPlayerNearby = false; // Tracks if the player is near the silo
    private PlayerControls controls;

    // ================================================================================ 
    //                                CONTROLLER INPUT 
    // ================================================================================ 

    private void Awake()
    {
        // Setup player controls
        controls = new PlayerControls();

        // Open shop
        controls.Player.Interact.performed += ctx => OpenSiloMenu();

        // Hook up the back button for closing the menu
        controls.Player.Back.performed += ctx => CloseSiloMenu();
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Detect when the player enters the silo trigger area
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Detect when the player leaves the silo trigger area
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }

    public void OpenSiloMenu()
    {
        if (isPlayerNearby)
        {
            // Disable jab
            PitchforkController.canJab = false;

            // Activate the Silo Menu UI
            siloMenu.SetActive(true);
            isSiloOpen = true;

            // Hide normal money UI
            moneyTxt.SetActive(false);
            moneyIcon.SetActive(false);

            // Update the silo money display
            ShopInteract.updateMoneyTxt(siloMoneyTxt);

            // Optional: Set the first button for UI navigation
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(siloFirstButton);

            // Disable player movement
            PlayerMovement.canMove = false;

            // Disable pausing
            PauseManager.canPause = false;
        }
    }

    public void CloseSiloMenu()
    {
        // Enable jab
        PitchforkController.canJab = true;

        siloMenu.SetActive(false);
        // Deactivate the Silo Menu UI
        isSiloOpen = false;

        // Show normal money UI
        moneyTxt.SetActive(true);
        moneyIcon.SetActive(true);

        // Enable player movement
        PlayerMovement.canMove = true;

        // Enable pausing
        PauseManager.canPause = true;
    }
}
