using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleWave : MonoBehaviour
{
    public void MakeWave() {
        GetComponent<Waves>().GenerateWave(new DisruptiveWave());
    }
}
