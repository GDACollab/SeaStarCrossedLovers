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

    public Canvas winCanvas;
    public Text winText;

    public BlockManager blockManager;
    public TowerGoalpoint goalpoint;
    public SpawnBlock spawnBlock;
    public BlockController blockController;
    public BlockQueue blockQueue;
    [SerializeField] private ProgressBar progressBar;

    public Block activeBlock { get; set; }

    private void Awake()
    {
        winCanvas.enabled = false;

        // Getting all necessary references
        blockManager = gameObject.GetComponent<BlockManager>();
        blockManager.Construct(this);

        goalpoint = (TowerGoalpoint)FindObjectOfType(typeof(TowerGoalpoint));
        goalpoint.Construct(blockManager);

        spawnBlock = (SpawnBlock)FindObjectOfType(typeof(SpawnBlock));
        spawnBlock.Construct(this, blockManager);

        blockController = (BlockController)FindObjectOfType(typeof(BlockController));
        blockController.Construct(spawnBlock);

        blockQueue = (BlockQueue)FindObjectOfType(typeof(BlockQueue));
        blockQueue.Construct(this);
        progressBar.Construct(blockManager, goalpoint.getGoalHeight());

        winText.text = defualtWinTimerMessage;
    }

    private void Update()
    {
        progressBar.updateProgress();
        if(currentGameState == GameState.playing &&
            goalpoint.CheckWin() && !WinTimerOnCooldown)
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

        winCanvas.enabled = true;

        // Continuously check if the goalpoint detects the player
        // is still elligible to win until the win timer reaches timeToWin
        while (currWaitTime < timeToWin)
        {
            if (goalpoint.CheckWin())
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
                // If ever the player loses elligibility to win during the timer,
                // reset text, exit win timer, hide canvas and start cooldown
                winCanvas.enabled = false;
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

        // Wait for a couple seconds before message disappears
        yield return new WaitForSeconds(5);
        winCanvas.enabled = false;
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
