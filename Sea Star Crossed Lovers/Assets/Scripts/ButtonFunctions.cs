using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonFunctions : MonoBehaviour
{
    public GameObject spawnBlock;
    public GameObject waves;

    public void toggleSpawn()
    {
        spawnBlock.GetComponent<SpawnBlock>().toggleBlockSpawn();
    }
}
