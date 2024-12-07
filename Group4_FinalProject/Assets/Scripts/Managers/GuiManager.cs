using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Miscellaneous GUI elements
/// </summary>
public class GuiManager : MonoBehaviour
{
    // UI : Money
    public TextMeshProUGUI moneyText;

    // UI : Seeds
    public TextMeshProUGUI potatoSeedText;
    public TextMeshProUGUI carrotSeedText;
    public TextMeshProUGUI strawberrySeedText;

    private PersistentData persistentData;

    // Start is called before the first frame update
    void Start()
    {
        persistentData = PersistentData.instance;

        // Initialize UI
        updateMoneyText(persistentData.GetMoney());
        UpdateSeedTexts();
    }

    // Update is called once per frame
    void Update()
    {
        updateMoneyText(persistentData.GetMoney());
        UpdateSeedTexts();
    }

    // Update money text
    public void updateMoneyText(int money)
    {
        moneyText.text = money.ToString();
    }

    // Update seed texts
    public void UpdateSeedTexts()
    {
        potatoSeedText.text = $"x{persistentData.GetSeedCount("Potato")}";
        carrotSeedText.text = $"x{persistentData.GetSeedCount("Carrot")}";
        strawberrySeedText.text = $"x{persistentData.GetSeedCount("Strawberry")}";
    }
}
