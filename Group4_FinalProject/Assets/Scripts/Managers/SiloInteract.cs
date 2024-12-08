using UnityEngine;
using UnityEngine.EventSystems;

public class SiloInteract : MonoBehaviour
{
    public GameObject siloMenu; // Reference to the Silo Menu UI
    public GameObject siloFirstButton;

    private bool isPlayerNearby = false; // Tracks if the player is near the silo
    private PlayerControls controls;

    private void Awake()
    {
        // Setup player controls
        controls = new PlayerControls();

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

    private void Update()
    {
        // Check for interaction when the player is nearby and presses right mouse or Submit
        if (isPlayerNearby && (Input.GetMouseButtonDown(1) || Input.GetButtonDown("Submit")))
        {
            OpenSiloMenu();
        }
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
        // Activate the Silo Menu UI
        siloMenu.SetActive(true);

        // Optional: Set the first button for UI navigation
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(siloFirstButton);

        // Disable player movement
        PlayerMovement.canMove = false;

        // Disable pausing
        PauseManager.canPause = false;
    }

    public void CloseSiloMenu()
    {
        // Deactivate the Silo Menu UI
        siloMenu.SetActive(false);

        // Enable player movement
        PlayerMovement.canMove = true;

        // Enable pausing
        PauseManager.canPause = true;
    }
}
