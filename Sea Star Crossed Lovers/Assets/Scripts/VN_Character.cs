using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VN_Character : MonoBehaviour
{
    public CharacterData data;

    // Internal
    private Image currentImage;
    [HideInInspector]
    public RectTransform rectTransform;

    // Debug
    [HideInInspector]
    public Text nameText;

    // Start is called before the first frame update
    void Start()
    {
        currentImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();

        nameText.text = data.name;

        switch (data.Side)
        {
            case CharacterData.ScreenSide.left:
                // Change anchors and pivot to be on left middle of screen
                // rectTransform.anchoredPosition of (0,0) means the Character's left edge will be on the left screen edge
                rectTransform.pivot = new Vector2(0, 0.5f);
                rectTransform.anchorMin = new Vector2(0, 0.5f);
                rectTransform.anchorMax = new Vector2(0, 0.5f);
                break;
            case CharacterData.ScreenSide.right:
                // Change anchors and pivot to be on right middle of screen
                // rectTransform.anchoredPosition of (0,0) means the Character's right edge will be on the right screen edge
                rectTransform.pivot = new Vector2(1, 0.5f);
                rectTransform.anchorMin = new Vector2(1, 0.5f);
                rectTransform.anchorMax = new Vector2(1, 0.5f);
                break;
        }

        // Make sure character has default sprite and starts with it
        changeSprite("default");
        // Set character position offscreen 
        Transition(CharacterData.MoveTransition.teleport, CharacterData.TransitionDirection.exit);
    }

    public void changeSprite(string newSpriteName)
    {
        // Find sprite name from Sprites
        Sprite newSprite = data.Sprites.Find(x => x.spriteName == newSpriteName).emotionSprite;
        // If found, change image to newSprite
        if (newSprite) currentImage.sprite = newSprite;
        else Debug.LogError("VN_Character.changeSprite invalid newSpriteName");
    }

    // High level method to transition a Character in and out
    public void Transition(CharacterData.MoveTransition transition, CharacterData.TransitionDirection direction)
    {
        switch (transition)
        {
            case CharacterData.MoveTransition.teleport:
                teleportTransition(direction);
                break;
            case CharacterData.MoveTransition.slide:
                slideTransition(direction);
                break;
        }
    }

    Vector2 GetTargetPosition(CharacterData.TransitionDirection direction)
    {
        float targetX = 0;
        switch (direction)
        {
            case CharacterData.TransitionDirection.enter:
                targetX = data.ScreenEdgeDistance;
                switch (data.Side)
                {
                    // If on left, go right to be on target
                    case CharacterData.ScreenSide.left:
                        return new Vector2(targetX, 0);
                    case CharacterData.ScreenSide.right:
                        // If on right, go left to be on target
                        return new Vector2(-targetX, 0);
                }
                break;
            case CharacterData.TransitionDirection.exit:
                targetX = GameObject.Find("VN_Manager").GetComponent<VN_Manager>()
                    .OffScreenDistance + rectTransform.sizeDelta.x;
                switch (data.Side)
                {
                    // If on left, go left to be on target
                    case CharacterData.ScreenSide.left:
                        return new Vector2(-targetX, 0);
                    case CharacterData.ScreenSide.right:
                        // If on right, go right to be on target
                        return new Vector2(targetX, 0);
                }
                break;
        }

        // This should never happen
        Debug.LogError("Invalid transition target position found");
        return Vector2.zero;
    }

    void teleportTransition(CharacterData.TransitionDirection direction)
    {
        rectTransform.anchoredPosition = GetTargetPosition(direction);
    }

    void slideTransition(CharacterData.TransitionDirection direction)
    {
        Vector2 initialPosition = rectTransform.anchoredPosition;
        Vector2 endPosition = GetTargetPosition(direction);

        // From lerp tutorial: https://gamedevbeginner.com/the-right-way-to-lerp-in-unity-with-examples/
        StartCoroutine(slide());

        IEnumerator slide()
        {
            float time = 0;
            
            while (time < data.TransitionDuration)
            {
                time += Time.deltaTime;
                float t = time / data.TransitionDuration;
                // "smootherstep" https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro/
                t = t * t * t * (t * (6f * t - 15f) + 10f);
                rectTransform.anchoredPosition = Vector2.Lerp(initialPosition, endPosition, t);

                yield return null;
            }
            // Set postion to end in case Lerp isn't exact
            rectTransform.anchoredPosition = endPosition;
        }
    }
}