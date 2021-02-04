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

    // Start is called before the first frame update
    void Start()
    {
        row = -1;
        size = this.GetComponent<BoxCollider2D>().size;
        platform = GameObject.FindWithTag("Platform");
        waves = GameObject.FindWithTag("Wave");
    }

    // Update is called once per frame
    void Update()
    {
        //get position of blocklet relative to position of platform
        relativePos = platform.transform.InverseTransformPoint(this.transform.position);
        //calculate row #
        row = (int)(relativePos.y / size.y);

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