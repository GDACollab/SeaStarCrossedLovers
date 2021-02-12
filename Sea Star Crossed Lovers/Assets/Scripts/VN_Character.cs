using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VN_Character : MonoBehaviour
{
    // Settings
    public int ScreenEdgeDistance;

    [System.Serializable]
    public struct CharacterSprites
    {
        public string name;
        public Sprite image;
    }
    [Tooltip("List of character's sprites and their names")]
    public List<CharacterSprites> Sprites;

    public enum ScreenSide { left, right };
    [Tooltip("Position of character on the screen")]
    public ScreenSide Side;

    public enum MoveTransition { teleport, slide };
    [Tooltip("Style of sprite movement when entering/exiting the screen")]
    public MoveTransition transition;



    // Internal
    private Image currentImage;
    RectTransform rectTransform;

    Vector3 position;

    // Start is called before the first frame update
    void Start()
    {
        currentImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();

        // Make sure character has default sprite and starts with it
        changeSprite("default");
    }

    public void changeSprite(string newSpriteName)
    {
        // Find sprite name from Sprites
        Sprite newSprite = Sprites.Find(x => x.name == newSpriteName).image;
        // If found, change image to newSprite
        if (newSprite) currentImage.sprite = newSprite;
        else Debug.LogError("VN_Character.changeSprite invalid newSpriteName");
    }

    public void enter(MoveTransition transition)
    {
        switch (transition)
        {
            case MoveTransition.teleport:
                teleportTransition();
                break;
            case MoveTransition.slide:
                slideTransition();
                break;
        }
    }

    void teleportTransition()
    {
        //Debug.Log("teleportTransition");
        switch (Side)
        {
            case ScreenSide.left:
                //position.x = ScreenEdgeDistance;
                //Debug.Log("ScreenSide.left " + position.x.ToString());

                //Debug.Log("ScreenSide.left " + rectTransform.anchoredPosition.ToString());
                break;
            case ScreenSide.right:
                //rectTransform.anchoredPosition.Set(Screen.width - ScreenEdgeDistance, 0);
                //Debug.Log("ScreenSide.right " + rectTransform.anchoredPosition.ToString());
                break;
        }
    }

    void slideTransition()
    {

        //light.intensity = Mathf.Lerp(light.intensity, 8f, 0.5f * Time.deltaTime);

        //gameObject.transform.position = Mathf.Lerp(gameObject.transform.position, );
    }


}