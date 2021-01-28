using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBlock : MonoBehaviour
{
    public List<GameObject> BlockList;

    public float horizontalSpeed = 10;
    public float fallSpeed = 5;
    public float dropForce = 300;
    public int startingDownwardForce = 150;
    public int spawnDelay = 1;

    private float blockGravity;

    private bool waitingForBlock;

    private GameObject activeBlock;
    private Rigidbody2D activeRB;
    private BlockCollision blockCollision;

    private void Start()
    {
        spawnNewBlock();
    }

    private void Update()
    {

        // control an active block
        if (activeBlock != null) {

            // apply horizontal and vertical change
            float xOffset = Input.GetAxisRaw("Horizontal")*Time.deltaTime*horizontalSpeed;
            activeBlock.transform.position += new Vector3(xOffset, -fallSpeed*Time.deltaTime, 0);
        
            // detect if player has dropped block
            if (Input.GetButtonDown("Drop")) {
                // apply a starting force to make drop feel more natural (momentum)
                // give block normal gravity
                activeRB.AddForce(new Vector3(0, -dropForce, 0));
                activeRB.gravityScale = blockGravity;
                blockCollision.isActive = false;
            }

            // detect rotation
            if (Input.GetButtonDown("RotateCounterclockwise")) {
                activeRB.transform.Rotate(new Vector3(0, 0, 90));
            } else if (Input.GetButtonDown("RotateClockwise")) {
                activeRB.transform.Rotate(new Vector3(0, 0, -90));
            }
            
        }

        // if an active block collides, give it normal gravity
        // and prepare to spawn new block
        if (!blockCollision.isActive && !waitingForBlock)
        {
            waitingForBlock = true;
            activeRB.gravityScale = blockGravity;
            activeBlock = null;

            StartCoroutine(delaySpawnBlock());
        }
    }

    IEnumerator delaySpawnBlock()
    {
        yield return new WaitForSeconds(spawnDelay);
        waitingForBlock = false;
        spawnNewBlock();
    }

    // Spawn random block from blockList at parent gameObject position
    private void spawnNewBlock()
    {
        Vector3 spawnPosition = gameObject.transform.position;
        spawnPosition.z = 0;

        var selectedBlock = BlockList[Random.Range(0, BlockList.Count)];
        activeBlock = Instantiate(selectedBlock, spawnPosition, Quaternion.identity);
        activeRB = activeBlock.GetComponent<Rigidbody2D>();
        blockCollision = activeBlock.GetComponent<BlockCollision>();

        blockGravity = activeRB.gravityScale;
        activeRB.gravityScale = 0f;
    }
}
