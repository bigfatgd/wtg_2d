using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BossBattleManager : MonoBehaviour
{
    public SlotMachine slotMachine;
    public TMP_Text battleResultText;
    public Button playerRollButton;
    public TMP_Text playerScoreText;
    public TMP_Text bossScoreText;
    public Camera mainCamera;
    public TimerToMoveCameraPosition timerManager;

    [Header("Boss Multiplier Settings")]
    public float baseBossMultiplier = 1.0f;              // Starting multiplier
    public float multiplierIncreasePerRound = 0.5f;      // Increase after each battle
    private float currentBossMultiplier = 1.0f;

    private int playerScore = 0;
    private int bossScore = 0;
    private int rollsLeft = 5;
    private bool isBattleActive = false;
    private int currentBattleRound = 0;

    private TimerToMoveCameraPosition timerScript;

    void Start()
    {
        timerScript = FindObjectOfType<TimerToMoveCameraPosition>();

        playerRollButton.gameObject.SetActive(true);
        playerRollButton.onClick.AddListener(PlayerRoll);

        battleResultText.text = "";
        playerScoreText.text = "Player Score: 0";
        bossScoreText.text = "Boss Score: 0";

        currentBossMultiplier = baseBossMultiplier;
        StartBattle(); // Start the first round immediately
    }

    public void StartBattle()
    {
        if (isBattleActive)
        {
            Debug.Log("StartBattle() blocked, battle already active");
            return;
        }

        isBattleActive = true;

        currentBattleRound++;
        currentBossMultiplier = baseBossMultiplier + (multiplierIncreasePerRound * (currentBattleRound - 1));
        Debug.Log($"[ROUND {currentBattleRound}] Boss multiplier: {currentBossMultiplier}");

        battleResultText.text = " ";
        playerScoreText.text = "Player Score: 0";
        bossScoreText.text = "Boss Score: 0";
    }

    public void ResetBattle()
    {
        Debug.Log("ResetBattle() called");

        playerScore = 0;
        bossScore = 0;
        rollsLeft = 5;
        isBattleActive = false;

        slotMachine.ResetBossEffects();

        battleResultText.text = "";
        playerScoreText.text = "Player Score: 0";
        bossScoreText.text = "Boss Score: 0";

        playerRollButton.gameObject.SetActive(true);
    }

    void PlayerRoll()
    {
        if (rollsLeft > 0)
        {
            Debug.Log("Player clicked roll");

            StartCoroutine(GetPlayerScore());
            rollsLeft--;

            if (rollsLeft == 0)
            {
                playerRollButton.gameObject.SetActive(false); // Disable button during boss phase
                StartCoroutine(BossTurn());
            }
        }
    }

    // Coroutine to get player score after roll
    IEnumerator GetPlayerScore()
    {
        playerRollButton.interactable = false; // Disable the button during player roll
        yield return StartCoroutine(slotMachine.RollAndGetScore(false, true)); // Player roll in boss phase
        int scoreFromRoll = slotMachine.GetScore();
        playerScore += scoreFromRoll;
        playerScoreText.text = "Player Score: " + playerScore;
        yield return new WaitForSeconds(slotMachine.rollCooldown); // Wait for the cooldown before enabling the button again
        playerRollButton.interactable = true; // Re-enable the button after the cooldown
    }

    // Coroutine for boss turn
    IEnumerator BossTurn()
    {
        int bossRolls = 5;
        slotMachine.ResetBossEffects();

        while (bossRolls > 0)
        {
            yield return StartCoroutine(slotMachine.RollAndGetScore(bossRoll: true, isPlayerInBoss: false));
            int baseScore = slotMachine.GetScore();
            int multipliedScore = Mathf.RoundToInt(baseScore * currentBossMultiplier);
            bossScore += multipliedScore;

            bossScoreText.text = "Boss Score: " + bossScore;
            bossRolls--;

            // Wait for the cooldown between boss rolls (use the same delay as the player)
            yield return new WaitForSeconds(slotMachine.rollCooldown + 0.1f); // Extra 0.1f to simulate more animation time
        }

        EndBattle();
    }

    void EndBattle()
    {
        if (playerScore > bossScore)
        {
            battleResultText.text = "You Won!";
            Debug.Log("PLAYER WON");

            StartCoroutine(MoveCameraRight());

            if (timerScript != null)
            {
                timerScript.ResetTimer();
            }

            ResetBattle();
            StartCoroutine(DelayedStartNextBattle());
        }
        else
        {
            battleResultText.text = "You Lost!";
            Debug.Log("PLAYER LOST");

            StartCoroutine(MoveCameraLeft());
            isBattleActive = false;
        }
    }

    IEnumerator DelayedStartNextBattle()
    {
        yield return new WaitForSeconds(1f);
        StartBattle();
    }

    IEnumerator MoveCameraRight()
    {
        Vector3 newPos = mainCamera.transform.position + new Vector3(19f, 0, 0);
        mainCamera.transform.position = newPos;
        yield return null;
    }

    IEnumerator MoveCameraLeft()
    {
        Vector3 newPos = mainCamera.transform.position + new Vector3(-18f, 0, 0);
        mainCamera.transform.position = newPos;
        yield return null;
    }
}
