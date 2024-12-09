using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SiloCanvas : MonoBehaviour
{
    // UI references
    public Button exitBtn;
    public Button sellBtn;
    public TextMeshProUGUI moneyText;

    // Potato controls
    public Button potatoIncreaseBtn;
    public Button potatoDecreaseBtn;
    public TextMeshProUGUI potatoSellAmountText;
    public TextMeshProUGUI potatoTotalAmountText;

    // Carrot controls
    public Button carrotIncreaseBtn;
    public Button carrotDecreaseBtn;
    public TextMeshProUGUI carrotSellAmountText;
    public TextMeshProUGUI carrotTotalAmountText;

    // Strawberry controls
    public Button strawberryIncreaseBtn;
    public Button strawberryDecreaseBtn;
    public TextMeshProUGUI strawberrySellAmountText;
    public TextMeshProUGUI strawberryTotalAmountText;

    // Crop sell counters
    private int potatoSellAmount = 0;
    private int carrotSellAmount = 0;
    private int strawberrySellAmount = 0;

    private void Start()
    {
        exitBtn.onClick.AddListener(CloseCanvas);
        sellBtn.onClick.AddListener(SellCrops);

        // Set up proper button listeners
        potatoIncreaseBtn.onClick.AddListener(() => AdjustCropSellAmount("Potato", 1));
        potatoDecreaseBtn.onClick.AddListener(() => AdjustCropSellAmount("Potato", -1));
        carrotIncreaseBtn.onClick.AddListener(() => AdjustCropSellAmount("Carrot", 1));
        carrotDecreaseBtn.onClick.AddListener(() => AdjustCropSellAmount("Carrot", -1));
        strawberryIncreaseBtn.onClick.AddListener(() => AdjustCropSellAmount("Strawberry", 1));
        strawberryDecreaseBtn.onClick.AddListener(() => AdjustCropSellAmount("Strawberry", -1));

        UpdateCropSellTexts();
    }

    public void updateCropInfo(List<Silo.siloData> data)
    {
        foreach (var crop in data)
        {
            switch (crop.name)
            {
                case "Potato":
                    potatoTotalAmountText.text = crop.amount.ToString();
                    break;
                case "Carrot":
                    carrotTotalAmountText.text = crop.amount.ToString();
                    break;
                case "Strawberry":
                    strawberryTotalAmountText.text = crop.amount.ToString();
                    break;
            }
        }

        potatoSellAmount = 0;
        carrotSellAmount = 0;
        strawberrySellAmount = 0;

        UpdateCropSellTexts();
    }

    private void AdjustCropSellAmount(string crop, int adjustment)
    {
        int maxAmount = 0;
        int curAmount = 0;

        /*// Prevent adjusting values too fast or outside allowed range
        switch (crop)
        {
            case "Potato":
                potatoSellAmount = Mathf.Clamp(potatoSellAmount + adjustment, 0, PersistentData.instance.GetCropCount("Potato"));
                break;
            case "Carrot":
                carrotSellAmount = Mathf.Clamp(carrotSellAmount + adjustment, 0, PersistentData.instance.GetCropCount("Carrot"));
                break;
            case "Strawberry":
                strawberrySellAmount = Mathf.Clamp(strawberrySellAmount + adjustment, 0, PersistentData.instance.GetCropCount("Strawberry"));
                break;
        }*/

        // Get amounts
        switch (crop)
        {
            case "Potato":
                maxAmount = PersistentData.instance.GetCropCount("Potato");
                curAmount = potatoSellAmount;
                break;
            case "Carrot":
                maxAmount = PersistentData.instance.GetCropCount("Carrot");
                curAmount = carrotSellAmount;
                break;
            case "Strawberry":
                maxAmount = PersistentData.instance.GetCropCount("Strawberry");
                curAmount = strawberrySellAmount;
                break;
        }

        // Set sell amount to max amount if trying to decrease below 0
        if (adjustment < 0 && curAmount == 0)
        {
            curAmount = maxAmount;
        }

        // Set sell amount to 0 if trying to increase above max amount
        else if (adjustment > 0 && curAmount == maxAmount)
        {
            curAmount = 0;
        }
        else
        {
            curAmount = Mathf.Clamp(curAmount + adjustment, 0, maxAmount);
        }

        // Apply sell amounts
        switch (crop)
        {
            case "Potato":
                potatoSellAmount = curAmount;
                break;
            case "Carrot":
                carrotSellAmount = curAmount;
                break;
            case "Strawberry":
                strawberrySellAmount = curAmount;
                break;
        }

        UpdateCropSellTexts();
    }

    private void UpdateCropSellTexts()
    {
        potatoSellAmountText.text = potatoSellAmount.ToString();
        carrotSellAmountText.text = carrotSellAmount.ToString();
        strawberrySellAmountText.text = strawberrySellAmount.ToString();

        potatoTotalAmountText.text = PersistentData.instance.GetCropCount("Potato").ToString();
        carrotTotalAmountText.text = PersistentData.instance.GetCropCount("Carrot").ToString();
        strawberryTotalAmountText.text = PersistentData.instance.GetCropCount("Strawberry").ToString();
    }

    public void SellCrops()
    {
        int potatoPrice = 3;
        int carrotPrice = 4;
        int strawberryPrice = 5;

        int totalEarnings = (potatoSellAmount * potatoPrice) +
                            (carrotSellAmount * carrotPrice) +
                            (strawberrySellAmount * strawberryPrice);

        PersistentData.instance.AddMoney(totalEarnings);
        PersistentData.instance.AddCropToSilo("Potato", -potatoSellAmount);
        PersistentData.instance.AddCropToSilo("Carrot", -carrotSellAmount);
        PersistentData.instance.AddCropToSilo("Strawberry", -strawberrySellAmount);

        potatoSellAmount = 0;
        carrotSellAmount = 0;
        strawberrySellAmount = 0;

        UpdateCropSellTexts();

        // Update money text
        ShopInteract.updateMoneyTxt(moneyText);
    }

    private void CloseCanvas()
    {
        gameObject.SetActive(false);
    }
}
