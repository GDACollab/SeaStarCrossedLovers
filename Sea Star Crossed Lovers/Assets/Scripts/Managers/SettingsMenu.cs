﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Waves waves;

    // Remember wave stats when paused
    private float resumeWaveSpeed = 0.01f;
    private float resumeSizeModifier = 0.9996f;

    public void toggleSettingsMenu() 
    {
        bool paused = gameObject.activeSelf;

        // Reverse the active setting
        gameObject.SetActive(!paused);

        // Pause/Unpause physics
        if (paused)
        {
            // Unpause
            Time.timeScale = 1;
            // Restart waves
            foreach (DisruptiveWave wave in waves.activeWaveList)
            {
                // Restart wave
                wave.speed = resumeWaveSpeed;
                wave.sizeModifier = resumeSizeModifier;
            }
        }
        else
        {
            // Pause
            Time.timeScale = 0;
            // Stop waves
            foreach (DisruptiveWave wave in waves.activeWaveList)
            {
                // Remember wave stats
                resumeWaveSpeed = wave.speed;
                resumeSizeModifier = wave.sizeModifier;
                // Stop wave
                wave.speed = 0;
                wave.sizeModifier = 1;
            }
        }
    }
}
