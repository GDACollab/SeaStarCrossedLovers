﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VN_AudioManager : MonoBehaviour
{
    public AudioSource buttonClick;
    public AudioSource buttonHover;

    private VN_Manager manager;

    public void Construct(VN_Manager manager)
    {
        this.manager = manager;
    }
}
