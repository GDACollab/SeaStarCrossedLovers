using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportCharacterTransition : MonoBehaviour, ICharacterTransition
{
    private VN_Character _character;
    private RectTransform _rectTransform;

    public void Construct(VN_Character character)
    {
        _character = character;
        _rectTransform = character.rectTransform;
    }

    public IEnumerator Co_ExitScreen()
    {
        Vector2 endPosition = VN_Util.GetTransitionTarget(
            _character, CharacterData.TransitionDirection.exit);

        yield return StartCoroutine(Co_TeleportTransition(endPosition));
    }

    public IEnumerator Co_EnterScreen()
    {
        Vector2 endPosition = VN_Util.GetTransitionTarget(
            _character, CharacterData.TransitionDirection.enter);

        yield return StartCoroutine(Co_TeleportTransition(endPosition));
    }

    IEnumerator Co_TeleportTransition(Vector2 endPosition)
    {
        _rectTransform.anchoredPosition = endPosition;
        yield break;
    }
}
