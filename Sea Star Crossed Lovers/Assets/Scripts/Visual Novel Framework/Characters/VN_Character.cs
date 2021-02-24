using UnityEngine;
using UnityEngine.UI;

public class VN_Character : MonoBehaviour
{
    public CharacterData data;
    public Image currentImage;
    public RectTransform rectTransform;
    // Debug
    [SerializeField] private Text nameText;

    /* TODO Make VN_Character GameObject move to a designated starting location
     * and store the Vector2 initialLocation as a public field to use in resetting
     * and in tranisitions where the funciton needs initial location
    */ 
    void Awake()
    {
        currentImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }

    /* TODO Try to follow SOLID for this class
     * - Make interfaces for SetData and ChangeSprite? 
     * ... Maybe not now that this class is smaller
    */

    // Updates this VN_Character's data – it's CharacterTransition, its anchors, and debug nameText
    public void SetData(CharacterData toSetData)
    {
        data = toSetData;
        if (toSetData)
        {
            // Debug nametag
            nameText.text = toSetData.name;
            switch (toSetData.screenSide)
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
        if(newSpriteName == null || newSpriteName == "")
        {
            currentImage.sprite = null;
            currentImage.enabled = false;
            return;
        }
        if(data)
        {
            // Find sprite name from Sprites
            Sprite newSprite = data.characterSprites.Find(
                x => x.spriteName == newSpriteName).emotionSprite;
            // If found, change image to newSprite
            if (newSprite)
            {
                currentImage.enabled = true;
                currentImage.sprite = newSprite;
            }
            else
            {
                currentImage.sprite = null;
            }
        }
        else
        {
            Debug.LogError("Cannot ChangeSprite when VN_Character has null Data");
        }
    }
}