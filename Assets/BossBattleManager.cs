using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BossBattleManager : MonoBehaviour
{
    public SlotMachine slotMachine; // SlotMachine script
    public TMP_Text battleResultText;
    public Button playerRollButton; // Button for player's roll
    public TMP_Text playerScoreText;
    public TMP_Text bossScoreText;
    public Camera mainCamera; // Camera for moving

    private int playerScore = 0;
    private int bossScore = 0;
    private int rollsLeft = 5; // 5 rolls per turn (as per original game)
    private bool isBattleActive = false;
    private bool isPlayerTurn = true; // Track if it's player or boss's turn

    void Start()
    {
        // Initially, only the roll button is visible
        playerRollButton.gameObject.SetActive(true); // Enable roll button initially
        playerRollButton.onClick.AddListener(PlayerRoll); // Make sure the roll button calls PlayerRoll on click
        battleResultText.text = "";
    }

    public void StartBattle()
{
    if (isBattleActive) return; // Prevent starting a new battle while one is active

    isBattleActive = true;
    rollsLeft = 5;
    playerScore = 0;
    bossScore = 0;
    battleResultText.text = "Battle Started!";
    
    // Start the battle with player's turn
    StartCoroutine(PlayerTurn());
}


    IEnumerator PlayerTurn()
    {
        while (rollsLeft > 0)
        {
            // Wait for the player to click the roll button, don't do it automatically
            yield return null; // Just wait for the button press to call PlayerRoll()
        }

        // After player's turn, it's the boss's turn
        battleResultText.text = "Boss's Turn!";
        yield return new WaitForSeconds(1f); // Delay before boss's move

        // Boss starts rolling
        StartCoroutine(BossTurn());
    }

    IEnumerator GetPlayerScore()
    {
        // Wait for the score from the slot machine (the RollAndGetScore method should return the score)
        yield return StartCoroutine(slotMachine.RollAndGetScore());

        // Assuming RollAndGetScore() modifies a public variable in slotMachine to hold the score
        int scoreFromRoll = slotMachine.GetScore(); // This method should return the score the player has earned from the roll
        playerScore += scoreFromRoll; // Add score to player's total
    }

    IEnumerator BossTurn()
{
    int bossRolls = 5; // Boss ma 5 rzutów
    slotMachine.ResetBossEffects(); // Resetowanie efektów power-upów dla bossa

    while (bossRolls > 0)
    {
        // Symulujemy rzut bossa
        yield return StartCoroutine(slotMachine.RollAndGetScore()); 

        // Zliczanie wyników
        int scoreFromBossRoll = slotMachine.GetScore(); 
        bossScore += scoreFromBossRoll; // Dodawanie punktów
        bossRolls--; // Zmniejszanie liczby rzutów
        bossScoreText.text = "Boss Score: " + bossScore;
        yield return new WaitForSeconds(1f); // Krótkie opóźnienie między rzutami
    }

    // Po turze bossa kończymy bitwę
    EndBattle();
}


    void EndBattle()
    {
        // If player's score is less than boss's score
        if (playerScore < bossScore)
        {
            battleResultText.text = "You Lost! Moving to next scene!";
            StartCoroutine(MoveToNextScene());
        }
        else
        {
            battleResultText.text = "You Won!";
            // Move camera to the right if player wins
            StartCoroutine(MoveCameraRight());
            // You can add bonuses, rewards, etc.
        }
    }

    IEnumerator MoveToNextScene()
    {
        yield return new WaitForSeconds(2f); // Wait a moment before changing scene
        int nextSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1; // Change to the next scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneIndex); // Load the next scene
    }

    IEnumerator MoveCameraRight()
    {
        // Instead of moving smoothly, instantly teleport the camera
        mainCamera.transform.position = new Vector3(mainCamera.transform.position.x + 5f, mainCamera.transform.position.y, mainCamera.transform.position.z);
        yield return null;
    }

    void PlayerRoll()
    {
        if (rollsLeft > 0)
        {
            // Player presses the roll button, perform roll action
            StartCoroutine(GetPlayerScore());

            // Update score and roll count
            rollsLeft--;
            playerScoreText.text = "Player Score: " + playerScore;

            // Disable the button when there are no rolls left
            if (rollsLeft == 0)
            {
                playerRollButton.gameObject.SetActive(false); // Hide the roll button when player runs out of rolls
            }
        }
    }
}
