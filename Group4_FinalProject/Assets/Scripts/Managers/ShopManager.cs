using UnityEngine;
using UnityEngine.UI;
using TMPro; // For dynamic text updates

public class ShopManager : MonoBehaviour
{
    public TextMeshProUGUI staminaLevelText; // Reference to "Stamina lv." text
    public TextMeshProUGUI buttonText; // Reference to the button text
    public Button staminaUpgradeButton; // Reference to the stamina upgrade button

    private int currentStaminaLevel = 1; // Tracks current stamina level
    private int[] staminaUpgradeCosts = { 500, 1000, 1500 }; // Costs for each upgrade
    private int maxStaminaLevel = 4; // Max stamina level

    private PlayerMovement playerMovement; // Reference to PlayerMovement script
    private PersistentData persistentData; // Reference to PersistentData

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

            // Upgrade stamina in the PlayerMovement script
            playerMovement.maxStamina += 1.0f;

            // Increment stamina level
            currentStaminaLevel++;

            // Update the shop UI
            UpdateShopUI();
        }
        else
        {
            Debug.Log("Not enough money to buy this upgrade!");
        }
    }
}
