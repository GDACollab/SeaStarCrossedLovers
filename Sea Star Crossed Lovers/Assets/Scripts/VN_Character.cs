using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class VN_Character : MonoBehaviour
{
    public CharacterData data;
    public Image currentImage;
    public RectTransform rectTransform;

    public ICharacterTransition _characterTransition;

    private List<ICharacterTransition> characterTransitions;

    // Debug
    [SerializeField] private Text nameText;

    void Awake()
    {
        currentImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();

        // Must be in the scene to be found
        // Put them into an empty game object as components
        characterTransitions = FindObjectsOfType<MonoBehaviour>()
            .OfType<ICharacterTransition>().ToList();
    }

    /* TODO Try to follow SOLID for this class
     * - Somehow get and bind the transition with an interface so that
     * this class delegates the actual transitioning to another class
     * - Make interfaces for SetData and ChangeSprite? 
    */

    public void SetData(CharacterData toSetData)
    {
        data = toSetData;
        if (toSetData)
        {
            UpdateCharacterTransition();

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

    /* Get the ICharacterTransition for the current data by trying to match data's
     * moveTransition string to the name of a class type that extends ICharacterTransition
    */
    private void UpdateCharacterTransition()
    {
        characterTransitions.ForEach(transition =>
        {
            if (transition.GetType().ToString() == data.moveTransition)
            {
                _characterTransition = transition;
                _characterTransition.Construct(this);

                print("transition: " + transition.GetType().ToString());
            }
        });
    }

    public void ChangeSprite(string newSpriteName)
    {
        if(data)
        {
            // Find sprite name from Sprites
            Sprite newSprite = data.characterSprites.Find(
                x => x.spriteName == newSpriteName).emotionSprite;
            // If found, change image to newSprite
            if (newSprite) currentImage.sprite = newSprite;
            else
            {
                currentImage.sprite = null;
                //Debug.LogError("VN_Character.ChangeSprite invalid newSpriteName");
            }
        }
        else
        {
            Debug.LogError("Cannot ChangeSprite when VN_Character has null Data");
        }
    }

    #region Transition Functions


    public void AddCharacter(CharacterData characterData)
    {
        print("AddCharacter: " + characterData.name);
        SetData(characterData);
        ChangeSprite(characterData.defaultSpriteName);
        StartCoroutine(_characterTransition.Co_EnterScreen());
    }

    /* TODO Make all custom inky function calls implement an interface with
     * one method, process, that returns IEnumerator
     * Subtract character won't work as a void since it needs to wait for
     * the transition to be done before nulling the VN_Character.data
    */

    public IEnumerator Co_AddCharacter(CharacterData characterData)
    {
        SetData(characterData);
        yield return StartCoroutine(_characterTransition.Co_EnterScreen());
        ChangeSprite(characterData.defaultSpriteName);
    }

    public IEnumerator Co_SubtractCharacter()
    {
        yield return StartCoroutine(_characterTransition.Co_EnterScreen());
        ChangeSprite("");
        SetData(null);
    }

    //public IEnumerator Co_ExitScreen(CharacterData.MoveTransition transition)
    //{
    //    yield return StartCoroutine(Co_Transition(transition, CharacterData.TransitionDirection.exit));
    //    ChangeSprite("default");
    //    SetData(null);
    //    yield break;
    //}

    //public IEnumerator Co_EnterScreen(CharacterData.MoveTransition transition, CharacterData data)
    //{
    //    SetData(data);
    //    ChangeSprite(data.defaultSpriteName);
    //    StartCoroutine(Co_Transition(transition, CharacterData.TransitionDirection.enter));
    //    yield break;
    //}

    public void StopCoroutines()
    {
        StopAllCoroutines();
    }

    #endregion
}