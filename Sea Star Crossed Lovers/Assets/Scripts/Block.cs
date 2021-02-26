using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public enum BlockState { active, falling, stable, deleting };
    public BlockState currentState = BlockState.active;
    //// Active if player has control over it
    //public bool isActive = true;
    //// Tracks if the block has made contact (ground or other block)
    //public bool madeContact = false;
    // Spawn new block if active block being deleted
    //public bool beingDeleted = false;

    private BlockManager _blockManager;

    public void Construct(BlockManager blockManager)
    {
        _blockManager = blockManager;
    }

    public void Delete(int rowsToDelete)
    {
        currentState = BlockState.deleting;
        //Find all blocklets that make up the tetronimo, mark them for deletion
        foreach (Transform child in transform)
        {
            child.gameObject.GetComponent<Blocklet>().MarkDelete(rowsToDelete);
        }
    }

    public void CheckFullyDeleted()
    {
        if (transform.childCount == 0)
        {
            _blockManager.RemoveBlockFromList(this);
            Destroy(this);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        currentState = BlockState.stable;
    }
}
