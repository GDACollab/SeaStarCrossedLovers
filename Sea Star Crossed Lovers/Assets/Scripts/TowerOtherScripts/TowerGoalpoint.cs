using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerGoalpoint : MonoBehaviour
{
    private BlockManager _blockManager;
    private LevelManager _levelManager;
    private float goalHeight;

    private bool stableBlockInTrigger;

    private List<Block> winningBlocks = new List<Block>();

    public void Construct(BlockManager blockManager)
    {
        _blockManager = blockManager;
        _levelManager = _blockManager._levelManager;
    }

    private void Awake()
    {
        goalHeight = gameObject.transform.position.y;
    }

    public bool CheckWin()
    {
        // Elligible for win if has winnning blocks
        return winningBlocks.Count > 0;
    }

    // Checks elligibility of towers blocks to be considered winning
    // blocks every frame
    void OnTriggerStay2D(Collider2D collision)
    {
        // Check if blocklet is collision
        var blockletCheck = collision.gameObject.GetComponent<Blocklet>();
        if (!blockletCheck) return;
        // If blocklet is found, check if Block is not active and is stable
        Block block = blockletCheck.blockParent;
        if (blockletCheck != _blockManager.activeBlock &&
            block.currentState == Block.BlockState.stable)
        {
            // Add to winningBlocks if passed 
            if (!winningBlocks.Contains(block))
            {
                winningBlocks.Add(block);
            }
        }
    }

    // Checks if blocks exit the trigger and remove their elligibility if so
    void OnTriggerExit2D(Collider2D collision)
    {
        // Check if blocklet is collision
        var blockletCheck = collision.gameObject.GetComponent<Blocklet>();
        if (!blockletCheck) return;
        // If blocklet is found, remove block from winningBlocks
        Block block = blockletCheck.blockParent;
        if (block) winningBlocks.Remove(block);
    }
}
