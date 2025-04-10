using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class SlotMachine : MonoBehaviour
{
    public TMP_Text resultText;
    public TMP_Text moneyText;
    public TMP_Text moneyTextSecondary; // Drugi slot tekstowy na hajs
    public Button rollButton;
    public float rollCooldown = 1.0f; // Czas oczekiwania przed kolejnym rzutem

    private List<(string symbol, int points, float mult, float chance)> symbols = new List<(string, int, float, float)>
    {
        ("wisienka", 20, 1.0f, 0.25f),
        ("siodemka", 75, 1.4f, 0.15f),
        ("mareczek", 100, 1.5f, 0.1f),
        ("osemka", 30, 1.1f, 0.2f),
        ("ogien", 75, 1.0f, 0.1f),
        ("serce", 5, 1.35f, 0.1f),
        ("ptaszek", 50, 1.5f, 0.075f),
        ("kosmita", 300, 1.0f, 0.025f)
    };

    private List<string> rollResults = new List<string>();
    private int basePoints = 0;
    private float totalMultiplier = 1.0f;
    private int totalMoney = 0;
    private int totalAccumulatedPoints = 0;
    private int pointsToDollar = 1000;

    // Zmienna kontrolująca efekty przedmiotów
    private bool randomizeSymbols = false; // Włączanie/wyłączanie randomizowania symboli (np. Wild Card)
    private int extraSymbols = 0; // Ilość dodatkowych symboli (np. Baterie dodają dodatkowe symbole)
    private float bonusMultiplier = 1.0f; // Multiplier (np. 1 Złotówka dodaje bonus 1.5x)
    private float pointReductionMultiplier = 1.0f; // Zmniejszenie punktów (np. Baterie zmniejszają punkty o połowę)

    void Start()
    {
        rollButton.onClick.AddListener(() => StartCoroutine(Roll()));
        UpdateMoneyText();
    }

    IEnumerator Roll()
    {
        rollButton.interactable = false;
        yield return new WaitForSeconds(rollCooldown);

        rollResults.Clear();
        basePoints = 0;
        totalMultiplier = 1.0f;

        int numSymbols = 5 + extraSymbols; // 5 rolli na rundę
        string chosenSymbol = randomizeSymbols ? GetRandomSymbol() : null;

        for (int i = 0; i < numSymbols; i++)
        {
            string rolledSymbol = randomizeSymbols ? chosenSymbol : GetRandomSymbol();
            var symbolData = symbols.Find(s => s.symbol == rolledSymbol);
            rollResults.Add(rolledSymbol);
            basePoints += symbolData.points;
            totalMultiplier += symbolData.mult - 1.0f;
        }

        int finalScore = Mathf.RoundToInt(basePoints * totalMultiplier * bonusMultiplier * pointReductionMultiplier);
        totalAccumulatedPoints += finalScore;

        while (totalAccumulatedPoints >= pointsToDollar)
        {
            totalMoney++;
            totalAccumulatedPoints -= pointsToDollar;
        }

        resultText.text = "Rolled: " + string.Join(", ", rollResults) + "\nScore: " + finalScore;
        UpdateMoneyText();

        rollButton.interactable = true;
    }

    string GetRandomSymbol()
    {
        float roll = Random.value;
        float cumulative = 0f;

        foreach (var symbol in symbols)
        {
            cumulative += symbol.chance;
            if (roll <= cumulative)
                return symbol.symbol;
        }
        return symbols[0].symbol; // Fallback
    }

    public IEnumerator RollAndGetScore()
    {
        rollButton.interactable = false;
        yield return new WaitForSeconds(rollCooldown);

        rollResults.Clear();
        basePoints = 0;
        totalMultiplier = 1.0f;

        int numSymbols = 5 + extraSymbols; // W zależności od przedmiotów
        string chosenSymbol = randomizeSymbols ? GetRandomSymbol() : null;

        for (int i = 0; i < numSymbols; i++)
        {
            string rolledSymbol = randomizeSymbols ? chosenSymbol : GetRandomSymbol();
            var symbolData = symbols.Find(s => s.symbol == rolledSymbol);
            rollResults.Add(rolledSymbol);
            basePoints += symbolData.points;
            totalMultiplier += symbolData.mult - 1.0f;
        }

        int finalScore = Mathf.RoundToInt(basePoints * totalMultiplier * bonusMultiplier * pointReductionMultiplier);
        totalAccumulatedPoints += finalScore;

        while (totalAccumulatedPoints >= pointsToDollar)
        {
            totalMoney++;
            totalAccumulatedPoints -= pointsToDollar;
        }

        resultText.text = "Rolled: " + string.Join(", ", rollResults) + "\nScore: " + finalScore;
        UpdateMoneyText();

        rollButton.interactable = true;

        yield return null; // Zwracamy wynik po turze
    }

    // Nowa funkcja GetScore
    public int GetScore()
    {
        return Mathf.RoundToInt(basePoints * totalMultiplier * bonusMultiplier * pointReductionMultiplier);
    }

    void UpdateMoneyText()
    {
        string moneyStr = "Money: $" + totalMoney;
        moneyText.text = moneyStr;
        if (moneyTextSecondary != null)
            moneyTextSecondary.text = moneyStr; // Aktualizacja drugiego slotu
    }

    // Funkcje aktywujące przedmioty
    public void ActivateBatteries()
    {
        extraSymbols = 5;  // Baterie dodają dodatkowe symbole
        pointReductionMultiplier = 0.5f;  // Zmniejszają punkty o połowę
    }

    public void ActivateGoldenMark()
    {
        // Zwiększają szansę na "mareczek" o 50%
        for (int i = 0; i < symbols.Count; i++)
        {
            if (symbols[i].symbol == "mareczek")
            {
                var symbol = symbols[i];
                symbol = (symbol.symbol, symbol.points, symbol.mult, symbol.chance + 0.05f);
                symbols[i] = symbol;
            }
        }
    }

    public void ActivateBrokenWatch()
    {
        rollCooldown = 0.5f; // Zmniejszają czas oczekiwania na połowę sekundy
    }
    
public void ResetBossEffects()
{
    extraSymbols = 0;  // Reset dodatkowych symboli
    bonusMultiplier = 1.0f; // Reset multiplikatora bonusu
    randomizeSymbols = false; // Wyłącz wild cards
    pointReductionMultiplier = 1.0f; // Reset multiplikatora punktów
}

    public void ActivateWildCard()
    {
        randomizeSymbols = true; // Każdy symbol staje się wild card
    }

    public void ActivateOneZlotowka()
    {
        bonusMultiplier = 1.5f; // Zwiększa punkty o 1.5x
    }

    public int GetMoney()
    {
        return totalMoney;
    }

    public bool SpendMoney(int amount)
    {
        if (totalMoney >= amount)
        {
            totalMoney -= amount;
            return true;
        }
        return false;
    }
}
