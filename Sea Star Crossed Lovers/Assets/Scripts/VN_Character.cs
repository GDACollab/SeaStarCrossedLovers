using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VN_Character : MonoBehaviour
{
    public CharacterData Data;

    // Internal
    private Image currentImage;
    private RectTransform rectTransform;

    // Debug
    [SerializeField] private Text nameText;

    private void Awake()
    {
        currentImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetData(CharacterData toSetData)
    {
        Data = toSetData;
        if (toSetData)
        {
            // Debug nametag
            nameText.text = toSetData.name;
            switch (toSetData.side)
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
        }
        else
        {
            nameText.text = "None";
        }
    }

    public void ChangeSprite(string newSpriteName)
    {
        if(Data)
        {
            // Find sprite name from Sprites
            Sprite newSprite = Data.sprites.Find(x => x.spriteName == newSpriteName).emotionSprite;
            // If found, change image to newSprite
            if (newSprite) currentImage.sprite = newSprite;
            else Debug.LogError("VN_Character.ChangeSprite invalid newSpriteName");
        }
        else
        {
            Debug.LogError("Cannot ChangeSprite when VN_Character has null Data");
        }
    }

    #region Transition Functions
    public void EnterScreen(CharacterData.MoveTransition transition, CharacterData data)
    {
        StartCoroutine(Co_EnterScreen(transition, data));
    }

    public void ExitScreen(CharacterData.MoveTransition transition)
    {
        StartCoroutine(Co_ExitScreen(transition));
    }

    public IEnumerator Co_ExitScreen(CharacterData.MoveTransition transition)
    {
        yield return StartCoroutine(Co_Transition(transition, CharacterData.TransitionDirection.exit));
        ChangeSprite("default");
        SetData(null);
        yield break;
    }

    public IEnumerator Co_EnterScreen(CharacterData.MoveTransition transition, CharacterData data)
    {
        SetData(data);
        ChangeSprite(data.defaultSpriteName);
        StartCoroutine(Co_Transition(transition, CharacterData.TransitionDirection.enter));
        yield break;
    }

    IEnumerator Co_Transition(CharacterData.MoveTransition transition, CharacterData.TransitionDirection direction)
    {
        Vector2 initialPosition = rectTransform.anchoredPosition;
        Vector2 endPosition = GetTargetPosition(direction);

        switch (transition)
        {
            case CharacterData.MoveTransition.teleport:
                yield return StartCoroutine(Co_TeleportTransition(endPosition));
                yield break;
            case CharacterData.MoveTransition.slide:
                yield return StartCoroutine(Co_SlideTransition(initialPosition, endPosition));
                yield break;
        }
    }

    IEnumerator Co_TeleportTransition(Vector2 endPosition)
    {
        rectTransform.anchoredPosition = endPosition;
        yield break;
    }

    IEnumerator Co_SlideTransition(Vector2 initialPosition, Vector2 endPosition)
    {
        // From lerp tutorial: https://gamedevbeginner.com/the-right-way-to-lerp-in-unity-with-examples/
        float time = 0;

        var wait = new WaitForEndOfFrame();

        while (time < Data.transitionDuration)
        {
            time += Time.deltaTime;
            float t = time / Data.transitionDuration;
            // "smootherstep" https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro/
            t = t * t * t * (t * (6f * t - 15f) + 10f);
            rectTransform.anchoredPosition = Vector2.Lerp(initialPosition, endPosition, t);

            yield return wait;
        }
        // Set postion to end in case Lerp isn't exact
        rectTransform.anchoredPosition = endPosition;

        yield break;
    }

    public void StopCoroutines()
    {
        StopAllCoroutines();
    }

    Vector2 GetTargetPosition(CharacterData.TransitionDirection direction)
    {
        float targetX = 0;
        switch (direction)
        {
            case CharacterData.TransitionDirection.enter:
                targetX = Data.screenEdgeDistance;
                switch (Data.side)
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
                switch (Data.side)
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
    #endregion
}