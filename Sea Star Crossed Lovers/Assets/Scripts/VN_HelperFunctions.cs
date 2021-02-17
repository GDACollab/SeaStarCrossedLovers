using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VN_HelperFunctions
{
    static VN_Manager _manager;

    public VN_HelperFunctions(VN_Manager manager)
    {
        _manager = manager;
    }

    /// <summary>
    /// Gets the Vector2 target position of a transition for a VN_Character from a TransitionDirection
    /// </summary>
    /// <param name="character">VN_Character who needs the target position</param>
    /// <param name="direction">TransitionDirection of character's planned transition</param>
    public static Vector2 GetTransitionTarget(VN_Character character, CharacterData.TransitionDirection direction)
    {
        CharacterData data = character.data;
        float targetX = 0;
        switch (direction)
        {
            case CharacterData.TransitionDirection.enter:
                targetX = data.screenEdgeDistance;
                switch (data.side)
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
                targetX = _manager.OffScreenDistance + character.rectTransform.sizeDelta.x;
                switch (data.side)
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
}