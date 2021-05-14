using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleWave : MonoBehaviour
{
    public int difficulty; //how many rows of blocks to delete
    public int maxDifficulty = 3;
    public float waveCooldown = 10f;
    public int waveHeight = 3;
    public float timeBetweenDifficulties = 30f; //in seconds
    private float wTimer; //wave timer
    private float dTimer; //difficulty timer
    public bool waveIsOver; //true if wave has passed, false if wave is currently passing through

    [SerializeField] private AudioSource WaveIncomingSource;

    private GameObject waveTimerText;
    private GameObject difficultyText;

    [SerializeField] private LevelManager _levelManager;

    // Start is called before the first frame update
    void Start()
    {
        difficulty = 0;
        wTimer = 0f;
        dTimer = 0f;
        waveIsOver = true;

        waveTimerText = GameObject.Find("WaveTimerText");
        difficultyText = GameObject.Find("DifficultyLevel");
    }

    // Update is called once per frame
    void Update()
    {
        if (_levelManager.currentGameState == LevelManager.GameState.playing ||
            _levelManager.currentGameState == LevelManager.GameState.won)
        {
            //Adds 1 to difficulty if enough time has passed and difficulty isn't max
            wTimer += Time.deltaTime;
            dTimer += Time.deltaTime;
            if (wTimer > waveCooldown)
            {
                MakeWave(waveHeight);
                wTimer = 0f;
            }
            if (dTimer > timeBetweenDifficulties && difficulty < maxDifficulty)
            {
                difficulty++;
                dTimer = 0f;
            }
        }

        // Update wave timer text
        waveTimerText.GetComponent<UnityEngine.UI.Text>().text =
            "Wave Timer: " + wTimer.ToString("0") + " / " + waveCooldown.ToString("0");
        // Update difficulty text
        //difficultyText.GetComponent<UnityEngine.UI.Text>().text =
        //    "Difficulty: " + difficulty.ToString() + " / " + maxDifficulty.ToString();
    }

    void OnParticleCollision(GameObject other)
    {
        if (other.tag == "Tetronimo")
        {
            Block b = other.GetComponent<Block>();
            b.splashSource.Play();
            b.Delete(difficulty);
        }
        else if (other.name == "Endpoint")
            waveIsOver = true;
    }

    public void MakeWave()
    {
        MakeWave(1);
    }

    public void MakeWave(int height)
    {
        waveIsOver = false;
        GetComponent<Waves>().GenerateWave(new DisruptiveWave(1, 5, height));

        WaveIncomingSource.Play();
    }
}
