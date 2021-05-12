using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Blocklet : MonoBehaviour
{
    [HideInInspector] public BlockletData data;

    [Range(0, 1)]
    public float fadeFinal;
    [Range(0, 10)]
    public float fadeSpeed;

    [HideInInspector] public int row;
    //blocklet position relative to platform
    private Vector3 relativePos;
    //size of blocklet
    private Vector2 size;
    private bool canFade;
    private bool markForDeletion;

    [HideInInspector] public GameObject platform;
    [HideInInspector] public GameObject waves;

    private GameObject comparisonBlocklet;

    public GameObject rowDebugObj;
    private TextMesh rowDebugText;
    [HideInInspector] public Block blockParent;

    [SerializeField] private SpriteRenderer spriteRenderer;
    private BoxCollider2D blockletCollider;

    private void Awake()
    {
        blockletCollider = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        row = -1;
        size = this.GetComponent<BoxCollider2D>().size;
        platform = GameObject.FindWithTag("Platform");
        comparisonBlocklet = GameObject.FindWithTag("BlockletPosition");
        waves = GameObject.FindWithTag("Wave");

        rowDebugText = rowDebugObj.GetComponent<TextMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        //get position of blocklet relative to position of platform
        relativePos = platform.transform.InverseTransformPoint(this.transform.position);
        //calculate row #
        //row = (int)(relativePos.y / size.y);
        row = (int)(relativePos.y - (comparisonBlocklet.transform.position.y - platform.transform.position.y));
        // Freeze rowDebugObj rotation
        rowDebugObj.transform.rotation = Quaternion.identity;
        // Update rowDebugObj text
        rowDebugText.text = row.ToString();

        if (markForDeletion)
        {
            Color blockColor = spriteRenderer.material.color;
            //Debug.Log("before fading");
            if (blockColor.a > fadeFinal)
            {
                //Debug.Log("inside fading");
                float fadeAmount = blockColor.a - (fadeSpeed * Time.deltaTime);

                spriteRenderer.material.color = new Color(blockColor.r, blockColor.b, blockColor.g, fadeAmount);
            }
            else if (blockColor.a <= fadeFinal && waves.GetComponent<SimpleWave>().waveIsOver)
            {
                // Delete Blocklets
                blockParent.blockletChildren.Remove(gameObject);
                blockParent.CheckFullyDeleted();
                Destroy(gameObject);
                blockParent.state = Block.BlockState.stable;
            }
        }
    }

    public void MarkDelete(int rowsToDelete)
    {
        if (row <= rowsToDelete)
        {
            markForDeletion = true;
        }
    }

    public Blocklet Construct(BlockletData blockletData, Block block)
    {
        blockParent = block;
        data = blockletData;

        spriteRenderer.color = blockletData.color;
        spriteRenderer.sprite = blockletData.sprite;
        return this;
    }
}