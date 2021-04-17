using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustPosition : MonoBehaviour
{
    private List<Vector3> startPositions;
    public List<GameObject> objects;
    public bool gradual;
    public float maxDelta;
    public int chunkSize;
    public int minHeightOnScreen;
    void Start()
    {
        startPositions = new List<Vector3>();
        foreach(GameObject obj in objects) {
            startPositions.Add(obj.transform.position);
        }
        Debug.Log("NUM POSITIONS: " + startPositions.Count);
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
        }
        else
        {
            adjustAmount = heightInRows;
        }

        for (int i = 0; i < objects.Count; i++)
        {
            adjustAmount = Mathf.Max(0f, adjustAmount - minHeightOnScreen);
            newY = startPositions[i].y + adjustAmount;
            Vector3 newPos = new Vector3(objects[i].transform.position.x, newY, objects[i].transform.position.z);
            if (gradual)
            {
                objects[i].transform.position = Vector3.MoveTowards(objects[i].transform.position, newPos, maxDelta);
            }
            else
            {
                objects[i].transform.position = newPos;
            }
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
