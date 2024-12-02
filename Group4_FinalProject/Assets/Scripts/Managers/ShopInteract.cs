using UnityEngine;
using UnityEngine.EventSystems;

public class ShopInteract : MonoBehaviour
{
    public GameObject shopMenu; // Assign the Shop UI menu here
    public GameObject darkBackground; // Optional: Background overlay for focus

    // Shop buttons
    public GameObject shopFirstButton;

    public static bool isShopOpen = false;

    private bool isPlayerNearby = false;

    private void Update()
    {
        // Check for interaction when the player is nearby and presses the right mouse button or the "Submit" button
        if (isPlayerNearby && (Input.GetMouseButtonDown(1) || Input.GetButtonDown("Submit")))
        {
            OpenShopMenu();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Detect when the player enters the shop's trigger area
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Detect when the player leaves the shop's trigger area
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }

    public void OpenShopMenu()
    {
        // Activate the shop UI and optional dark background
        shopMenu.SetActive(true);
        darkBackground?.SetActive(true);
        isShopOpen = true;

        // Setup first button
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(shopFirstButton);

        // Disable player movement
        PlayerMovement.canMove = false;
    }

    public void CloseShopMenu()
    {
        // Deactivate the shop UI and optional dark background
        shopMenu.SetActive(false);
        darkBackground?.SetActive(false);
        isShopOpen = false;

        // Enable player movement
        PlayerMovement.canMove = true;
    }
}
