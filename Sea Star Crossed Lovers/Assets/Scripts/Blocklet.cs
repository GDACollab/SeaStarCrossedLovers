using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocklet : MonoBehaviour
{
    [Range(1, 100)]
    public int health;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //this is temporary. will update with proper behaviour later
        if (other.gameObject.tag == "wave") Destroy(gameObject);
    }
}
