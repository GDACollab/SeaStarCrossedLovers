using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "SlideCharacterTransition", menuName = "ScriptableObjects/SlideCharacterTransition")]
public class SlideCharacterTransition : CharacterTransition
{
    public override IEnumerator Co_ExitScreen(VN_Character character, MonoBehaviour caller)
    {
        Vector2 initialPosition = character.rectTransform.anchoredPosition;
        Vector2 endPosition = VN_Util.GetTransitionTarget(
            character, CharacterData.TransitionDirection.exit);

        yield return caller.StartCoroutine(Co_SlideTransition(character, initialPosition, endPosition));
    }

    public override IEnumerator Co_EnterScreen(VN_Character character, MonoBehaviour caller)
    {
        Vector2 initialPosition = character.rectTransform.anchoredPosition;
        Vector2 endPosition = VN_Util.GetTransitionTarget(
            character, CharacterData.TransitionDirection.enter);

        yield return caller.StartCoroutine(Co_SlideTransition(character, initialPosition, endPosition));
    }

    IEnumerator Co_SlideTransition(VN_Character character, Vector2 initialPosition, Vector2 endPosition)
    {
        // From lerp tutorial: https://gamedevbeginner.com/the-right-way-to-lerp-in-unity-with-examples/
        float time = 0;

        var wait = new WaitForEndOfFrame();

        while (time < character.data.transitionDuration)
        {
            time += Time.deltaTime;
            float t = time / character.data.transitionDuration;
            // "smootherstep" https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro/
            t = t * t * t * (t * (6f * t - 15f) + 10f);
            character.rectTransform.anchoredPosition = Vector2.Lerp(initialPosition, endPosition, t);

            yield return wait;
        }
        // Set postion to end in case Lerp isn't exact
        character.rectTransform.anchoredPosition = endPosition;

        yield break;
    }
}
