using UnityEngine;
using UnityEngine.UI;

public class VN_Character : MonoBehaviour
{
    public CharacterData data;

    private Image VN_CharBox;
    [SerializeField] private Image VN_CharSprite;

    [HideInInspector] public RectTransform rectTransform;
    // Debug
    [SerializeField] private Text nameText;

    private VN_Manager _manager;

    public void Construct(VN_Manager manager)
    {
        _manager = manager;
    }

    void Awake()
    {
        VN_CharBox = GetComponent<Image>();
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
            // Change character box
            ScaleImageCanvas(VN_CharBox, data.characterBox, _manager.characterBoxScale);
        }
        else
        {
            nameText.text = "None";
        }
    }

    public void ChangeSprite(string newSpriteName)
    {
        if (newSpriteName == null || newSpriteName == "")
        {
            VN_CharSprite.sprite = null;
            VN_CharSprite.enabled = false;
            return;
        }
        if(data)
        {
            Sprite newSprite = data.characterSprites.Find(x =>
            {
                string emotionName = x.name;
                var trimmed = emotionName.Split('_');
                if(trimmed.Length > 1)
                {
                    emotionName = trimmed[1];
                }
                
                return emotionName == newSpriteName;
            });
            // If found, change image to newSprite
            if (newSprite)
            {
                VN_CharSprite.enabled = true;
                ScaleImageCanvas(VN_CharSprite, newSprite, _manager.characterSpriteScale);
            }
            else
            {
                Debug.LogError("Couldn't find sprite name \"" + newSpriteName + "\"" +
                    " in characterData \"" + data.name + "\"");
                VN_CharSprite.sprite = data.defaultSprite;
            }
        }
        else
        {
            Debug.LogError("Cannot ChangeSprite when VN_Character has null Data");
        }
    }

    public void ChangeSprite(Sprite newSprite)
    {
        if (newSprite == null)
        {
            VN_CharSprite.sprite = null;
            VN_CharSprite.enabled = false;
            return;
        }
        if (data)
        {
            Sprite foundSprite = data.characterSprites.Find(x =>
            {
                 return x == newSprite;
            });
            // If found, change image to newSprite
            if (foundSprite)
            {
                VN_CharSprite.enabled = true;
                ScaleImageCanvas(VN_CharSprite, foundSprite, _manager.characterSpriteScale);
            }
            else
            {
                Debug.LogError("Couldn't find sprite name \"" + newSprite.name + "\"" +
                    " in characterData \"" + data.name + "\"");
                VN_CharSprite.sprite = data.defaultSprite;
            }
        }
        else
        {
            Debug.LogError("Cannot ChangeSprite when VN_Character has null Data");
        }
    }

    /* TODO replace this somehow, very jank since forces the image canvas to the size of
     * the image in pixels. Pixel size varies so every character and character box looks
     * different.
    */
    private void ScaleImageCanvas(Image image, Sprite sprite, float scale)
    {
        if(!image)
        {
            Debug.LogError("Cannot ScaleImageCanvas of null image");
            return;
        }
        if (!sprite)
        {
            Debug.LogError("Cannot ScaleImageCanvas of null sprite");
            return;
        }

        Vector2 spriteSize = sprite.rect.size;
        image.rectTransform.sizeDelta = new Vector2(spriteSize.x * scale, spriteSize.y * scale);
        image.sprite = sprite;
    }
}