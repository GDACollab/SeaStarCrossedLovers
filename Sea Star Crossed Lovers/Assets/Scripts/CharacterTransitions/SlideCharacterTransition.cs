using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideCharacterTransition : MonoBehaviour, ICharacterTransition
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
        Vector2 initialPosition = _rectTransform.anchoredPosition;
        Vector2 endPosition = VN_HelperFunctions.GetTransitionTarget(
            _character, CharacterData.TransitionDirection.exit);

        yield return StartCoroutine(Co_SlideTransition(initialPosition, endPosition));
    }

    public IEnumerator Co_EnterScreen()
    {
        Vector2 initialPosition = _rectTransform.anchoredPosition;
        Vector2 endPosition = VN_HelperFunctions.GetTransitionTarget(
            _character, CharacterData.TransitionDirection.enter);

        yield return StartCoroutine(Co_SlideTransition(initialPosition, endPosition));
    }

    IEnumerator Co_SlideTransition(Vector2 initialPosition, Vector2 endPosition)
    {
        // From lerp tutorial: https://gamedevbeginner.com/the-right-way-to-lerp-in-unity-with-examples/
        float time = 0;

        var wait = new WaitForEndOfFrame();

        while (time < _characterData.transitionDuration)
        {
            time += Time.deltaTime;
            float t = time / _characterData.transitionDuration;
            // "smootherstep" https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro/
            t = t * t * t * (t * (6f * t - 15f) + 10f);
            _rectTransform.anchoredPosition = Vector2.Lerp(initialPosition, endPosition, t);

            yield return wait;
        }
        // Set postion to end in case Lerp isn't exact
        _rectTransform.anchoredPosition = endPosition;

        yield break;
    }
}
