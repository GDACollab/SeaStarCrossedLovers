using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public void toggleSettingsMenu() 
    {
        bool paused = gameObject.activeSelf;

        // Reverse the active setting
        gameObject.SetActive(!paused);

        // Pause/Unpause physics
        if (paused)
            Time.timeScale = 1;
        else
            Time.timeScale = 0;
    }
}
