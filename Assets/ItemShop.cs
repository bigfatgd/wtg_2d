using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class ItemShop : MonoBehaviour
{
    public SlotMachine slotMachine;

    public Button goldenMarkButton;
    public Button batteriesButton;
    public Button brokenWatchButton;
    public Button wildCardButton;
    public Button zlotowkaButton;

    public TMP_Text moneyText;
    public TMP_Text moneyTextSecondary;

    private HashSet<string> boughtItems = new HashSet<string>();

    void Start()
    {
        goldenMarkButton.onClick.AddListener(() => TryBuyItem("GoldenMark", 3, slotMachine.ActivateGoldenMark));
        batteriesButton.onClick.AddListener(() => TryBuyItem("Batteries", 5, slotMachine.ActivateBatteries));
        brokenWatchButton.onClick.AddListener(() => TryBuyItem("BrokenWatch", 10, slotMachine.ActivateBrokenWatch));
        wildCardButton.onClick.AddListener(() => TryBuyItem("WildCard", 7, slotMachine.ActivateWildCard));
        zlotowkaButton.onClick.AddListener(() => TryBuyItem("Zlotowka", 3, slotMachine.ActivateOneZlotowka));
        UpdateMoneyText();
    }

    void TryBuyItem(string itemName, int price, System.Action onBuy)
    {
        if (boughtItems.Contains(itemName))
        {
            Debug.Log($"Already bought: {itemName}");
            return;
        }

        if (slotMachine.SpendMoney(price))
        {
            onBuy.Invoke();
            boughtItems.Add(itemName);
            UpdateMoneyText();
        }
        else
        {
            Debug.Log("Not enough money!");
        }
    }

    void UpdateMoneyText()
    {
        string moneyStr = "Money: $" + slotMachine.GetMoney();
        if (moneyText != null) moneyText.text = moneyStr;
        if (moneyTextSecondary != null) moneyTextSecondary.text = moneyStr;
    }
}
