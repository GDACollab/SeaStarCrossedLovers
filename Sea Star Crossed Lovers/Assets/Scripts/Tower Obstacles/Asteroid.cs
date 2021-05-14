using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public Rigidbody2D rigidbody2D;
    private Vector2 shotVelocity;
    private Vector2 asteroidForce;

    private BlockManager blockManager;

    public void Construct(BlockManager blockManager, Vector2 shotVelocity, Vector2 asteroidForce)
    {
        this.blockManager = blockManager;
        this.shotVelocity = shotVelocity;
        this.asteroidForce = asteroidForce;
    }

    private void Start()
    {
        rigidbody2D.velocity = shotVelocity; 
    }

    private void Update()
    {
        if(gameObject.transform.position.x > 25)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other != null && other.gameObject.layer == default)
        {
            
            Block hitBlock = other.gameObject.GetComponent<Block>();
            if (hitBlock)
            {
                hitBlock.rigidBody.AddForce(asteroidForce, ForceMode2D.Force);
                // Destroy non active block
                //if (blockManager.activeBlock != hitBlock)
                //{
                //    blockManager.RemoveBlockFromList(hitBlock);
                //}
            }
            Destroy(gameObject);
        }
    }
}
