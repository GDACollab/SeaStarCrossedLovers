using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "ScriptableObjects/CharacterData")]
public class CharacterData : ScriptableObject
{
    public enum ScreenSide { left, right };
    public enum MoveTransition { teleport, slide };
    public enum TransitionDirection { enter, exit };

    [Header("Settings")]
    public CharacterTransition transition;

    [Tooltip("Starting sprite name; make sure it exists in the sprites list")]
    public string defaultSpriteName;

    [Tooltip("Position of character on the screen")]
    public ScreenSide screenSide;

    [Tooltip("Distance in pixels away from the edge of the screen")]
    public int screenEdgeDistance;
    [Tooltip("Duration in seconds of Character enter/exit transition")]
    public float transitionDuration;

    [System.Serializable] public struct CharacterSprites
    {
        public string spriteName;
        public Sprite emotionSprite;
    }
    [Header("Character's Art Assets")]
    [Tooltip("List of character's sprites and their names")]
    public List<CharacterSprites> characterSprites;
}
