using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public Rigidbody2D rigidbody2D;
    public Vector2 shotVelocity;

    private BlockManager blockManager;

    public void Construct(BlockManager blockManager)
    {
        this.blockManager = blockManager;
    }

    private void Start()
    {
        rigidbody2D.velocity = shotVelocity; 
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null && collision.gameObject.layer == default)
        {
            Block hitBlock = collision.gameObject.GetComponent<Block>();
            if (hitBlock)
            {
                if (blockManager.activeBlock != hitBlock)
                {
                    blockManager.RemoveBlockFromList(hitBlock);
                }
            }
            Destroy(gameObject);
        }
    }
}
