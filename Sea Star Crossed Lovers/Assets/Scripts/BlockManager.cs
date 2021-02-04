using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    // Active if player has control over it
    public bool isActive = true;
    // Spawn new block if active block being deleted
    public bool beingDeleted = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Delete(int rowsToDelete)
    {
        beingDeleted = true;
        //Find all blocklets that make up the tetronimo, mark them for deletion
        foreach (Transform child in transform)
        {
            child.gameObject.GetComponent<Blocklet>().MarkDelete(rowsToDelete);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        isActive = false;
    }
}
