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

    // Start is called before the first frame update
    void Start()
    {
        difficulty = 1;
        wTimer = 0f;
        dTimer = 0f;
        waveIsOver = true;
    }

    // Update is called once per frame
    void Update()
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

    void OnParticleCollision(GameObject other)
    {
        if (other.tag == "Blocklet")
        {
            //    Blocklet b = other.GetComponent<Blocklet>();
            //    b.Delete(difficulty);
        }
        else if (other.name == "Endpoint")
            waveIsOver = true;
    }

    public void MakeWave()
    {
        MakeWave(1);
    }

    public void MakeWave(int height) {
        waveIsOver = false;
        for(int i = 0; i < height; i++)
            GetComponent<Waves>().GenerateWave(new DisruptiveWave());
    }
}
