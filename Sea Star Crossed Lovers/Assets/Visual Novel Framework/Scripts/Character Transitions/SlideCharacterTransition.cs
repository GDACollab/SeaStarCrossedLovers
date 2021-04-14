using System.Collections;
using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "SlideCharacterTransition", menuName = "ScriptableObjects/SlideCharacterTransition")]
public class SlideCharacterTransition : CharacterTransition
{

    public override IEnumerator Co_EnterScreen(VN_Character character, MonoBehaviour caller)
    {
        Vector2 endPosition = VN_Util.GetTransitionTarget(
            character, CharacterData.TransitionDirection.enter);

        yield return caller.StartCoroutine(Co_Move(character, endPosition, Ease.OutSine));
    }

    public override IEnumerator Co_ExitScreen(VN_Character character, MonoBehaviour caller)
    {
        Vector2 endPosition = VN_Util.GetTransitionTarget(
            character, CharacterData.TransitionDirection.exit);

        yield return caller.StartCoroutine(Co_Move(character, endPosition, Ease.InSine));
    }

    IEnumerator Co_Move(VN_Character character, Vector2 endPosition, Ease ease)
    {
        bool waitingForComplete = true;
        character.rectTransform.DOAnchorPos(endPosition, character.data.transitionDuration)
            .OnComplete(() => waitingForComplete = false)
            .SetEase(ease);

        yield return new WaitUntil(() => waitingForComplete == false);
    }

}
