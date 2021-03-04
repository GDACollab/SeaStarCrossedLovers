using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VN_Util
{
    static VN_Manager _manager;

    public VN_Util(VN_Manager manager)
    {
        _manager = manager;
    }

    public static char[] toTrim = { '\"', ' ', '\n' };

    /// <summary>
    /// Gets the Vector2 target position of a transition for a VN_Character from a TransitionDirection
    /// </summary>
    /// <param name="character">VN_Character who needs the target position</param>
    /// <param name="direction">TransitionDirection of character's planned transition</param>
    public static Vector2 GetTransitionTarget(VN_Character character,
        CharacterData.TransitionDirection direction)
    {
        CharacterData data = character.data;
        float targetY = character.rectTransform.anchoredPosition.y;
        float targetX = 0;
        switch (direction)
        {
            case CharacterData.TransitionDirection.enter:
                targetX = data.screenEdgeDistance;
                switch (data.screenSide)
                {
                    // If on left, go right to be on target
                    case CharacterData.ScreenSide.left:
                        return new Vector2(targetX, targetY);
                    case CharacterData.ScreenSide.right:
                        // If on right, go left to be on target
                        return new Vector2(-targetX, targetY);
                }
                break;
            case CharacterData.TransitionDirection.exit:
                targetX = _manager.offScreenDistance + character.rectTransform.sizeDelta.x;
                switch (data.screenSide)
                {
                    // If on left, go left to be on target
                    case CharacterData.ScreenSide.left:
                        return new Vector2(-targetX, targetY);
                    case CharacterData.ScreenSide.right:
                        // If on right, go right to be on target
                        return new Vector2(targetX, targetY);
                }
                break;
        }

        // This should never happen
        Debug.LogError("Invalid transition target position found");
        return Vector2.zero;
    }

    /**
	* Finds the data corresponding to a specified character
	*
	* @param characterName: the name of the character we are getting the data from
	* @return: the data for the specified character
	*/
    public static CharacterData FindCharacterData(string characterName)
    {
        // Get currentSpeaker by finding speakerName in CharacterObjects
        CharacterData character = _manager.AllCharacterData.Find(x => x.name == characterName);

        // Catch character being null
        if (!character)
        {
            character = null;
            Debug.LogError("Character of name " + characterName + " could not be found");
        }
        return character;
    }

    /**
	* Finds a character corresponding to the given data
	*
	* @param data: the data of the character we are trying to find
	* @return: the character for the specified data
	*/
    public static VN_Character FindCharacterObj(CharacterData data)
    {
        CharacterData characterData = _manager.AllCharacterData.Find(x => x == data);

        if (!characterData)
        {
            Debug.LogError("Cannot find " + data.name + " in AllCharacterData");
            return null;
        }

        foreach (VN_Character charObj in _manager.CharacterObjects)
        {
            if (charObj.data == characterData) return charObj;
        }

        Debug.LogError("Cannot find data of " + data.name + " in CharacterObjects");
        return null;
    }

    public static string RemoveSubstring(string source, string toRemove)
    {
        return source.Remove(source.IndexOf(toRemove), toRemove.Length);
    }
}