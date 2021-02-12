using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VN_Character : MonoBehaviour
{
    // Settings
    public int ScreenEdgeDistance;

    [SerializeField]
    [Tooltip("Character's name")]
    private string Name = "";
    [System.Serializable]
    public struct CharacterSprites
    {
        public string name;
        public Sprite image;
    }
    [Tooltip("List of character's sprites and their names")]
    public List<CharacterSprites> Sprites;

    public enum ScreenSide { left, right };
    [SerializeField]
    public ScreenSide Side;

    private Image currentImage;

    // Start is called before the first frame update
    void Start()
    {
        currentImage = gameObject.GetComponent<Image>();
    }

    void changeSprite(string newSpriteName)
    {
        // Find sprite name from Sprites
        Sprite newSprite = Sprites.Find(x => x.name == newSpriteName).image;
        // If found, change image to newSprite
        if (newSprite) currentImage.sprite = newSprite;
        else Debug.LogError("VN_Character.changeSprite invalid newSpriteName");
    }
}