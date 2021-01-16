using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBlockAtMouse : MonoBehaviour
{
    public GameObject Block;
    public float horizontalSpeed = 10;
    public float dropSpeed = 1;
    public int startingDownwardForce = 200;
    
    private float blockGravity;
    private Camera Camera;
    private GameObject activeBlock;

    private void Start()
    {
        Camera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && activeBlock == null)
        {
            Debug.Log("Left mouse button clicked");

            Vector3 spawnPosition = Camera.ScreenToWorldPoint(Input.mousePosition);
            spawnPosition.z = 0;

            activeBlock = Instantiate(Block, spawnPosition, Quaternion.identity);
            blockGravity = activeBlock.GetComponent<Rigidbody2D>().gravityScale;
            activeBlock.GetComponent<Rigidbody2D>().gravityScale = 0f;

            Debug.Log("Block spawnPosition: " + spawnPosition);
        }

        if (activeBlock != null) {
            float xOffset = Input.GetAxisRaw("Horizontal")*Time.deltaTime*horizontalSpeed;
            activeBlock.transform.position += new Vector3(xOffset, -dropSpeed*Time.deltaTime, 0);
        
            // detect if player is done with block
            if (Input.GetButtonDown("Drop")) {
                activeBlock.GetComponent<BlockCollision>().isActive = false;
            }

            // if an active block is no longer active, give it normal gravity
            if (!activeBlock.GetComponent<BlockCollision>().isActive) {
                activeBlock.GetComponent<Rigidbody2D>().gravityScale = blockGravity;
                activeBlock.GetComponent<Rigidbody2D>().AddForce(new Vector3(0, -startingDownwardForce, 0));
                activeBlock = null;
            }
        }


    }
}
