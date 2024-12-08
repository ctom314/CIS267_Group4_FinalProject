using System.Collections;
using TMPro;
using UnityEngine;

public class GuiManager : MonoBehaviour
{
    // UI : Money
    public TextMeshProUGUI moneyText;

    // UI : Seeds
    public TextMeshProUGUI potatoSeedText;
    public TextMeshProUGUI carrotSeedText;
    public TextMeshProUGUI strawberrySeedText;

    // UI : Harvested Crops in Silo
    public TextMeshProUGUI potatoHarvestText;
    public TextMeshProUGUI carrotHarvestText;
    public TextMeshProUGUI strawberryHarvestText;

    private PersistentData persistentData;

    private void Start()
    {
        // Check if PersistentData is initialized
        if (PersistentData.instance == null)
        {
            Debug.LogError("PersistentData is not initialized! Ensure it exists in the scene.");
            return;
        }

        persistentData = PersistentData.instance;

        // Log initialization
        Debug.Log($"GuiManager initialized. Current money: {persistentData.GetMoney()}");

        // Initialize UI with current data
        UpdateAllUI();
    }

    // Update is called once per frame (ensures UI stays in sync)
    private void Update()
    {
        // Update the money and seed UI elements continuously to stay consistent
        UpdateMoneyText();
        UpdateSeedTexts();
        UpdateHarvestTexts();
    }

    // Method to update all UI elements at once
    public void UpdateAllUI()
    {
        UpdateMoneyText();
        UpdateSeedTexts();
        UpdateHarvestTexts();
    }

    // Update money text
    public void UpdateMoneyText()
    {
        if (persistentData != null)
        {
            int currentMoney = persistentData.GetMoney();
            moneyText.text = currentMoney.ToString();
        }
        else
        {
            Debug.LogError("PersistentData is not assigned!");
        }
    }

    // Update seed texts
    public void UpdateSeedTexts()
    {
        if (persistentData != null)
        {
            potatoSeedText.text = $"x{persistentData.GetSeedCount("Potato")}";
            carrotSeedText.text = $"x{persistentData.GetSeedCount("Carrot")}";
            strawberrySeedText.text = $"x{persistentData.GetSeedCount("Strawberry")}";
        }
        else
        {
            Debug.LogError("PersistentData is not assigned!");
        }
    }

    // Update harvested crop texts
    public void UpdateHarvestTexts()
    {
        if (persistentData != null)
        {
            potatoHarvestText.text = $"x{persistentData.GetCropCount("Potato")}";
            carrotHarvestText.text = $"x{persistentData.GetCropCount("Carrot")}";
            strawberryHarvestText.text = $"x{persistentData.GetCropCount("Strawberry")}";
        }
        else
        {
            Debug.LogError("PersistentData is not assigned!");
        }
    }
}
