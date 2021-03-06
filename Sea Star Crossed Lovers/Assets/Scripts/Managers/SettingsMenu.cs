﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool wavesExist = true;

    [Header("References")]
    [SerializeField] private Waves waves;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixerGroup audioMixerGroup;

    [Header("Sound Effects")]
    [SerializeField] private AudioSource pauseSound;
    [SerializeField] private AudioSource unpauseSound;

    // Remember wave stats when paused
    private float resumeWaveSpeed = 0.01f;
    private float resumeSizeModifier = 0.9996f;

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("masterVolume", Mathf.Log10(volume) * 20);
    }

    public void ToggleSettingsMenu() 
    {
        bool paused = gameObject.activeSelf;

        // Show or hide the menu
        gameObject.SetActive(!paused);

        // Pause/Unpause physics
        if (paused)
        {
            // Unpause
            Time.timeScale = 1;
            // Restart waves
            if (wavesExist)
            {
                foreach (DisruptiveWave wave in waves.activeWaveList)
                {
                    // Restart wave
                    wave.speed = resumeWaveSpeed;
                    wave.sizeModifier = resumeSizeModifier;
                }
            }
        }
        else
        {
            // Pause
            Time.timeScale = 0;
            // Stop waves
            if (wavesExist)
            {
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

    private void OnEnable() {
        // Sound Effect
        pauseSound.Play();
    }

    private void OnDisable() {
        // Sound Effect
        unpauseSound.Play();
    }
}
