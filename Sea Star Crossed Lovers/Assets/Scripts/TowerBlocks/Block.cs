using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [HideInInspector] public BlockData data;

    [HideInInspector] public enum BlockState { active, preStable, falling, stable, deleting };
    public BlockState state = BlockState.active;
    [HideInInspector] public SimpleWave wave;
    [HideInInspector] public List<GameObject> blockletChildren = new List<GameObject>();

    public AudioSource audioSource;
    [HideInInspector] public Rigidbody2D rigidBody;

    [HideInInspector] public BlockManager blockManager;
    private BlockController blockController;
    private SpawnBlock spawnBlock;

    [HideInInspector] public GameObject debugObj;
    private TextMesh debugObjText;

    void Awake()
    {
        wave = GameObject.Find("Waves").GetComponent<SimpleWave>();
        debugObjText = debugObj.GetComponent<TextMesh>();
        rigidBody = GetComponent<Rigidbody2D>();
    }

    public Block Construct(BlockManager blockManager, BlockController blockController, SpawnBlock spawnBlock, BlockData data, Vector2 origin)
    {
        this.blockManager = blockManager;
        this.blockController = blockController;
        this.spawnBlock = spawnBlock;
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
        // Get a list of all blockletChildren
        foreach (Transform child in gameObject.transform)
        {
            if (child.gameObject.tag == "Blocklet")
            {
                blockletChildren.Add(child.gameObject);
            }
        }

        /* Since each was given a transform bases on a grid, the center position 
         * of the children isn't the same as the Block GameObject position
         * To fix this, first get the blocklets' center position by averaging 
         * each of their positions
        */
        int numChildren = blockletChildren.Count;
        Vector3 sumVector = new Vector3(0, 0, 0);
        foreach (GameObject child in blockletChildren)
        {
            sumVector += child.transform.position;
        }
        Vector3 center = sumVector / numChildren;

        /* Some wizardary vector math. Each child position is transformed so that they
         * originate from the gameobject position with an offset calculated from the
         * childrens relative position to their calculated aggregate center.
        */
        foreach (GameObject child in blockletChildren)
        {
            child.transform.position = gameObject.transform.position -
                (center - child.transform.position);
        }

        // Center debugObj
        debugObj.transform.position = gameObject.transform.position;

        return this;
    }

    void Update()
    {
        if(state != BlockState.active)
        {
            rigidBody.gravityScale = spawnBlock.blockGravity;
        }

        // Freeze rowDebugObj rotation
        debugObj.transform.rotation = Quaternion.identity;
        // Update rowDebugObj text
        debugObjText.text = state.ToString();
        //debugObjText.text = gameObject.GetComponent<Rigidbody2D>().gravityScale.ToString("F1");
        //debugObjText.text = gameObject.transform.position.x.ToString("F0");

        // Delete Block GameObject if go far below wave
        if (gameObject.transform.position.y < wave.transform.position.y - 10)
        {
            blockManager.RemoveBlockFromList(this);
        }
    }

    public void Delete(int rowsToDelete)
    {
        if (state != BlockState.deleting)
        {
            state = BlockState.deleting;
            if (!wave.waveIsOver)
            {
                //Find all blocklets that make up the tetronimo, mark them for deletion
                foreach (GameObject child in blockletChildren)
                    child.GetComponent<Blocklet>().MarkDelete(rowsToDelete);
            }
        }
    }

    public void CheckFullyDeleted()
    {
        if (blockletChildren.Count == 0)
        {
            blockManager.RemoveBlockFromList(this);
            Destroy(this);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision != null && collision.gameObject.layer == default &&
            (state == BlockState.active))
        {
            state = BlockState.preStable;
            audioSource.Play();
            StartCoroutine(PreStableControlTimer(blockController.prestableControlTime));
        }
    }

    private IEnumerator PreStableControlTimer(float time)
    {
        yield return new WaitForSeconds(time);
        state = BlockState.stable;
        blockManager.activeBlock = null;
    } 
}
