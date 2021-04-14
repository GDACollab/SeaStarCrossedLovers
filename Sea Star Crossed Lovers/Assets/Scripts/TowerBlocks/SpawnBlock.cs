using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBlock : MonoBehaviour
{
    public List<BlockData> SpawnBlockList;
    public List<AudioClip> BlockHitSounds;

    public float blockHitVolume = 1;
    public float blockMass = 1;
    public int spawnDelay = 1;

    private bool waitingForBlock = false;
    private bool canSpawnBlock = false;

    public float blockGravity;

    public Block activeBlock;
    public Rigidbody2D activeRB;

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
    }

    private void Update()
    {
        // if an active block collides or is being deleted,
        // give it normal gravity and prepare to spawn new block
        if (canSpawnBlock && !waitingForBlock &&
            (activeBlock.state == Block.BlockState.stable ||
            activeBlock.state == Block.BlockState.deleting))
        {
            // Set true to ensure no additional block is spawned during the spawn delay
            waitingForBlock = true;
            // Reset activeBlock to original gravity and null activeBlock
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

        // Random select BlockData from SpawnBlockList
        BlockData selectedBlockData = SpawnBlockList[Random.Range(0, SpawnBlockList.Count)];

        if (!selectedBlockData.blockPrefab)
        {
            Debug.LogError("BlockData \"" + selectedBlockData.name + "\" has null blockPrefab");
        }
        Block newBlock = Instantiate(selectedBlockData.blockPrefab, spawnPosition, Quaternion.identity);
        newBlock.Construct(_blockManager, _levelManager.blockController, selectedBlockData, spawnPosition);

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
