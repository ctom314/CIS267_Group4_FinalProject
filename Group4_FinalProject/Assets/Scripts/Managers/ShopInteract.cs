using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopInteract : MonoBehaviour
{
    public GameObject shopMenu; // Assign the Shop UI menu here

    // Shop buttons
    public GameObject shopFirstButton;

    // Shop Money Display
    public GameObject shopMoneyTxt;

    // Normal Money UI
    public GameObject moneyTxt;
    public GameObject moneyIcon;

    public static bool isShopOpen = false;

    private bool isPlayerNearby = false;

    private PlayerControls controls;

    // ================================================================================ 
    //                                CONTROLLER INPUT 
    // ================================================================================ 
    private void Awake()
    {
        // Setup player controls
        controls = new PlayerControls();

        // Open shop
        controls.Player.Interact.performed += ctx => OpenShopMenu();

        // Back button to close in-game menus
        controls.Player.Back.performed += ctx => CloseShopMenu();
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

    public static void updateMoneyTxt(TextMeshProUGUI moneyTxt)
    {
        moneyTxt.text = PersistentData.instance.GetMoney().ToString();
    }

    public void OpenShopMenu()
    {
        if (isPlayerNearby)
        {
            // Disable jab
            PitchforkController.canJab = false;

            // Activate the shop UI and optional dark background
            shopMenu.SetActive(true);
            isShopOpen = true;

            // Hide normal money UI
            moneyTxt.SetActive(false);
            moneyIcon.SetActive(false);

            // Update shop money display
            updateMoneyTxt(shopMoneyTxt.GetComponent<TextMeshProUGUI>());

            // Setup first button
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(shopFirstButton);

            // Disable player movement
            PlayerMovement.canMove = false;
        }
    }

    public void CloseShopMenu()
    {
        // Enable jab
        PitchforkController.canJab = true;

        // Deactivate the shop UI and optional dark background
        shopMenu.SetActive(false);
        isShopOpen = false;

        // Show normal money UI
        moneyTxt.SetActive(true);
        moneyIcon.SetActive(true);

        // Enable player movement
        PlayerMovement.canMove = true;
    }
}
