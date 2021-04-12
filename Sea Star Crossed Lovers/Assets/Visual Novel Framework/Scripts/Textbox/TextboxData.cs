using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TextboxData", menuName = "ScriptableObjects/TextboxData")]
public class TextboxData : ScriptableObject
{
    [Header("Art Assets")]
    public Sprite textboxSprite;
    public Sprite nameboxSprite;
    public Sprite settingsSprite;
    public Sprite skipSprite;

    public float activeOffset;
    public float hiddenOffset;

    [System.Serializable]
    public struct CornerDecor
    {
        public Sprite sprite;
        public Vector2 positionOffset;
        public Quaternion rotationOffset;
    }
    [Tooltip("List of different corner dectorations")]
    public List<CornerDecor> cornerDecorList;

    public Sprite FindCornerDecorSprite(string target)
    {
        CornerDecor decor = cornerDecorList.Find(x => x.sprite.name == target);
        return decor.sprite;
    }

    public Sprite FindCornerDecorSprite(Sprite sprite)
    {
        CornerDecor decor = cornerDecorList.Find(x => x.sprite == sprite);
        return decor.sprite;
    }

    public (Vector2, Quaternion) GetCornerDecorOffsets(Sprite sprite)
    {
        CornerDecor decor = cornerDecorList.Find(x => x.sprite == sprite);
        return (decor.positionOffset, decor.rotationOffset);
    }
}
