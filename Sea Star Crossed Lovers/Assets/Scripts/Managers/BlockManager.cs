using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public List<Block> SceneBlockList;


    [Tooltip("The sprites of the blocks to used when showing upcoming blocks in the block queue. The main process of sprite setup is done by BlockQueue.cs.")]
    public List<Sprite> spriteList;

    public Block highestBlock;
    public float highestBlockHeight = 0;

    public LevelManager _levelManager;

    public Block activeBlock { get; set; }

    public void Construct(LevelManager levelManager)
    {
        _levelManager = levelManager;
    }

    private void Update()
    {
        highestBlock = GetHighestTowerBlock();
        if(highestBlock)
        {
            highestBlockHeight = getBlockHeight(highestBlock);
        }
    }

    public void AddBlockFromList(Block toAdd)
    {
        SceneBlockList.Add(toAdd);
    }

    public void RemoveBlockFromList(Block toRemove)
    {
        Destroy(toRemove.gameObject);
        SceneBlockList.Remove(toRemove);
    }

    public Block GetHighestTowerBlock()
    {
        Block resultBlock = highestBlock;
        SceneBlockList.ForEach(block =>
        {
            if (block != activeBlock &&
                block.state == Block.BlockState.stable &&
                getBlockHeight(block) > highestBlockHeight)
            {
                resultBlock = block;
            }
        });

        return resultBlock;
    }

    // Returns the maximum height of the block parameter, as defined by the bounds of the collider.
    private float getBlockHeight(Block block)
    {
        return block.GetComponent<Collider2D>().bounds.max.y;
    }
}
