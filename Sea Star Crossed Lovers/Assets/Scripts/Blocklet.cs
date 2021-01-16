using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocklet : MonoBehaviour
{
    public const int STARTING_HEALTH = 100;
    [Range(1, STARTING_HEALTH)]
    public int health;
    // Start is called before the first frame update
    void Start()
    {
        health = STARTING_HEALTH;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
