using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class holding a potential block shape to use, e.g. the T or | shapes from the set of tetrominos.
/// </summary>
/// <remarks>I'm thinking we subclass this for each shape, though other data structures should work.</remarks>
public class BlockShape
{
    HashSet<(int, int)> blockletCoords;

    public List<Blocklet> Create()
    {
        foreach ((int, int) coord in blockletCoords)
        {
            // spawn blocklet at specified coordinates (relative offsets to some in-scene position, probably)
            // add blocklet to the list
            // glue together
        }
    }
}
