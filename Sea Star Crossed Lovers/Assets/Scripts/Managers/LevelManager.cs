using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public float timeToWin;
    public float winTimerIncrement;
    public float winTimerCooldown;
    public float winDelay;

    public string defualtWinTimerMessage;
    public string defualtWinMessage;

    public bool transitionSceneOnWin = true;
    public string nextScene;

    public enum GameState { paused, playing, winTimer, won }
    public GameState currentGameState = GameState.paused;

    public Block activeBlock { get; set; }

    [Header("Need to be filled via inspector")]
    public Canvas winCanvas;
    public Text winText;
    [SerializeField] private ProgressBar progressBar;

    [Header("Retrieved on awake")]
    public SceneLoader activeLoader;
    public ObstacleManager obstacleManager;
    public BlockManager blockManager;
    public TowerGoalpoint goalpoint;
    public SpawnBlock spawnBlock;
    public BlockController blockController;
    public BlockQueue blockQueue;

    private bool WinTimerOnCooldown = false;

    private void Awake()
    {
        // Set target frame rate so that nothing weird is going on.
        Application.targetFrameRate = 60;
        winCanvas.enabled = false;

        // Getting all necessary references
        activeLoader = (SceneLoader)FindObjectOfType(typeof(SceneLoader));

        blockManager = gameObject.GetComponent<BlockManager>();
        blockManager.Construct(this);

        obstacleManager = gameObject.GetComponent<ObstacleManager>();
        obstacleManager.Construct(this);

        goalpoint = (TowerGoalpoint)FindObjectOfType(typeof(TowerGoalpoint));
        goalpoint.Construct(blockManager);

        spawnBlock = (SpawnBlock)FindObjectOfType(typeof(SpawnBlock));
        spawnBlock.Construct(this, blockManager);

        blockController = (BlockController)FindObjectOfType(typeof(BlockController));
        blockController.Construct(spawnBlock, obstacleManager);

        blockQueue = (BlockQueue)FindObjectOfType(typeof(BlockQueue));
        blockQueue.Construct(this, blockManager.spriteList);

        winText.text = defualtWinTimerMessage;
    }

    private void Start() {
        progressBar.Construct(blockManager, goalpoint.getGoalHeight());
    }

    private void Update()
    {
        progressBar.updateProgress();

        if(currentGameState == GameState.playing &&
            goalpoint.CheckWin() && !WinTimerOnCooldown)
        {

            // New blocks should not spawn while the win timer is active:
            spawnBlock.toggleBlockSpawn();
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
                // reset text, exit win timer, hide canvas and start cooldown.
                winCanvas.enabled = false;
                winText.text = defualtWinTimerMessage;
                StartCoroutine(WinTimerCooldown());
                currentGameState = GameState.playing;
                // Also, make sure blocks can spawn again:
                spawnBlock.toggleBlockSpawn();
                spawnBlock.delaySpawnBlock();
                yield break;
            }
        }

        // If remain elligibile for win the whole time, play wins
        Debug.Log("You win");
        winText.text = defualtWinMessage;
        currentGameState = GameState.won;

        // Wait for a couple seconds before message disappears
        yield return new WaitForSeconds(winDelay);
        winCanvas.enabled = false;

        // TODO need to use unity events more; should have one here

        if (transitionSceneOnWin)
        {
            activeLoader.QuickFadeOutLoad(nextScene);
        }
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
