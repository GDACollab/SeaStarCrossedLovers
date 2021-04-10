﻿using System.Collections;
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
    [SerializeField] private ProgressBar _progressBar;

    public Block activeBlock { get; set; }

    private void Awake()
    {
        _blockManager = gameObject.GetComponent<BlockManager>();
        _blockManager.Construct(this);

        _goalpoint = (TowerGoalpoint)FindObjectOfType(typeof(TowerGoalpoint));
        _goalpoint.Construct(_blockManager);

        _progressBar.Construct(_blockManager, _goalpoint.getGoalHeight());

        SpawnBlock spawnBlock = (SpawnBlock)FindObjectOfType(typeof(SpawnBlock));
        spawnBlock.Construct(this, _blockManager);

        winText.text = defualtWinTimerMessage;
    }

    private void Update()
    {
        _progressBar.updateProgress();
        if(currentGameState == GameState.playing &&
            _goalpoint.CheckWin() && !WinTimerOnCooldown)
        {
            StopCoroutine(winTimer());
            currentGameState = GameState.winTimer;
            StartCoroutine(winTimer());
        }
    }

    private IEnumerator winTimer()
    {
        winText.text = "Win timer: " + timeToWin;

        float currWaitTime = 0;
        while (currWaitTime < timeToWin)
        {
            if (_goalpoint.CheckWin())
            {
                float timeLeft = timeToWin - currWaitTime;
                winText.text = "Win timer: " + timeLeft.ToString("F1");
                currWaitTime += winTimerIncrement;
                yield return new WaitForSeconds(winTimerIncrement);
            }
            else
            {
                winText.text = defualtWinTimerMessage;
                StartCoroutine(WinTimerCooldown());
                currentGameState = GameState.playing;
                yield break;
            }
        }

        Debug.Log("You win");
        winText.text = "YOU WIN! You may continue playing.";
        currentGameState = GameState.won;
    }

    private IEnumerator WinTimerCooldown()
    {
        WinTimerOnCooldown = true;
        yield return new WaitForSeconds(winTimerCooldown);
        WinTimerOnCooldown = false;
    }

    public void ChangeScene(string SceneName)
    {
        // TODO insert some scene transition here

        SceneManager.LoadScene(SceneName);
    }
}
