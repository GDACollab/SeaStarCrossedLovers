using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [HideInInspector] public BlockData data;

    [HideInInspector] public enum BlockState { active, falling, stable, deleting };
    [HideInInspector] public BlockState currentState = BlockState.active;
    [HideInInspector] public SimpleWave wave;

    public AudioSource audioSource;

    private BlockManager _blockManager;

    void Awake()
    {
        wave = GameObject.Find("Waves").GetComponent<SimpleWave>();
    }

    public void Construct(BlockManager blockManager)
    {
        _blockManager = blockManager;
    }

    public void Delete(int rowsToDelete)
    {
        currentState = BlockState.deleting;
        if (!wave.waveIsOver)
        {
            //Find all blocklets that make up the tetronimo, mark them for deletion
            foreach (Transform child in transform)
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
        if (currentState != BlockState.stable)
        {
            audioSource.Play();
        }
        currentState = BlockState.stable;
    }

    public Block Construct(BlockData data, Vector2 origin)
    {
        this.data = data;
        // Construct the block out of the filled cell grid
        for (int cell_x = 0; cell_x < data.GRID_SIZE; cell_x++)
        {
            for (int cell_y = 0; cell_y < data.GRID_SIZE; cell_y++)
            {
                BlockletData thisBlockletData = data.GridCellDictionary[(cell_x, cell_y)];

                if (thisBlockletData != null)
                {
                    Vector3 blockletScale = thisBlockletData.blockletPrefab.transform.localScale;

                    /* I honestly don't know why 4 works but it does since blocklet positions all seem
                     * corret when changing the blocklet prefab scale
                    */
                    Vector2 blockletPosition = new Vector2(origin.x + 4 * blockletScale.x * cell_x,
                        origin.y + 4 * blockletScale.y * cell_y);

                    Blocklet newBlocklet = Instantiate(thisBlockletData.blockletPrefab,
                        blockletPosition, Quaternion.identity, gameObject.transform);
                    newBlocklet.Construct(thisBlockletData, this);
                }
            }
        }
        /* Since each was given a transform bases on a grid, the center position 
         * of the children isn't where the same as the Block gameobject position
         * 
        */
        int numChildren = 0;
        Vector3 sumVector = new Vector3(0, 0, 0);
        foreach (Transform child in gameObject.transform)
        {
            numChildren++;
            sumVector += child.transform.position;
        }

        Vector3 center = sumVector / numChildren;

        /* Some wizardary vector math. After getting the children's center position
         * by averaging their positions, each child position is changed so that they
         * originate from the gameobject position with an offset calculated from the
         * childrens relative position to their center.
        */
        foreach (Transform child in gameObject.transform)
        {
            child.transform.position = gameObject.transform.position -
                (center - child.transform.position);
        }

        return this;
    }
}
