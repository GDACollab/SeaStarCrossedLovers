using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class VN_Character : MonoBehaviour
{
    public CharacterData data;

    public enum State { hidden, active };
    public State state = State.hidden;

    public enum ScenePosition { left, right };
    public ScenePosition scenePosition;

    public Image VN_CharBox;
    public Image VN_CharSprite;

    [HideInInspector] public RectTransform rectTransform;
    // Debug
    [SerializeField] private Text nameText;

    public VN_Manager manager;
    private VN_CharacterManager characterManager;

    public void Construct(VN_Manager manager, VN_CharacterManager characterManager)
    {
        this.manager = manager;
        this.characterManager = characterManager;

        VN_CharBox = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }

    // Updates this VN_Character's data – it's CharacterTransition, its anchors, and debug nameText
    public void SetData(CharacterData toSetData)
    {
        data = toSetData;
        if (toSetData)
        {
            // Debug nametag
            nameText.text = toSetData.name;
            switch (toSetData.scenePosition)
            {
                case CharacterData.ScenePosition.left:
                    // Change anchors and pivot to be on left middle of screen
                    // rectTransform.anchoredPosition of (0,0) means the Character's left edge will be on the left screen edge
                    rectTransform.pivot = new Vector2(0, 0.5f);
                    rectTransform.anchorMin = new Vector2(0, 0.5f);
                    rectTransform.anchorMax = new Vector2(0, 0.5f);
                    break;
                case CharacterData.ScenePosition.right:
                    // Change anchors and pivot to be on right middle of screen
                    // rectTransform.anchoredPosition of (0,0) means the Character's right edge will be on the right screen edge
                    rectTransform.pivot = new Vector2(1, 0.5f);
                    rectTransform.anchorMin = new Vector2(1, 0.5f);
                    rectTransform.anchorMax = new Vector2(1, 0.5f);
                    break;
            }
            // Change character box
            ScaleImageCanvas(VN_CharBox, data.characterBox, data.characterBoxScale);
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
                //var trimmed = emotionName.Split('_');
                //if (trimmed.Length > 1)
                //{
                //    emotionName = trimmed[1];
                //}

                return emotionName == newSpriteName;
            });
            // If found, change image to newSprite
            if (newSprite)
            {
                VN_CharSprite.enabled = true;
                ScaleImageCanvas(VN_CharSprite, newSprite, data.characterSpriteScale);
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
                ScaleImageCanvas(VN_CharSprite, foundSprite, data.characterSpriteScale);
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

        image.SetNativeSize();
        Vector2 spriteSize = sprite.rect.size;
        image.rectTransform.sizeDelta = new Vector2(spriteSize.x * scale, spriteSize.y * scale);
        image.sprite = sprite;
    }

    public IEnumerator TweenColor(Image image, Color color, float duration, Ease ease)
    {
        bool waitingForComplete = true;

        image.DOColor(color, duration)
            .OnComplete(() => waitingForComplete = false)
            .SetEase(ease);

        yield return new WaitUntil(() => waitingForComplete == false);
    }

    public IEnumerator TweenPosition(RectTransform rectTransform, Vector2 endPosition, float duration, Ease ease)
    {
        bool waitingForComplete = true;

        rectTransform.DOAnchorPos(endPosition, duration)
            .OnComplete(() => waitingForComplete = false)
            .SetEase(ease);

        yield return new WaitUntil(() => waitingForComplete == false);
    }
}