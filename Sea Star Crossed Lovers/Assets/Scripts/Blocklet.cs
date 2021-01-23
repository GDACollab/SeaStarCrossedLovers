using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocklet : MonoBehaviour
{
    private bool canFade;
    [Range(0, 10)]
    public float fadeSpeed;

    public int row;
    //blocklet position relative to platform
    private Vector3 relativePos;
    //size of blocklet
    private Vector2 size;

    public GameObject platform;

    // Start is called before the first frame update
    void Start()
    {
        row = -1;
        size = this.GetComponent<BoxCollider2D>().size;  
    }

    // Update is called once per frame
    void Update()
    {
        //get position of blocklet relative to position of platform
        relativePos = platform.transform.InverseTransformPoint(this.transform.position);
        //calculate row #
        row = (int)(relativePos.y / size.y);

        if (canFade) 
        {
            Color blockColor = this.GetComponent<Renderer>().material.color;
            float fadeAmount = blockColor.a - (fadeSpeed * Time.deltaTime);

            this.GetComponent<Renderer>().material.color = new Color(blockColor.r, blockColor.b, blockColor.g, fadeAmount);

            if (blockColor.a <= 0) Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //row condition is temporary, will need to find way to make threshold increase over time
        if (other.gameObject.tag == "Wave" && row <= 1) canFade = true;
    }
}
