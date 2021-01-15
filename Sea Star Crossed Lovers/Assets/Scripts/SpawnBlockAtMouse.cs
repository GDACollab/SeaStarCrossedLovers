using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBlockAtMouse : MonoBehaviour
{
    public GameObject Block;
    private Camera Camera;

    private void Start()
    {
        Camera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Left mouse button clicked");

            Vector3 spawnPosition = Camera.ScreenToWorldPoint(Input.mousePosition);
            spawnPosition.z = 0;

            Instantiate(Block, spawnPosition, Quaternion.identity);

            Debug.Log("Block spawnPosition: " + spawnPosition);
        }
    }
}
