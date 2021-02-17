using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportCharacterTransition : MonoBehaviour
{
    private VN_Character _character;
    private RectTransform _rectTransform;

    private void Construct(VN_Character character)
    {
        _character = character;
        _rectTransform = character.rectTransform;
    }

    //IEnumerator EnterScreen(RectTransform _rectTransform, CharacterData.MoveTransition transition, CharacterData data)
    //{
    //    yield return StartCoroutine(Co_EnterScreen(transition, data));
    //}

    //IEnumerator ExitScreen(RectTransform _rectTransform, CharacterData.MoveTransition transition)
    //{
    //    yield return StartCoroutine(Co_ExitScreen(transition));
    //}

    //public IEnumerator Co_ExitScreen(CharacterData.MoveTransition transition)
    //{
    //    yield return StartCoroutine(Co_Transition(transition, CharacterData.TransitionDirection.exit));
    //    _character.ChangeSprite("");
    //    _character.SetData(null);
    //    yield break;
    //}

    //public IEnumerator Co_EnterScreen(CharacterData.MoveTransition transition, CharacterData data)
    //{
    //    _character.SetData(data);
    //    _character.ChangeSprite(data.defaultSpriteName);
    //    StartCoroutine(Co_Transition(transition, CharacterData.TransitionDirection.enter));
    //    yield break;
    //}

    //IEnumerator Co_Transition(CharacterData.MoveTransition transition, CharacterData.TransitionDirection direction)
    //{
    //    Vector2 initialPosition = _rectTransform.anchoredPosition;
    //    Vector2 endPosition = _character.GetTargetPosition(direction);

    //    switch (transition)
    //    {
    //        case CharacterData.MoveTransition.teleport:
    //            yield return StartCoroutine(Co_TeleportTransition(endPosition));
    //            yield break;
    //        case CharacterData.MoveTransition.slide:
    //            yield return StartCoroutine(Co_SlideTransition(initialPosition, endPosition));
    //            yield break;
    //    }
    //}

    //IEnumerator Co_TeleportTransition(Vector2 endPosition)
    //{
    //    rectTransform.anchoredPosition = endPosition;
    //    yield break;
    //}
}
