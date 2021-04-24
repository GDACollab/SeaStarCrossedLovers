using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private ParticleSystem waves;

    public void toggleSettingsMenu() 
    {
        bool paused = gameObject.activeSelf;

        // Reverse the active setting
        gameObject.SetActive(!paused);

        // Pause/Unpause physics
        if (paused)
        {
            Time.timeScale = 1;
            waves.Play(true);
        }
        else
        {
            Time.timeScale = 0;
            waves.Pause(true);
        }
    }
}
