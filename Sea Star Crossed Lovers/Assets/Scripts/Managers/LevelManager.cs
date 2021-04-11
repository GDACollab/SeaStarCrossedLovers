using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public float timeToWin;
    public float winTimerIncrement;
    public float winTimerCooldown;

    private bool WinTimerOnCooldown = false;

    public string defualtWinTimerMessage;

    public enum GameState { paused, playing, winTimer, won }
    public GameState currentGameState = GameState.paused;

    public Text winText;

    private BlockManager _blockManager;
    private TowerGoalpoint _goalpoint;

    private void Awake()
    {
        // Getting all necessary references
        _blockManager = gameObject.GetComponent<BlockManager>();
        _blockManager.Construct(this);

        _goalpoint = (TowerGoalpoint)FindObjectOfType(typeof(TowerGoalpoint));
        _goalpoint.Construct(_blockManager);

        SpawnBlock spawnBlock = (SpawnBlock)FindObjectOfType(typeof(SpawnBlock));
        spawnBlock.Construct(this, _blockManager);

        winText.text = defualtWinTimerMessage;
    }

    private void Update()
    {
        if(currentGameState == GameState.playing &&
            _goalpoint.CheckWin() && !WinTimerOnCooldown)
        {
            // In case the winTimer is already running
            // (theoretically not possible?) stop it so it doesn't run twice
            StopCoroutine(winTimer());
            currentGameState = GameState.winTimer;
            StartCoroutine(winTimer());
        }
    }

    private IEnumerator winTimer()
    {
        winText.text = "Win timer: " + timeToWin;

        float currWaitTime = 0;
        // Continuously check if the goalpoint detects the player
        // is still elligible to win until the win timer reaches timeToWin
        while (currWaitTime < timeToWin)
        {
            if (_goalpoint.CheckWin())
            {
                // Update text
                float timeLeft = timeToWin - currWaitTime;
                // Display timeLeft float with 1 decimal place
                winText.text = "Win timer: " + timeLeft.ToString("F1");
                // Wait increment amount of time for next update
                currWaitTime += winTimerIncrement;
                yield return new WaitForSeconds(winTimerIncrement);
            }
            else
            {
                // If ever the player loses elligibility to win during
                // the timer, reset text, exit win timer, and start cooldown
                winText.text = defualtWinTimerMessage;
                StartCoroutine(WinTimerCooldown());
                currentGameState = GameState.playing;
                yield break;
            }
        }

        // If remain elligibile for win the whole time, play wins
        Debug.Log("You win");
        winText.text = "YOU WIN! You may continue playing.";
        currentGameState = GameState.won;
    }

    // Used to stop Update function from starting winTimer in finicky situations
    // where player is elligible and then unelligible in a short amount of time
    private IEnumerator WinTimerCooldown()
    {
        WinTimerOnCooldown = true;
        yield return new WaitForSeconds(winTimerCooldown);
        WinTimerOnCooldown = false;
    }
}
