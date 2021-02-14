using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "CharacterData")]
public class CharacterData : ScriptableObject
{
    public enum ScreenSide { left, right };
    [Header("Properties")]
    [Tooltip("Position of character on the screen")]
    public ScreenSide Side;

    public enum MoveTransition { teleport, slide };
    [Tooltip("Style of sprite movement when entering/exiting the screen")]
    public MoveTransition transition;

    [Tooltip("Distance in pixels away from the edge of the screen")]
    public int ScreenEdgeDistance;
    [Tooltip("Duration in seconds of Character enter/exit transition")]
    public float TransitionDuration;

    public enum TransitionDirection { enter, exit };

    [System.Serializable] public struct CharacterSprites
    {
        public string spriteName;
        public Sprite emotionSprite;
    }
    [Header("Character's Art Assets")]
    [Tooltip("List of character's sprites and their names")]
    public List<CharacterSprites> Sprites;
}
