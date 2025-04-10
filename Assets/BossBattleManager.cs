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
    public TimerToMoveCameraPosition timerManager; // przypisz w Inspectorze

    private int playerScore = 0;
    private int bossScore = 0;
    private int rollsLeft = 5;
    private bool isBattleActive = false;

    private TimerToMoveCameraPosition timerScript;

    void Start()
    {
        timerScript = FindObjectOfType<TimerToMoveCameraPosition>();

        playerRollButton.gameObject.SetActive(true);
        playerRollButton.onClick.AddListener(PlayerRoll);

        battleResultText.text = "";
        playerScoreText.text = "Player Score: 0";
        bossScoreText.text = "Boss Score: 0";
    }

    public void StartBattle()
{
    if (isBattleActive)
    {
        Debug.Log("StartBattle() blocked, battle already active");
        return;
    }

    isBattleActive = true;

    battleResultText.text = "Battle Started!";
    playerScoreText.text = "Player Score: 0";
    bossScoreText.text = "Boss Score: 0";

    Debug.Log("Battle STARTED");
}


public void ResetBattle()
{
    Debug.Log("ResetBattle() called");

    playerScore = 0;
    bossScore = 0;
    rollsLeft = 5;
    isBattleActive = false;

    slotMachine.ResetBossEffects(); // Jeśli masz jakieś power-upy u bossa, resetuj

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
          playerRollButton.gameObject.SetActive(false);
          StartCoroutine(BossTurn());
        }

        }
    }

    IEnumerator GetPlayerScore()
    {
        yield return StartCoroutine(slotMachine.RollAndGetScore());

        int scoreFromRoll = slotMachine.GetScore();
        playerScore += scoreFromRoll;
        playerScoreText.text = "Player Score: " + playerScore;
    }

    IEnumerator BossTurn()
    {
        int bossRolls = 5;
        slotMachine.ResetBossEffects(); // Reset power-upów

        while (bossRolls > 0)
        {
            yield return StartCoroutine(slotMachine.RollAndGetScore());

            int scoreFromBossRoll = slotMachine.GetScore();
            bossScore += scoreFromBossRoll;
            bossScoreText.text = "Boss Score: " + bossScore;

            bossRolls--;
            yield return new WaitForSeconds(1f);
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

        ResetBattle(); // <-- resetujemy grę po wygranej

        slotMachine.ResetBossEffects(); // <-- resetujemy tylko power-upy bossa
    }
    else
    {
        battleResultText.text = "You Lost!";
        Debug.Log("PLAYER LOST");

        StartCoroutine(MoveCameraLeft());
        isBattleActive = false;
    }
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
