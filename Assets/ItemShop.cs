using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
public class ItemShop : MonoBehaviour
{
    public SlotMachine slotMachine;

    public GameObject buttonPrefab;
    public Transform buttonParent;
public Button rerollItemsButton;  // Nowy przycisk rerollowania przedmiotów
public TMP_Text itemDescriptionText;
    public TMP_Text moneyText;
    public TMP_Text moneyTextSecondary;
public TMP_FontAsset buttonFont; // Assign your desired font in the Inspector
    public Button rerollButton;

    private HashSet<string> boughtItems = new HashSet<string>();
    private int rerollCost = 5;
private Dictionary<string, string> itemDescriptions = new Dictionary<string, string>
{
    {"GoldenMark", "Zwiększa szansę na wylosowanie mareczka o 50%."},
    {"Batteries", "Zmniejsza wartość z przedmiotów o 0.5x, ale daje ci 5 dodatkowych wzorków kiedy rollujesz."},
    {"BrokenWatch", "Zmniejsza cooldown na rollowanie o 0.5 sekundy."},
    {"WildCard", "Każdy wzorek przy rollowaniu zamienia się w jeden."},
    {"Zlotowka", "Dodaje boost do punktów o 1.5x."},
    {"Beer", "Co 5 rolli dostajesz 2x punktów."},
    {"Zeton", "Wydłuża czas rollowania o 10 sekund."},
    {"Ticket", "50% szans na zwiększenie twojej ilości pieniędzy o 2x, 50% szans na zmniejszenie twojej ilości pieniędzy o 2x."},
    {"OrlenCard", "Co 3 rolli masz gwarantowaną szansę na mareczka."},
    {"RyzykFizyk", "50% szans na zwiększenie punktów o 3 razy, 50% szans na zmniejszenie punktów o połowę."},
    {"GrubyTony", "3x punktów, ale zwiększa czas między rollowaniem o 10 sekund."},
    {"ColaCherry", "90% na wylosowanie wisienki, ale dostajesz 7x więcej punktów."},
    {"Kacper", "?."},
    {"D", "Zwiększa czas na rollowanie o 30 sekund."},
    {"Dziecko", "Co dziesięć sekund tracisz 1 dollar, ale dostajesz 2 sloty do maszyny."}
};

    private List<(string, int)> allItems = new List<(string, int)>
    {
        ("GoldenMark", 3),
        ("Batteries", 5),
        ("BrokenWatch", 10),
        ("WildCard", 7),
        ("Zlotowka", 3),

        // NOWE ITEMY:
        ("Beer", 4),
        ("Zeton", 2),
        ("Ticket", 6),
        ("OrlenCard", 8),
        ("RyzykFizyk", 5),
        ("GrubyTony", 7),
        ("ColaCherry", 5),
        ("Kacper", 1),
        ("D", 3),
        ("Dziecko", 6)
    };

    void Start()
{
    rerollButton.onClick.AddListener(RerollItems);  
    rerollItemsButton.onClick.AddListener(RerollShopItems);  // Nowa funkcja rerollowania przedmiotów

    GenerateShopItems();
    UpdateMoneyText();
}


void CreateShopButton(string itemName, int price)
{
    GameObject button = Instantiate(buttonPrefab, buttonParent);

    // Zmieniamy rozmiar przycisku: 4x dłuższy i 2x wyższy
    RectTransform rectTransform = button.GetComponent<RectTransform>();
    rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
rectTransform.pivot = new Vector2(0.5f, 0.5f);
rectTransform.sizeDelta = new Vector2(200f, 60f); // Adjust size to reasonable values
rectTransform.anchoredPosition = Vector2.zero;

    // Ukryj tło (przezroczysty button)
    Image image = button.GetComponent<Image>();
    if (image != null)
    {
        image.color = new Color(0f, 0f, 0f, 0f);
    }

    // Ustawienia tekstu
    TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
    if (buttonText != null)
    {
        buttonText.text = $"{itemName} - ${price}";
        buttonText.color = Color.white;
        buttonText.fontSize = 42;      // Set a smaller, fixed font size
buttonText.font = buttonFont;  // Apply the assigned font
        buttonText.alignment = TextAlignmentOptions.Center;

        // 👇 Najważniejsze rzeczy:
        buttonText.enableWordWrapping = false;
        buttonText.overflowMode = TextOverflowModes.Overflow;
        buttonText.rectTransform.sizeDelta = new Vector2(800f, 120f); // Dopasuj rozmiar tekstu do buttona
    }
    else
    {
        Debug.LogError("Brak komponentu TMP_Text w prefabie przycisku!");
    }

    // Dodaj listener
    button.GetComponent<Button>().onClick.AddListener(() => TryBuyItem(itemName, price));
    EventTrigger trigger = button.GetComponent<EventTrigger>();
if (trigger == null)
    trigger = button.AddComponent<EventTrigger>();
// Pointer Enter (show description)
var pointerEnter = new EventTrigger.Entry();
pointerEnter.eventID = EventTriggerType.PointerEnter;
pointerEnter.callback.AddListener((eventData) => {
    if (itemDescriptions.TryGetValue(itemName, out string desc))
        itemDescriptionText.text = desc;
    else
        itemDescriptionText.text = "";
});
trigger.triggers.Add(pointerEnter);
// Pointer Exit (clear description)
var pointerExit = new EventTrigger.Entry();
pointerExit.eventID = EventTriggerType.PointerExit;
pointerExit.callback.AddListener((eventData) => {
    itemDescriptionText.text = "";
});
trigger.triggers.Add(pointerExit);
}


void ShowItemDescription(string itemName)
{
    Debug.Log($"Hovering over item: {itemName}"); // Debug log
    if (itemDescriptions.TryGetValue(itemName, out string desc))
    {
        itemDescriptionText.text = desc;
    }
    else
    {
        Debug.LogWarning($"Description not found for item: {itemName}"); // Debug log for missing description
        itemDescriptionText.text = "";
    }
}



    void TryBuyItem(string itemName, int price)
{
    // Sprawdzamy, czy przedmiot już został zakupiony
    if (boughtItems.Contains(itemName))
    {
        Debug.Log($"Already bought: {itemName}. You can't buy it again.");
        return;  // Zwróć, nie pozwalając na ponowny zakup
    }

    // Jeżeli gracz ma wystarczająco dużo pieniędzy, dokonujemy zakupu
    if (slotMachine.SpendMoney(price))
    {
        boughtItems.Add(itemName);  // Dodajemy przedmiot do listy zakupów
        UpdateMoneyText();
        Debug.Log($"Bought: {itemName}");

        // Aktywujemy odpowiedni efekt przedmiotu
        switch (itemName)
{
    case "GoldenMark": slotMachine.ActivateGoldenMarkItem(); break;
    case "Batteries": slotMachine.ActivateBatteriesItem(); break;
    case "BrokenWatch": slotMachine.ActivateBrokenWatchItem(); break;
    case "WildCard": slotMachine.ActivateWildCardItem(); break;
    case "Zlotowka": slotMachine.ActivateZlotowkaItem(); break;
    case "Beer": slotMachine.ActivateBeerItem(); break;
    case "Zeton": slotMachine.ActivateZetonItem(); break;
    case "Ticket": slotMachine.ActivateTicketItem(); break;
    case "OrlenCard": slotMachine.ActivateOrlenCardItem(); break;
    case "RyzykFizyk": slotMachine.ActivateRyzykFizykItem(); break;
    case "GrubyTony": slotMachine.ActivateGrubyTonyItem(); break;
    case "ColaCherry": slotMachine.ActivateColaCherryItem(); break;
    case "Kacper": slotMachine.ActivateKacperItem(); break;
    case "D": slotMachine.ActivateDItem(); break;
    case "Dziecko": slotMachine.ActivateDzieckoItem(); break;
}


        // Po zakupie, zaktualizuj sklep
        GenerateShopItems();
    }
    else
    {
        Debug.Log("Not enough money!");
    }
}



    void RerollItems()
    {
        if (slotMachine.GetMoney() >= rerollCost)
        {
            slotMachine.SpendMoney(rerollCost);
            StartCoroutine(RerollAndUpdate());
        }
        else
        {
            Debug.Log("Nie masz wystarczająco pieniędzy, by zrerollować!");
        }
    }

    private IEnumerator RerollAndUpdate()
    {
        GenerateShopItems();
        UpdateMoneyText();
        yield return null;
    }

    void ClearShopButtons()
    {
        foreach (Transform child in buttonParent)
        {
            Destroy(child.gameObject);
        }
    }

    void GenerateShopItems()
{
    ClearShopButtons();

    var randomItems = new List<(string, int)>();
    var pool = new List<(string, int)>(allItems);
    int count = Mathf.Min(4, pool.Count);

    Debug.Log("Generowanie przycisków: " + count + " przedmiotów");

    for (int i = 0; i < count; i++)
    {
        int index = Random.Range(0, pool.Count);
        randomItems.Add(pool[index]);
        pool.RemoveAt(index);
    }

    foreach (var item in randomItems)
    {
        Debug.Log("Dodano przycisk: " + item.Item1);
        CreateShopButton(item.Item1, item.Item2);
    }
}
void RerollShopItems()
{
    GenerateShopItems();  // Ponownie wygeneruj przedmioty
}


    void UpdateMoneyText()
    {
        string moneyStr = "Money: $" + slotMachine.GetMoney();
        if (moneyText != null) moneyText.text = moneyStr;
        if (moneyTextSecondary != null) moneyTextSecondary.text = moneyStr;
    }
}
