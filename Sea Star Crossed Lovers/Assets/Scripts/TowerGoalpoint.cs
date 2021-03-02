using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerGoalpoint : MonoBehaviour
{
    private BlockManager _blockManager;

    private float goalHeight;

    public void Construct(BlockManager blockManager)
    {
        _blockManager = blockManager;
    }

    private void Awake()
    {
        goalHeight = gameObject.transform.position.y;
    }

    public bool CheckWin()
    {
        if (_blockManager.highestBlock == null)
        {
            return false;
        }

        float current = _blockManager.highestBlockHeight;
        if (current > goalHeight)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
