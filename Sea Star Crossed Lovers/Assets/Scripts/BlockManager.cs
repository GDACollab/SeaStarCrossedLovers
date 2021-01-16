using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager for blocklets, controlling their creation, movement, and rotation.
/// </summary>
public class Block : MonoBehaviour
{
    public static List<BlockShape> PossibleBlockShapes;

    BlockShape shape;
    List<Blocklet> SubBlocks { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        // Select a random or predetermined shape
        shape = PossibleBlockShapes[(int)(Random.Range(0, PossibleBlockShapes.Count - 1))]
        // Create blocklets in that shape, glue them together
        SubBlocks = shape.Create();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
