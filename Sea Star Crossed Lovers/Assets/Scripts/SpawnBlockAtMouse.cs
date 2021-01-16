using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBlockAtMouse : MonoBehaviour
{
    public GameObject Block;

    public float horizontalSpeed = 10;
    public float dropSpeed = 1;
    public int startingDownwardForce = 150;
    
    private float blockGravity;
    private Camera Camera;

    private GameObject activeBlock;
    private Rigidbody2D activeRB;
    private BlockCollision blockCollision;

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
            activeRB = activeBlock.GetComponent<Rigidbody2D>();
            blockCollision = activeBlock.GetComponent<BlockCollision>();

            blockGravity = activeRB.gravityScale;
            activeRB.gravityScale = 0f;

            Debug.Log("Block spawnPosition: " + spawnPosition);
        }

        if (activeBlock != null) {
            float xOffset = Input.GetAxisRaw("Horizontal")*Time.deltaTime*horizontalSpeed;
            activeBlock.transform.position += new Vector3(xOffset, -dropSpeed*Time.deltaTime, 0);
        
            // detect if player is done with block
            if (Input.GetButtonDown("Drop")) {
                blockCollision.isActive = false;
            }

            // if an active block is no longer active, give it normal gravity
            if (!blockCollision.isActive) {
                activeRB.gravityScale = blockGravity;
                activeRB.AddForce(new Vector3(0, -startingDownwardForce, 0));
                activeBlock = null;
            }
        }


    }
}
