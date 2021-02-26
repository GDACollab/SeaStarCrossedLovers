using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustPosition : MonoBehaviour
{
    public Vector3 startPos;
    public bool gradual;
    public float maxDelta;
    public bool inChunks;
    public int chunkSize;

    // Update is called once per frame
    void Update()
    {
        int heightInRows = Mathf.Max(0, GetHeight(GetAllGroundedBlocklets()));
        float newY;
        if (inChunks)
        {
            newY = startPos.y + (Mathf.FloorToInt(heightInRows / chunkSize) * 5);
        } else
        {
            newY = startPos.y + heightInRows;
        }
        Vector3 newPos = new Vector3(transform.position.x, newY, transform.position.z);
        if (gradual)
        {
            transform.position = Vector3.MoveTowards(transform.position, newPos, maxDelta);
        } else
        {
            transform.position = newPos;
        }
    }

    List<Blocklet> GetAllGroundedBlocklets()
    {
        Block[] blocks = FindObjectsOfType<Block>();
        List<Blocklet> blocklets = new List<Blocklet>();
        foreach (Block block in blocks)
        {
            if (block.currentState == Block.BlockState.stable)
            {
                blocklets.AddRange(block.gameObject.GetComponentsInChildren<Blocklet>());
            }
        }
        return blocklets;
    }

    int GetHeight(List<Blocklet> blocklets)
    {
        int rowHeight = 0;
        foreach(Blocklet blocklet in blocklets)
        {
            rowHeight = Mathf.Max(blocklet.row, rowHeight);
        }
        return rowHeight;
    }
}
