using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportCharacterTransition : MonoBehaviour, ICharacterTransition
{
    private VN_Character _character;
    private RectTransform _rectTransform;
    private CharacterData _characterData;

    public void Construct(VN_Character character)
    {
        _character = character;
        _rectTransform = character.rectTransform;
        _characterData = character.data;
    }

    public IEnumerator Co_ExitScreen()
    {
        Vector2 endPosition = VN_HelperFunctions.GetTransitionTarget(
            _character, CharacterData.TransitionDirection.exit);

        yield return StartCoroutine(Co_TeleportTransition(endPosition));
        _character.ChangeSprite("");
        _character.SetData(null);
        yield break;
    }

    public IEnumerator Co_EnterScreen()
    {
        Vector2 endPosition = VN_HelperFunctions.GetTransitionTarget(
            _character, CharacterData.TransitionDirection.enter);

        _character.ChangeSprite(_characterData.defaultSpriteName);
        yield return StartCoroutine(Co_TeleportTransition(endPosition));
        yield break;
    }

    IEnumerator Co_TeleportTransition(Vector2 endPosition)
    {
        _rectTransform.anchoredPosition = endPosition;
        yield break;
    }
}
