using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SlotMachine : MonoBehaviour
{
    [System.Serializable]
    public class SymbolSprite
    {
        public string symbol;
        public Sprite sprite;
    }

    [Header("UI References")]
    public TMP_Text resultText;
    public TMP_Text moneyText;
    public TMP_Text moneyTextSecondary;
    public Button rollButton;
[Header("Audio")]
public AudioSource audioSource;
public AudioClip clickSound;
public AudioClip scoreBelow200;
public AudioClip score200To499;
public AudioClip score500To999;
public AudioClip score1000To1999;
public AudioClip score2000To4999;
public AudioClip score5000To10000;
public AudioClip scoreAbove10000;

    [Header("Animations")]
    public Animator handleAnimator;
    public Animator slotAnimator;

    [Header("Image Display")]
    public Transform resultImageContainer; // Parent container (with HorizontalLayoutGroup)
    public GameObject symbolImagePrefab; // Prefab with Image component
    public List<SymbolSprite> symbolSprites;
    public Transform bossResultImageContainer;
    public bool isBossRolling = false;
    public float visualRollDelay = 2.0f; // Used in boss phase
    private bool isPlayerBossRoll = false; // NEW: tracks if it's the player rolling in boss phase
public void ActivateBeer() => hasBeer = true;
public void ActivateZeton() => hasZeton = true;
public void ActivateTicket() => hasTicket = true;
public void ActivateDiamondCard() => hasDiamondCard = true;
public void ActivateRyzykFizyk() => hasRyzykFizyk = true;
public void ActivateGrubyTony() => hasGrubyTony = true;
public void ActivateColaCherry() => hasColaCherry = true;
public void ActivateKacper() => hasKacper = true;
public void ActivateD() => hasD = true;
public void ActivateDziecko() => hasDziecko = true;


    [Header("Game Settings")]
    public float rollCooldown = 1.0f;
    public int pointsToDollar = 1000;
public float additionalRollTime = 0f; // Czas dodany przez przedmioty takie jak D i Zeton

    private Dictionary<string, Sprite> symbolSpriteDict = new Dictionary<string, Sprite>();
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

    private bool randomizeSymbols = false;
    private int extraSymbols = 0;
    private float bonusMultiplier = 1.0f;
    private float pointReductionMultiplier = 1.0f;
private int rollCount = 0;
private bool hasBeer = false;
private bool hasZeton = false;
private bool hasTicket = false;
private bool hasDiamondCard = false;
private bool hasRyzykFizyk = false;
private bool hasGrubyTony = false;
private bool hasColaCherry = false;
private bool hasKacper = false; // Nie robi nic
private bool hasD = false;
private bool hasDziecko = false;
 private bool hasGoldenMark = false;
    private bool hasBatteries = false;
    private bool hasBrokenWatch = false;
    private bool hasWildCard = false;
    private bool hasZlotowka = false;
private float rollTimer = 0f;
private float dzieckoTimer = 0f;

    void Start()
{
    rollButton.onClick.AddListener(() =>
    {
        if (audioSource != null && clickSound != null)
            audioSource.PlayOneShot(clickSound);

        StartCoroutine(Roll());
    });

    UpdateMoneyText();

    // Load symbol-to-sprite mapping
    foreach (var item in symbolSprites)
    {
        symbolSpriteDict[item.symbol] = item.sprite;
    }
}


    private IEnumerator PlaySlotAnimations()
{

    // Trigger animations
    if (handleAnimator != null)
        handleAnimator.SetTrigger("Pull");

    if (slotAnimator != null)
        slotAnimator.SetTrigger("Spin");

    // Wait for animations to complete (adjust timing to match your animation length)
    yield return new WaitForSeconds(2.0f); // Adjust to match animation duration

}


IEnumerator Roll(bool showPlayerVisuals = true, bool useBossContainer = false)
{
    rollButton.interactable = false;

    // Sprawdzamy, czy przedmioty D lub Zeton są aktywne
    float rollCooldownWithItems = rollCooldown + additionalRollTime;

    // Czas oczekiwania na kolejne losowanie
    yield return new WaitForSeconds(rollCooldownWithItems);

    // Reszta logiki...


    rollCounter++;

    rollButton.interactable = false;
rollCount++;

if (hasDziecko)
{
    dzieckoTimer += Time.deltaTime;
    if (dzieckoTimer >= 10f && totalMoney > 0)
    {
        totalMoney--;
        extraSymbols += 2;
        dzieckoTimer = 0f;
    }
}

    // Play animation before rolling
    yield return StartCoroutine(PlaySlotAnimations());

    float delay = rollCooldown;
if (hasZeton) delay += 10f;
if (hasD) delay += 30f;
yield return new WaitForSeconds(delay);


    // (rest of roll logic stays the same...)
foreach (Transform child in resultImageContainer)
{
    Destroy(child.gameObject);
}


    rollResults.Clear();
    basePoints = 0;
    totalMultiplier = 1.0f;

    int numSymbols = 5 + extraSymbols;
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
    PlayScoreSound(finalScore);

    // BEER: co 5 rolli 2x punkty
if (hasBeer && rollCount % 5 == 0)
{
    finalScore *= 2;
}

// RYZYK FIZYK: 50/50 szansa na x3 lub /2
if (hasRyzykFizyk)
{
    if (Random.value < 0.5f)
        finalScore *= 3;
    else
        finalScore = Mathf.RoundToInt(finalScore * 0.5f);
}

// GRUBY TONY: 3x punkty, ale długi cooldown
if (hasGrubyTony)
{
    finalScore *= 3;
    rollCooldown = 10f;
}

// TICKET: 50% szansa na 2x lub /2 pieniędzy
if (hasTicket)
{
    if (Random.value < 0.5f)
        totalMoney *= 2;
    else
        totalMoney = Mathf.FloorToInt(totalMoney / 2f);
    hasTicket = false; // jednorazowe użycie
}

// DIAMENTOWA KARTA ORLEN: co 3 roll – mareczek
if (hasDiamondCard && rollCount % 3 == 0)
{
    rollResults.Clear();
    for (int i = 0; i < numSymbols; i++)
        rollResults.Add("mareczek");

    basePoints = 100 * numSymbols;
    totalMultiplier = 1.5f * numSymbols;
}

// COLA CHERRY: 90% szansa na wisienkę, 7x punkty
if (hasColaCherry && Random.value <= 0.9f)
{
    rollResults.Clear();
    for (int i = 0; i < numSymbols; i++)
        rollResults.Add("wisienka");

    basePoints = 20 * numSymbols;
    totalMultiplier = 1.0f * numSymbols;
    finalScore = basePoints * 7;
}

    totalAccumulatedPoints += finalScore;

    while (totalAccumulatedPoints >= pointsToDollar)
    {
        totalMoney++;
        totalAccumulatedPoints -= pointsToDollar;
    }

    if (showPlayerVisuals)
        DisplayResults(useBossContainer);

    UpdateMoneyText();
    rollButton.interactable = true;
}
public void ActivateDItem()
{
    additionalRollTime = 30f;  // Dodaj 30 sekund
}

public void ActivateZetonItem()
{
    additionalRollTime = 10f;  // Dodaj 10 sekund
}

public void ApplyItemEffect(string itemName)
{
    switch (itemName)
    {
        case "GoldenMark":
            ActivateGoldenMark();
            break;
        case "Batteries":
            ActivateBatteries();
            break;
        case "BrokenWatch":
            ActivateBrokenWatch();
            break;
        case "WildCard":
            ActivateWildCard();
            break;
        case "Zlotowka":
            ActivateOneZlotowka();
            break;

        // Nowe itemy
        case "Beer":
            StartCoroutine(BeerEffect());
            break;
        case "Zeton":
            rollCooldown += 10f;
            break;
        case "Ticket":
            if (Random.value < 0.5f)
                totalMoney *= 2;
            else
                totalMoney = Mathf.Max(0, totalMoney / 2);
            break;
        case "OrlenCard":
            StartCoroutine(OrlenCardEffect());
            break;
        case "RyzykFizyk":
            if (Random.value < 0.5f)
                bonusMultiplier *= 3f;
            else
                bonusMultiplier *= 0.5f;
            break;
        case "GrubyTony":
            bonusMultiplier *= 3f;
            rollCooldown = 10f;
            break;
        case "ColaCherry":
            symbols.Clear();
            symbols.Add(("wisienka", 20, 1.0f, 0.9f));
            break;
        case "Kacper":
            break; // nic nie robi
        case "D":
            rollCooldown += 30f;
            break;
        case "Dziecko":
            StartCoroutine(DzieckoEffect());
            break;
    }
}
private int rollCounter = 0;

private IEnumerator BeerEffect()
{
    while (true)
    {
        yield return new WaitUntil(() => rollCounter > 0 && rollCounter % 5 == 0);
        bonusMultiplier *= 2f;
        yield return new WaitForSeconds(0.5f); // delay, by nie wywołać kilka razy
        bonusMultiplier /= 2f;
    }
}

private IEnumerator OrlenCardEffect()
{
    while (true)
    {
        yield return new WaitUntil(() => rollCounter > 0 && rollCounter % 3 == 0);
        randomizeSymbols = true;
        yield return new WaitForSeconds(0.1f);
        randomizeSymbols = false;
    }
}

private IEnumerator DzieckoEffect()
{
    while (true)
    {
        yield return new WaitForSeconds(10f);
        totalMoney = Mathf.Max(0, totalMoney - 1);
        extraSymbols += 2;
    }
}


    void DisplayResults(bool showInBossUI)
{
    Transform container = showInBossUI ? bossResultImageContainer : resultImageContainer;

    foreach (Transform child in container)
    {
        Destroy(child.gameObject);
    }

    foreach (string symbol in rollResults)
    {
        GameObject imgObj = Instantiate(symbolImagePrefab, container);
        Image img = imgObj.GetComponent<Image>();

        if (symbolSpriteDict.TryGetValue(symbol, out Sprite sprite))
        {
            img.sprite = sprite;
        }
    }

    container.gameObject.SetActive(true); // ✅ Moved here
    resultText.text = "Score: " + GetScore();
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
        return symbols[0].symbol;
    }

public IEnumerator RollAndGetScore(bool bossRoll = false, bool isPlayerInBoss = false)
{
    isBossRolling = bossRoll;
    isPlayerBossRoll = isPlayerInBoss;

    // Boss should not trigger any UI
    bool showVisuals = !bossRoll;
    yield return Roll(showVisuals, isPlayerInBoss);

    isBossRolling = false;
    isPlayerBossRoll = false;
}

void PlayScoreSound(int score)
{
    if (audioSource == null) return;

    if (score < 200)
        audioSource.PlayOneShot(scoreBelow200);
    else if (score < 500)
        audioSource.PlayOneShot(score200To499);
    else if (score < 1000)
        audioSource.PlayOneShot(score500To999);
    else if (score < 2000)
        audioSource.PlayOneShot(score1000To1999);
    else if (score < 5000)
        audioSource.PlayOneShot(score2000To4999);
    else if (score <= 10000)
        audioSource.PlayOneShot(score5000To10000);
    else
        audioSource.PlayOneShot(scoreAbove10000);
}


    public int GetScore()
    {
        return Mathf.RoundToInt(basePoints * totalMultiplier * bonusMultiplier * pointReductionMultiplier);
    }

    void UpdateMoneyText()
    {
        string moneyStr = "Money: $" + totalMoney;
        moneyText.text = moneyStr;
        if (moneyTextSecondary != null)
            moneyTextSecondary.text = moneyStr;
    }

    // Boss Effects
    public void ActivateBatteries()
    {
        extraSymbols = 5;
        pointReductionMultiplier = 0.5f;
    }

    public void ActivateGoldenMark()
    {
        for (int i = 0; i < symbols.Count; i++)
        {
            if (symbols[i].symbol == "mareczek")
            {
                var s = symbols[i];
                symbols[i] = (s.symbol, s.points, s.mult, s.chance + 0.05f);
            }
        }
    }

    public void ActivateBrokenWatch()
    {
        rollCooldown = 0.5f;
    }

    public void ResetBossEffects()
    {
        extraSymbols = 0;
        bonusMultiplier = 1.0f;
        randomizeSymbols = false;
        pointReductionMultiplier = 1.0f;
    }

    public void ActivateWildCard()
    {
        randomizeSymbols = true;
    }

    public void ActivateOneZlotowka()
    {
        bonusMultiplier = 1.5f;
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
  
    public void ActivateGoldenMarkItem()
    {
        Debug.Log("GoldenMark activated - +50% chance for Mareczek");
        // Increase the chance of "mareczek" by 50% (0.5 as 50%)
        for (int i = 0; i < symbols.Count; i++)
        {
            if (symbols[i].symbol == "mareczek")
            {
                var s = symbols[i];
                // Add 0.5 to chance (50%)
                symbols[i] = (s.symbol, s.points, s.mult, Mathf.Min(s.chance + 0.5f, 1f));
            }
        }
        hasGoldenMark = true;
    }

    public void ActivateBatteriesItem()
    {
        Debug.Log("Batteries activated - 0.5x value, +5 symbols");
        pointReductionMultiplier = 0.5f; // Halve item value
        extraSymbols += 5;              // Add 5 extra symbols on roll
        hasBatteries = true;
    }

    public void ActivateBrokenWatchItem()
    {
        Debug.Log("BrokenWatch activated - -0.5s cooldown");
        rollCooldown = Mathf.Max(0.1f, rollCooldown - 0.5f); // Reduce cooldown by 0.5 seconds, but not below 0.1
        hasBrokenWatch = true;
    }

    public void ActivateWildCardItem()
    {
        Debug.Log("WildCard activated - All symbols become same");
        randomizeSymbols = true; // Make all rolled symbols the same during roll
        hasWildCard = true;
    }

    public void ActivateZlotowkaItem()
    {
        Debug.Log("Zlotowka activated - +1.5x point boost");
        bonusMultiplier *= 1.5f; // Increase points gain by 1.5x
        hasZlotowka = true;
    }

    public void ActivateBeerItem()
    {
        Debug.Log("Beer activated - Every 5 rolls = 2x points");
        hasBeer = true;
        // beer effect logic handled in Roll coroutine
    }


    public void ActivateTicketItem()
    {
        Debug.Log("Ticket activated - 50/50 x2 or /2 money");
        hasTicket = true;
        // one time effect handled in Roll method, after which hasTicket is set to false
    }

    public void ActivateOrlenCardItem()
    {
        Debug.Log("OrlenCard activated - Every 3rd roll guaranteed Mareczek");
        hasDiamondCard = true;
        // Effect handled in Roll coroutine checking roll count
    }

    public void ActivateRyzykFizykItem()
    {
        Debug.Log("RyzykFizyk activated - 50/50 x3 or /2 points");
        hasRyzykFizyk = true;
        // Effect handled in Roll coroutine with random 50% chance
    }

    public void ActivateGrubyTonyItem()
    {
        Debug.Log("GrubyTony activated - 3x points + 10s cooldown");
        bonusMultiplier *= 3f;  // Triple points
        rollCooldown = 10f;     // Set cooldown to 10 seconds
        hasGrubyTony = true;
    }

    public void ActivateColaCherryItem()
    {
        Debug.Log("ColaCherry activated - 90% cherry chance, 7x points");
        // Modify symbols list to favor "wisienka" with 90% chance
        symbols.Clear();
        symbols.Add(("wisienka", 20, 1.0f, 0.9f));
        symbols.Add(("siodemka", 75, 1.4f, 0.033f));
        symbols.Add(("mareczek", 100, 1.5f, 0.033f));
        symbols.Add(("osemka", 30, 1.1f, 0.034f));
        symbols.Add(("ogien", 75, 1.0f, 0.033f));
        symbols.Add(("serce", 5, 1.35f, 0.02f));
        symbols.Add(("ptaszek", 50, 1.5f, 0.01f));
        symbols.Add(("kosmita", 300, 1.0f, 0.01f));

        bonusMultiplier *= 7f; // 7x points
        hasColaCherry = true;
    }

    public void ActivateKacperItem()
    {
        Debug.Log("Kacper activated - Does nothing");
        hasKacper = true;
    }


    public void ActivateDzieckoItem()
    {
        Debug.Log("Dziecko activated - Lose $1 every 10 seconds, +2 extra symbols");
        hasDziecko = true;
        // Start coroutine that deducts money every 10 seconds and adds 2 extra symbols
        StartCoroutine(DzieckoEffect());
    }


    
}