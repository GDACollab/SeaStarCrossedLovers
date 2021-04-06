using System.Collections;
using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "SlideTextboxTransition", menuName = "ScriptableObjects/SlideTextboxTransition")]
public class SlideTextboxTransition : TextboxTransition
{
    public override IEnumerator Co_EnterScreen(VN_Manager manager, MonoBehaviour caller)
    {
        Vector2 endPosition = Vector2.zero;
        yield return caller.StartCoroutine(Co_Move(manager, endPosition, Ease.OutSine));
    }

    public override IEnumerator Co_ExitScreen(VN_Manager manager, MonoBehaviour caller)
    {
        RectTransform textbox = manager.textboxRectTransform;
        Vector2 endPosition = new Vector2 (0, -textbox.sizeDelta.y);
        yield return caller.StartCoroutine(Co_Move(manager, endPosition, Ease.InSine));
    }

    IEnumerator Co_Move(VN_Manager manager, Vector2 endPosition, Ease ease)
    {
        bool waitingForComplete = true;

        RectTransform textbox = manager.textboxRectTransform;
        textbox.DOAnchorPos(endPosition, manager.textboxTransitionDuration)
            .OnComplete(() => waitingForComplete = false)
            .SetEase(ease);

        yield return new WaitUntil(() => waitingForComplete == false);
    }
}
