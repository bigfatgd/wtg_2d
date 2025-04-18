using UnityEngine;
using TMPro;

public class TimerToMoveCameraPosition : MonoBehaviour
{
    public float timeRemaining = 60f;
    public TextMeshProUGUI timerText1;
    public TextMeshProUGUI timerText2;

    public float posX = 0f;
    public float posY = 0f;
    public Camera mainCamera;

    public BossBattleManager bossBattleManager;

    private bool timerIsRunning = true;
    private bool cameraMoved = false;

    void Update()
{
    if (timerIsRunning)
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerDisplay(timeRemaining);
        }
        else
        {
            timeRemaining = 0;
            timerIsRunning = false;

            if (!cameraMoved)
            {
                Debug.Log("Timer finished - moving camera and starting battle");

                MoveCameraToPosition(posX, posY);
                cameraMoved = true;

                if (bossBattleManager != null)
                {
                    bossBattleManager.StartBattle();
                    Debug.Log("StartBattle() called from Timer");
                }
            }
        }
    }
}



    void UpdateTimerDisplay(float timeToDisplay)
    {
        timeToDisplay += 1;
        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);
        string formattedTime = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (timerText1 != null) timerText1.text = formattedTime;
        if (timerText2 != null) timerText2.text = formattedTime;
    }

    public void MoveCameraToPosition(float x, float y)
    {
        if (mainCamera != null)
        {
            Vector3 currentPos = mainCamera.transform.position;
            mainCamera.transform.position = new Vector3(x, y, currentPos.z);
        }
    }

    public void ResetAndRestartCycle()
    {
        // Kamera przesuwa się w prawo o 5 jednostek (możesz zmienić)
        if (mainCamera != null)
        {
            mainCamera.transform.position += new Vector3(5f, 0f, 0f);
        }

        timeRemaining = 60f;
        timerIsRunning = true;
        cameraMoved = false;

        if (bossBattleManager != null)
            bossBattleManager.StartBattle();
    }
    

   public void ResetTimer()
{
    timeRemaining = 60f;
    timerIsRunning = true;
    cameraMoved = false; // <- UPEWNIJ SIĘ, ŻE TO TU JEST
    Debug.Log("ResetTimer called: cameraMoved reset to FALSE");
}


}
