using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TweenManager : MonoBehaviour
{
    void Awake()
    {
        DOTween.Init();
    }
}
