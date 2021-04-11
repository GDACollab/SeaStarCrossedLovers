using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBlock : MonoBehaviour
{
    public List<BlockData> SpawnBlockList;
    public List<AudioClip> BlockHitSounds;

    public float blockHitVolume = 1;

    public float blockMass = 1;

    public float horizontalSpeed = 2000;
    public float baseFallSpeed = 5;
    public float dropForce = 300;
    public int startingDownwardForce = 150;
    public int spawnDelay = 1;

    private float currentFallSpeed;
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
        foreach (BlockData block in SpawnBlockList)
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
        currentFallSpeed = baseFallSpeed;
    }

    private void FixedUpdate()
    {
        // control an active block
        if (activeBlock != null)
        {
            // apply horizontal and vertical change
            float xOffset = Input.GetAxisRaw("Horizontal") * Time.deltaTime * horizontalSpeed;
            activeRB.velocity = new Vector2(xOffset, -currentFallSpeed);

            // detect if player has dropped block
            if (Input.GetButton("Drop"))
            {

                //Implementation of block fall acceleration instead of activating physics
                currentFallSpeed = baseFallSpeed * 4;
            }
            else
            {
                currentFallSpeed = baseFallSpeed;
            }
        }
    }

    private void Update()
    {
        // if an active block collides or is being deleted,
        // give it normal gravity and prepare to spawn new block
        if (canSpawnBlock && !waitingForBlock &&
            (activeBlock.currentState == Block.BlockState.stable ||
            activeBlock.currentState == Block.BlockState.deleting))
        {
            // Set true to ensure no additional block is spawned during the spawn delay
            waitingForBlock = true;
            // Reset activeBlock to original gravity and null activeBlock
            activeRB.gravityScale = blockGravity;
            activeBlock = null;
            activeRB.velocity = new Vector2(0, 0);

            StartCoroutine(delaySpawnBlock());
        }

        // detect rotation
        if (activeBlock != null)
        {
            if (Input.GetButtonDown("RotateCounterclockwise"))
            {
                activeRB.transform.Rotate(new Vector3(0, 0, 90));
            }
            else if (Input.GetButtonDown("RotateClockwise"))
            {
                activeRB.transform.Rotate(new Vector3(0, 0, -90));
            }
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

        // Random select BlockData from SpawnBlockList
        BlockData selectedBlockData = SpawnBlockList[Random.Range(0, SpawnBlockList.Count)];

        if (!selectedBlockData.blockPrefab)
        {
            Debug.LogError("BlockData \"" + selectedBlockData.name + "\" has null blockPrefab");
        }
        Block newBlock = Instantiate(selectedBlockData.blockPrefab, spawnPosition, Quaternion.identity);
        newBlock.Construct(_blockManager, selectedBlockData, spawnPosition);

        // Set active block to be controlled
        activeBlock = newBlock;
        activeRB = activeBlock.GetComponent<Rigidbody2D>();
        activeBlock = activeBlock.GetComponent<Block>();

        // Give random collision sound
        activeBlock.audioSource.clip = BlockHitSounds[Random.Range(0, BlockHitSounds.Count)];
        activeBlock.audioSource.volume = blockHitVolume;

        activeRB.mass = blockMass;

        // Pass active block to level manager and block manager
        _blockManager.activeBlock = activeBlock;
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
