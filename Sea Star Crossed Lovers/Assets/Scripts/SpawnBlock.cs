﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBlock : MonoBehaviour
{
    public List<Block> SpawnBlockList;

    public float horizontalSpeed = 10;
    public float fallSpeed = 5;
    public float dropForce = 300;
    public int startingDownwardForce = 150;
    public int spawnDelay = 1;

    private float blockGravity;
    private bool waitingForBlock = false;
    private bool canSpawnBlock = false; 

    private Block activeBlock;
    private Rigidbody2D activeRB;

    private LevelManager _levelManager;
    private BlockManager _blockManager;

    public void Construct(LevelManager levelManager, BlockManager blockManager)
    {
        _levelManager = levelManager;
        _blockManager = blockManager;
    }

    private void Awake()
    {
        foreach (Block block in SpawnBlockList)
        {
            if (block == null)
            {
                Debug.LogError("SpawnBlockList Error: null Block");
            }
        }
        if (SpawnBlockList.Count == 0)
        {
            Debug.LogError("SpawnBlockList Error: empty list");
        }
    }

    private void Update()
    {

        // control an active block
        if (activeBlock != null) {

            // apply horizontal and vertical change
            float xOffset = Input.GetAxisRaw("Horizontal")*Time.deltaTime*horizontalSpeed;
            activeBlock.transform.position += new Vector3(xOffset, -fallSpeed*Time.deltaTime, 0);
        
            // detect if player has dropped block
            if (Input.GetButtonDown("Drop")) {
                // apply a starting force to make drop feel more natural (momentum)
                // give block normal gravity
                activeRB.AddForce(new Vector3(0, -dropForce, 0));
                activeRB.gravityScale = blockGravity;
                activeBlock.currentState = Block.BlockState.falling;
            }

            // detect rotation
            if (Input.GetButtonDown("RotateCounterclockwise")) {
                activeRB.transform.Rotate(new Vector3(0, 0, 90));
            } else if (Input.GetButtonDown("RotateClockwise")) {
                activeRB.transform.Rotate(new Vector3(0, 0, -90));
            }
        }

        // if an active block collides or is being deleted,
        // give it normal gravity and prepare to spawn new block
        if (canSpawnBlock && !waitingForBlock &&
            (activeBlock.currentState == Block.BlockState.stable || activeBlock.currentState == Block.BlockState.deleting))
        {
            // Set true to ensure no additional block is spawned during the spawn delay
            waitingForBlock = true;
            // Reset activeBlock to original gravity and null activeBlock
            activeRB.gravityScale = blockGravity;
            activeBlock = null;

            StartCoroutine(delaySpawnBlock());
        }
    }

    IEnumerator delaySpawnBlock()
    {
        yield return new WaitForSeconds(spawnDelay);
        waitingForBlock = false;
        spawnNewBlock();
    }

    // Spawn random block from blockList at parent gameObject position
    private void spawnNewBlock()
    {
        // Spawn on parent object position
        Vector3 spawnPosition = gameObject.transform.position;
        spawnPosition.z = 0;

        // Random select block from BlockList
        Block selectedBlock = SpawnBlockList[Random.Range(0, SpawnBlockList.Count)];

        // Spawn block and track components
        activeBlock = Instantiate(selectedBlock, spawnPosition, Quaternion.identity);
        activeRB = activeBlock.GetComponent<Rigidbody2D>();
        activeBlock = activeBlock.GetComponent<Block>();

        // Pass active block to level manager and block manager
        _levelManager.activeBlock = activeBlock;
        _blockManager.AddBlockFromList(activeBlock);

        // Store original gravity
        blockGravity = activeRB.gravityScale;
        // Disable active block gravity
        activeRB.gravityScale = 0f;
    }

    public void toggleBlockSpawn()
    {
        if (activeRB) {
            activeRB.gravityScale = blockGravity;
            activeBlock = null;
        }
        
        if (canSpawnBlock)
        {
            _levelManager.currentGameState =
                LevelManager.GameState.paused;
            canSpawnBlock = false;
        }
        else
        {
            _levelManager.currentGameState =
                LevelManager.GameState.playing;
            spawnNewBlock();
            canSpawnBlock = true;
        }
    }

}