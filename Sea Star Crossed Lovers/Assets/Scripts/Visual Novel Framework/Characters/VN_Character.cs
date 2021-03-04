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
            VN_CharBox.sprite = data.characterBox;
            ScaleCanvasImage(VN_CharBox, _manager.characterBoxScale);
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
                VN_CharSprite.sprite = newSprite;
                //ScaleCanvasImage(VN_CharSprite, _manager.characterSpriteScale);
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
                VN_CharSprite.sprite = foundSprite;
                ScaleCanvasImage(VN_CharSprite, _manager.characterSpriteScale);
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

    /* TODO replace this somehow, very jank since continually scales canvas everytime
     * it is called when it should scale down to native size and then apply scale.
    */
    private void ScaleCanvasImage(Image image, float scale)
    {
        Vector2 orignalSize = image.rectTransform.sizeDelta;
        image.SetNativeSize();
        image.rectTransform.sizeDelta = new Vector2(orignalSize.x * scale, orignalSize.y * scale);
    }
}