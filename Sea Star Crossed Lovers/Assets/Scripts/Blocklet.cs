using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocklet : MonoBehaviour
{
    private bool canFade;
    private float fadeFinal = 0.5f;
    private bool markForDeletion;
    [Range(0, 10)]
    public float fadeSpeed;

    public int row;
    //blocklet position relative to platform
    private Vector3 relativePos;
    //size of blocklet
    private Vector2 size;

    public GameObject platform;
    public GameObject waves;

    public GameObject rowDebugObj;
    private TextMesh rowDebugText;
    private GameObject BlockParent;

    // Start is called before the first frame update
    void Start()
    {
        row = -1;
        size = this.GetComponent<BoxCollider2D>().size;
        platform = GameObject.FindWithTag("Platform");
        waves = GameObject.FindWithTag("Wave");

        rowDebugText = rowDebugObj.GetComponent<TextMesh>();
        BlockParent = gameObject.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //get position of blocklet relative to position of platform
        relativePos = platform.transform.InverseTransformPoint(this.transform.position);
        //calculate row #
        //row = (int)(relativePos.y / size.y);
        row = (int)(relativePos.y);

        // Update rowDebugObj rotation
        rowDebugObj.transform.rotation = Quaternion.identity;
        // Update rowDebugObj text
        rowDebugText.text = row.ToString();

        if (markForDeletion)
        {
            Color blockColor = this.GetComponent<Renderer>().material.color;
            //Debug.Log("before fading");
            if (blockColor.a > fadeFinal)
            {
                //Debug.Log("inside fading");
                float fadeAmount = blockColor.a - (fadeSpeed * Time.deltaTime);

                this.GetComponent<Renderer>().material.color = new Color(blockColor.r, blockColor.b, blockColor.g, fadeAmount);
            }
            else if (blockColor.a <= fadeFinal && waves.GetComponent<SimpleWave>().waveIsOver)
            {
                Destroy(gameObject);
                //Debug.Log("destroy");
            }
        }
    }

    public void MarkDelete(int rowsToDelete)
    {
        if (row <= rowsToDelete)
        {
            markForDeletion = true;
            //Debug.Log("markForDeletion");
        }
    }
}