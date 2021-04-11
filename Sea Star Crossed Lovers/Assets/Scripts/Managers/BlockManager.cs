using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public List<Block> SceneBlockList;

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
            highestBlockHeight = highestBlock.transform.position.y;
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
                block.currentState == Block.BlockState.stable &&
                block.transform.position.y > highestBlockHeight)
            {
                resultBlock = block;
            }
        });

        return resultBlock;
    }
}
