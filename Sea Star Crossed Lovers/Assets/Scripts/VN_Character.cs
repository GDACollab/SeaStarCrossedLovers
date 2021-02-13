using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VN_Character : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Distance in pixels away from the edge of the screen")]
    public int ScreenEdgeDistance;
    [Tooltip("Duration in seconds of Character enter/exit transition")]
    public float TransitionDuration;

    
    [System.Serializable]
    public struct CharacterSprites
    {
        public string name;
        public Sprite image;
    }
    [Header("Character's Art Assets")]
    [Tooltip("List of character's sprites and their names")]
    public List<CharacterSprites> Sprites;

    public enum ScreenSide { left, right };
    [Header("Properties")]
    [Tooltip("Position of character on the screen")]
    public ScreenSide Side;

    public enum MoveTransition { teleport, slide };
    [Tooltip("Style of sprite movement when entering/exiting the screen")]
    public MoveTransition transition;

    public enum TransitionDirection { enter, exit };

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

        nameText.text = gameObject.name;

        switch (Side)
        {
            case ScreenSide.left:
                // Change anchors and pivot to be on left middle of screen
                // rectTransform.anchoredPosition of (0,0) means the Character's left edge will be on the left screen edge
                rectTransform.pivot = new Vector2(0, 0.5f);
                rectTransform.anchorMin = new Vector2(0, 0.5f);
                rectTransform.anchorMax = new Vector2(0, 0.5f);
                break;
            case ScreenSide.right:
                // Change anchors and pivot to be on right middle of screen
                // rectTransform.anchoredPosition of (0,0) means the Character's right edge will be on the right screen edge
                rectTransform.pivot = new Vector2(1, 0.5f);
                rectTransform.anchorMin = new Vector2(1, 0.5f);
                rectTransform.anchorMax = new Vector2(1, 0.5f);
                break;
        }

        // Make sure character has default sprite and starts with it
        changeSprite("default");
    }

    public void changeSprite(string newSpriteName)
    {
        // Find sprite name from Sprites
        Sprite newSprite = Sprites.Find(x => x.name == newSpriteName).image;
        // If found, change image to newSprite
        if (newSprite) currentImage.sprite = newSprite;
        else Debug.LogError("VN_Character.changeSprite invalid newSpriteName");
    }

    // High level method to transition a Character in and out
    public void Transition(MoveTransition transition, TransitionDirection direction)
    {
        switch (transition)
        {
            case MoveTransition.teleport:
                teleportTransition(direction);
                break;
            case MoveTransition.slide:
                slideTransition(direction);
                break;
        }
    }

    Vector2 GetTargetPosition(TransitionDirection direction)
    {
        float targetX = 0;
        switch (direction)
        {
            case TransitionDirection.enter:
                targetX = ScreenEdgeDistance;
                break;
            case TransitionDirection.exit:
                targetX = GameObject.Find("VN_Manager").GetComponent<VN_Manager>().OffScreenDistance;
                break;
        }

        switch (Side)
        {
            // If on left, go right to be on target
            case ScreenSide.left:
                return new Vector2(targetX, 0);
            case ScreenSide.right:
            // If on right, go left to be on target
                return new Vector2(-targetX, 0);
        }
        // This should never happen
        Debug.LogError("Invalid transition target position found");
        return Vector2.zero;
    }

    void teleportTransition(TransitionDirection direction)
    {
        rectTransform.anchoredPosition = GetTargetPosition(direction);
    }

    void slideTransition(TransitionDirection direction)
    {
        Vector2 initialPosition = rectTransform.anchoredPosition;
        Vector2 endPosition = GetTargetPosition(direction);

        // From lerp tutorial: https://gamedevbeginner.com/the-right-way-to-lerp-in-unity-with-examples/
        StartCoroutine(slide());

        IEnumerator slide()
        {
            float time = 0;
            
            while (time < TransitionDuration)
            {
                time += Time.deltaTime;
                float t = time / TransitionDuration;
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