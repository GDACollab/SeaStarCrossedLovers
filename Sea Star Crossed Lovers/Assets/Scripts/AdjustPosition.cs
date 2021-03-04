using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustPosition : MonoBehaviour
{
    public Vector3 startPos;
    public bool gradual;
    public float maxDelta;
    public int chunkSize;
    public int minHeightOnScreen;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int heightInRows = Mathf.Max(0, GetHeight(GetAllGroundedBlocklets()));
        float newY;
        float adjustAmount;
        if (chunkSize != 0)
        {
            adjustAmount = (Mathf.FloorToInt(heightInRows / chunkSize) * chunkSize);
        } else
        {
            adjustAmount = heightInRows;
        }
        
        adjustAmount = Mathf.Max(0f, adjustAmount - minHeightOnScreen);
        newY = startPos.y + adjustAmount;
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
        BlockManager[] blocks = FindObjectsOfType<BlockManager>();
        List<Blocklet> blocklets = new List<Blocklet>();
        foreach (BlockManager block in blocks)
        {
            if (block.madeContact)
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
