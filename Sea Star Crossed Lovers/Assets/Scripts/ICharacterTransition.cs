using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterTransition
{
    IEnumerator EnterScreen(RectTransform rectTransform, CharacterData.MoveTransition transition, CharacterData data);

    IEnumerator ExitScreen(RectTransform rectTransform, CharacterData.MoveTransition transition);
}