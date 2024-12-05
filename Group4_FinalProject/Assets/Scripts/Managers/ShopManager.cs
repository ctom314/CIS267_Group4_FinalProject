using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public TextMeshProUGUI staminaLevelText; // Reference to "Stamina lv." text
    public TextMeshProUGUI buttonText; // Reference to the button text
    public Button staminaUpgradeButton; // Reference to the stamina upgrade button

    private int currentStaminaLevel = 1; // Tracks current stamina level
    private int[] staminaUpgradeCosts = { 500, 1000, 2000 }; // Costs for each upgrade
    private int maxStaminaLevel = 4; // Max stamina level

    private PlayerMovement playerMovement; // Reference to PlayerMovement script
    private PersistentData persistentData; // Reference to PersistentData

    // Stamina UI levels (assign these in the Inspector)
    public GameObject[] staminaUILevels; // Array of GameObjects for each Stamina UI setup

    void Start()
    {
        // Get references
        playerMovement = FindObjectOfType<PlayerMovement>();
        persistentData = PersistentData.instance;

        // Initialize the button and text
        UpdateShopUI();

        // Add listener for the button click
        staminaUpgradeButton.onClick.AddListener(UpgradeStamina);
    }

    void UpdateShopUI()
    {
        // Update the stamina level text
        staminaLevelText.text = $"Stamina lv.{currentStaminaLevel}";

        // Update the button text
        if (currentStaminaLevel < maxStaminaLevel)
        {
            buttonText.text = $"${staminaUpgradeCosts[currentStaminaLevel - 1]}";
        }
        else
        {
            // Max level reached, disable the button
            buttonText.text = "MAX";
            staminaUpgradeButton.interactable = false;
            Debug.Log("Stamina upgrade is at MAX level!");
        }
    }

    void UpgradeStamina()
    {
        if (currentStaminaLevel >= maxStaminaLevel)
        {
            Debug.Log("Stamina is already at max level!");
            return;
        }

        int upgradeCost = staminaUpgradeCosts[currentStaminaLevel - 1];

        // Check if the player has enough money
        if (persistentData.GetMoney() >= upgradeCost)
        {
            // Deduct money
            persistentData.SetMoney(persistentData.GetMoney() - upgradeCost);
            Debug.Log($"Money deducted: {upgradeCost}. Remaining money: {persistentData.GetMoney()}");

            // Upgrade stamina in the PlayerMovement script
            playerMovement.maxStamina += 1.0f;
            Debug.Log($"Stamina upgraded to Lv{currentStaminaLevel + 1}!");

            // Increment stamina level BEFORE updating the UI
            currentStaminaLevel++;

            // Update the stamina UI
            UpdateStaminaUI();

            // Update the shop UI
            UpdateShopUI();
        }
        else
        {
            Debug.Log("Not enough money to buy this upgrade!");
        }
    }

    void UpdateStaminaUI()
    {
        Debug.Log($"Updating Stamina UI for Level {currentStaminaLevel}...");

        // Deactivate all UI levels first
        for (int i = 0; i < staminaUILevels.Length; i++)
        {
            staminaUILevels[i].SetActive(false);
            Debug.Log($"Sprint UI Lv{i + 1} disabled.");
        }

        // Activate the current UI level
        int uiIndex = currentStaminaLevel - 1; // Get the correct array index
        if (uiIndex < staminaUILevels.Length)
        {
            staminaUILevels[uiIndex].SetActive(true);
            Debug.Log($"Sprint UI Lv{currentStaminaLevel} enabled.");

            // Update the current sprint bar in PlayerMovement
            Transform greyscaleBar = staminaUILevels[uiIndex].transform.Find("Greyscale Sprint");
            Transform flashOverlay = staminaUILevels[uiIndex].transform.Find("StaminaFlashOverlay");

            if (greyscaleBar != null && flashOverlay != null)
            {
                playerMovement.currentSprintBar = greyscaleBar.gameObject;
                playerMovement.staminaFlashOverlay = flashOverlay.gameObject; // Update flash overlay reference
                Debug.Log("PlayerMovement sprint bar and flash overlay references updated.");
            }
            else
            {
                Debug.LogWarning("Greyscale Sprint or StaminaFlashOverlay not found in the current UI level.");
            }
        }
        else
        {
            Debug.LogWarning($"Invalid UI index: {uiIndex}. Check staminaUILevels array!");
        }
    }
}
