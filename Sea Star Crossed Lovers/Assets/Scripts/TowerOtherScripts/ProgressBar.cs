using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Manages the movement of the progress bar during gameplay
public class ProgressBar : MonoBehaviour
{
    // The Slider object that moves the progress bar
    public Slider slider;

    // Object that manages block position. Gives provides information to update the slider
    private BlockManager _blockManager;
    /**
     * Start is called before the first frame update
     * Sets a local reference to the block manager and set the maximum value of the slider
     */
    public void Construct(BlockManager _blockManager, float goalHeight)
    {
        this._blockManager = _blockManager;
        slider.minValue = 0;
        slider.maxValue = goalHeight;
        slider.value = slider.minValue;
    }

    /**
     * Updates the value of the progress bar to the maximum height of the tower
     * Doesn't update if the tower height isn't within the bounds of the slider
     * 
     * TODO: Make slider update smoothly (look at camera movement coroutines)
     */
    public void updateProgress()
    {
        float towerHeight = _blockManager.highestBlockHeight;
        float currentHeight = slider.value;
        if(towerHeight != currentHeight && towerHeight >= slider.minValue)
        {
            if(towerHeight <= slider.maxValue)
            {
                slider.value = towerHeight;
            }
            else
            {
                slider.value = slider.maxValue;
            }
        }
    }
}
