using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
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
        //Find all blocklets that make up the tetronimo, mark them for deletion
        foreach (Transform child in transform)
        {
            child.gameObject.GetComponent<Blocklet>().MarkDelete(rowsToDelete);
        }
    }
}
