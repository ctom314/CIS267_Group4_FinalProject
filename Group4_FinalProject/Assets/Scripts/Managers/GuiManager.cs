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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        updateMoneyText(PersistentData.instance.GetMoney());
    }

    // Update money text
    public void updateMoneyText(int money)
    {
        moneyText.text = money.ToString();
    }
}
