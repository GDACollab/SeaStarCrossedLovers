using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockQueue : MonoBehaviour
{
    public Image next_0Image;
    public Image next_1Image;
    public Image next_2Image;

    public Text next_0Text;
    public Text next_1Text;
    public Text next_2Text;

    public Image holdImage;
    public Text holdText;

    public List<BlockData> blockQueueList = new List<BlockData>(3);
    public BlockData holdBlock;

    private LevelManager manager;
    private SpawnBlock spawnBlock;

    private List<BlockData> SpawnBlockList;

    public void Construct(LevelManager manager)
    {
        this.manager = manager;
        spawnBlock = manager.spawnBlock;
        SpawnBlockList = spawnBlock.SpawnBlockList;

        // Fill the queue
        for(int i = 0; i < 3; i++)
        {
            EnqueueBlock();
        }
        UpdateBlockDisplay();
    }

    public void UpdateBlockDisplay()
    {
        next_0Text.text = blockQueueList[0].name;
        next_1Text.text = blockQueueList[1].name;
        next_2Text.text = blockQueueList[2].name;

        if (holdBlock)
        {
            holdText.text = holdBlock.name;
        }
        else
        {
            holdText.text = "None";
        }
    }

    public void EnqueueBlock()
    {
        BlockData newBlock = SpawnBlockList[Random.Range(0, SpawnBlockList.Count)];
        blockQueueList.Add(newBlock);
    }

    public BlockData DequeueBlock()
    {
        BlockData nextBlock = blockQueueList[0];
        blockQueueList.Remove(blockQueueList[0]);
        return nextBlock;
    }

    public void HoldBlock(BlockData activeBlockData)
    {
        // Set next block to block in hold if exits
        if(holdBlock)
        {
            blockQueueList[0] = holdBlock;
        }
        // Set hold block to activeBlock
        holdBlock = activeBlockData;
    }
}
