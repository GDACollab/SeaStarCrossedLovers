﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBlock : MonoBehaviour
{
    public List<GameObject> BlockList;

    public float horizontalSpeed = 10;
    public float fallSpeed = 5;
    public float dropForce = 300;
    public int startingDownwardForce = 150;
    public int spawnDelay = 1;

    private float blockGravity;
    private bool waitingForBlock = false;
    private bool canSpawnBlock = false; 

    private GameObject activeBlock;
    private Rigidbody2D activeRB;
    private BlockManager blockManager;


    private void Start()
    {

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
                blockManager.isActive = false;
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
            (!blockManager.isActive || blockManager.beingDeleted))
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
        var selectedBlock = BlockList[Random.Range(0, BlockList.Count)];

        // Spawn block and track components
        activeBlock = Instantiate(selectedBlock, spawnPosition, Quaternion.identity);
        activeRB = activeBlock.GetComponent<Rigidbody2D>();
        blockManager = activeBlock.GetComponent<BlockManager>();

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
            canSpawnBlock = false;
        }
        else
        {
            spawnNewBlock();
            canSpawnBlock = true;
        }
    }

}