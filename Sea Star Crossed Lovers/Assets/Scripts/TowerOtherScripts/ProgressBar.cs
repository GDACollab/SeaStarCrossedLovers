using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Manages the movement of the progress bar during gameplay
public class ProgressBar : MonoBehaviour
{
    // The Slider object that moves the progress bar
    public Slider slider;

    /** The maximum amount the slider can change each update
     *  If 0, no maximum change will be enforced
     *  Default value = 0 
     */
    public float maxChange = 0;

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
     * Updates the value of the progress bar to the current height of the tower
     * Updates only within the bounds of maxChange to give the progression a smooth look
     */
    public void updateProgress()
    {
        float towerHeight = _blockManager.highestBlockHeight;
        float currentHeight = slider.value;
        if(towerHeight != currentHeight)
        {
            slider.value = getProgressValue(towerHeight);
        }
    }

    /**
     * Calculates to what value the slider needs to change this update to be as accurate and smooth as possible
     * If maxChange == 0, the progress value will be the tower height
     * 
     * @Param towerHeight: the current height of the tower and the value the slider aims to reflect
     * @Return: How much the progress slider will change this update
     */
    private float getProgressValue(float towerHeight)
    {
        float progressValue = towerHeight;
        // The reflected progress will be the height of the tower unless the distance between the current and new height is larger than maxChange
        if(maxChange != 0 && Mathf.Abs(towerHeight - slider.value) > maxChange)
        {
            // If the target height is larger than the current value, add maxChange, otherwise subtract maxChange
            if(towerHeight > slider.value)
            {
                progressValue = slider.value + maxChange;
            }
            else
            {
                progressValue = slider.value - maxChange;
            }
        }

        // Safety checks to ensure that the slider doesn't try to update to a value out of its range
        if(progressValue > slider.maxValue)
        {
            progressValue = slider.maxValue;
        }
        if(progressValue < slider.minValue)
        {
            progressValue = slider.minValue;
        }
        
        return progressValue;
    }
}
