﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    public float maxSpeed;
    public float accelRate;

    public float baseFallSpeed;
    public float fastFallSpeed;

    public float prestableControlTime;
    public float prestableControlMult;

    private float currentFallSpeed;
    public bool canHold;

    private SpawnBlock spawnBlock;
    private BlockQueue blockQueue;

    public void Construct(SpawnBlock spawnBlock)
    {
        currentFallSpeed = baseFallSpeed;
        this.spawnBlock = spawnBlock;

        blockQueue = spawnBlock.blockQueue;
        canHold = true;
    }

    private void FixedUpdate()
    {
        // control an active block
        if (spawnBlock.activeBlock != null)
        {
            // apply horizontal and vertical change
            float sidewaysVelocity = Input.GetAxis("Horizontal")
                * Time.fixedDeltaTime * accelRate;
            // Prevent velocity going above maxSpeed
            sidewaysVelocity = Mathf.Min(sidewaysVelocity, maxSpeed);
            // Control block when in air
            if (spawnBlock.activeBlock.state == Block.BlockState.active)
            {
                spawnBlock.activeRB.velocity = new Vector2(sidewaysVelocity, -currentFallSpeed);
            }
            // TODO Change friction to very low while active/preStable?

            // After being colliding, give limited control
            else if (spawnBlock.activeBlock.state == Block.BlockState.preStable)
            {
                spawnBlock.activeRB.gravityScale = spawnBlock.blockGravity;
                Vector2 force = new Vector2(sidewaysVelocity * prestableControlMult, 0);
                spawnBlock.activeRB.AddForce(force);
            }
        }
    }

    private void Update()
    {
        if (spawnBlock.activeBlock != null)
        {
            // detect rotation
            if (Input.GetButtonDown("RotateCounterclockwise"))
            {
                spawnBlock.activeRB.transform.Rotate(new Vector3(0, 0, 90));
            }
            else if (Input.GetButtonDown("RotateClockwise"))
            {
                spawnBlock.activeRB.transform.Rotate(new Vector3(0, 0, -90));
            }

            // detect if player has dropped block
            if (Input.GetButton("Drop"))
            {
                //Implementation of block fall acceleration instead of activating physics
                currentFallSpeed = fastFallSpeed;
            }
            else
            {
                currentFallSpeed = baseFallSpeed;
            }

            if (Input.GetButtonDown("Hold") && canHold)
            {
                // Cannot hold until new activeBlock passes spawnBlock update check for spawning new block
                canHold = false;

                blockQueue.HoldBlock(spawnBlock.activeBlock.data);
                blockQueue.UpdateBlockDisplay();
                // Destroy activeBlock and spawn the new block
                Destroy(spawnBlock.activeBlock.gameObject);
                StartCoroutine(spawnBlock.delaySpawnBlock());
            }
        }
    }
}