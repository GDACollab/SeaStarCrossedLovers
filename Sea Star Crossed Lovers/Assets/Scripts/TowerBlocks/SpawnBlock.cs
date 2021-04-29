using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBlock : MonoBehaviour
{
    public List<BlockData> SpawnBlockList;
    public List<AudioClip> BlockHitSounds;

    public float blockHitVolume = 1;
    public float blockMass = 1;
    public float spawnDelay = 1;

    private bool waitingForBlock = false;
    private bool canSpawnBlock = false;

    [HideInInspector] public float blockGravity;

    [HideInInspector] public Block activeBlock;
    [HideInInspector] public Rigidbody2D activeRB;

    private LevelManager levelManager;
    private BlockManager blockManager;
    public BlockQueue blockQueue;

    public void Construct(LevelManager levelManager, BlockManager blockManager)
    {
        this.levelManager = levelManager;
        this.blockManager = blockManager;

        blockQueue = GetComponent<BlockQueue>();
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
            // Can only hold again when reach these conditions
            this.levelManager.blockController.canHold = true;
            StartCoroutine(delaySpawnBlock());
        }
    }

    public IEnumerator delaySpawnBlock()
    {
        // Set true to ensure no additional block is spawned during the spawn delay
        waitingForBlock = true;
        // Reset activeBlock to original gravity and null activeBlock
        activeBlock = null;

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

        BlockData selectedBlockData = blockQueue.DequeueBlock();
        blockQueue.EnqueueBlock();
        blockQueue.UpdateBlockDisplay();

        if (!selectedBlockData.blockPrefab)
        {
            Debug.LogError("BlockData \"" + selectedBlockData.name + "\" has null blockPrefab");
        }
        Block newBlock = Instantiate(selectedBlockData.blockPrefab, spawnPosition, Quaternion.identity);
        newBlock.Construct(blockManager, levelManager.blockController, selectedBlockData, spawnPosition);

        // Set active block to be controlled
        activeBlock = newBlock;
        activeRB = activeBlock.GetComponent<Rigidbody2D>();
        activeBlock = activeBlock.GetComponent<Block>();

        // Give random collision sound
        activeBlock.audioSource.clip = BlockHitSounds[Random.Range(0, BlockHitSounds.Count)];
        activeBlock.audioSource.volume = blockHitVolume;

        activeRB.mass = blockMass;

        // Pass active block to level manager and block manager
        blockManager.activeBlock = activeBlock;
        blockManager.AddBlockFromList(activeBlock);

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
            levelManager.currentGameState =
                LevelManager.GameState.paused;
            canSpawnBlock = false;
        }
        else
        {
            levelManager.currentGameState =
                LevelManager.GameState.playing;
            spawnNewBlock();
            canSpawnBlock = true;
        }
    }
}
