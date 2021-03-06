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

    public float gravity;

    private SpawnBlock spawnBlock;
    private BlockQueue blockQueue;
    private ObstacleManager obstacleManager;

    public void Construct(SpawnBlock spawnBlock, ObstacleManager obstacleManager)
    {
        this.obstacleManager = obstacleManager;
        this.spawnBlock = spawnBlock;
        blockQueue = spawnBlock.blockQueue;

        currentFallSpeed = baseFallSpeed;
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

            if (Input.GetButton("Focus"))
            {
                sidewaysVelocity = Input.GetAxis("Horizontal")
                * Time.fixedDeltaTime * accelRate/3;

                // Activate focus particles
                spawnBlock.activeBlock.focusParticles.Play();
            }
            else
            {
                // Deactivate focus particles
                spawnBlock.activeBlock.focusParticles.Stop();
            }

            // Prevent velocity going above maxSpeed
            sidewaysVelocity = Mathf.Min(sidewaysVelocity, maxSpeed);

            // Control block when in air
            if (spawnBlock.activeBlock.state == Block.BlockState.active)
            {
                Vector2 playerMovement = new Vector2(sidewaysVelocity, -currentFallSpeed);

                // Add wind velocity if active
                if (obstacleManager.windIsActive)
                {
                    spawnBlock.activeRB.velocity = playerMovement +
                        new Vector2(obstacleManager.windDirection * obstacleManager.windVelocity, 0);
                }
                else
                {
                    spawnBlock.activeRB.velocity = playerMovement;
                }
            }
            // TODO Change friction to very low while active/preStable?

            // After colliding, give limited control
            else if (spawnBlock.activeBlock.state == Block.BlockState.preStable)
            {
                Vector2 force = new Vector2(sidewaysVelocity * prestableControlMult, 0);
                spawnBlock.activeRB.AddForce(force);
            }

            // Clamp horizontal movement
            spawnBlock.activeBlock.transform.position = new Vector2(Mathf.Clamp(spawnBlock.activeBlock.transform.position.x, -8f, 8f), spawnBlock.activeBlock.transform.position.y);
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
