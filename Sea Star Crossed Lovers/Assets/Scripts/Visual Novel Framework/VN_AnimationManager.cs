using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VN_AnimationManager : MonoBehaviour
{
    public Animator TextboxAnimator;

    private VN_Manager _manager;

    // TODO ? Make VN_Characters add to this dictionary when they awake
    // instead of getting all from manager
    public Dictionary<VN_Character, Animator> CharacterAnimators =
        new Dictionary<VN_Character, Animator>();

    public VN_Character testCharacter;

    public void Construct(VN_Manager manager)
    {
        _manager = manager;

        _manager.CharacterObjects.ForEach(character => {
            CharacterAnimators.Add(character,
                character.GetComponent<Animator>());
        });

        switch (_manager.activeState)
        {
            case VN_Manager.ActiveState.hidden:
                TextboxAnimator.SetBool("OnScreen", false);
                break;
            case VN_Manager.ActiveState.active:
                TextboxAnimator.SetBool("OnScreen", true);
                break;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(Co_ToggleBoolState(TextboxAnimator, "OnScreen"));
        }
    }

    // TODO What is the actual role of this class?

    public static IEnumerator Co_ToggleBoolState(Animator animator, string boolName)
    {
        // TODO replace first 1 with actual duration of animation
        float waitTime = 1 * (1 * (1 / animator.speed));

        if (!animator.enabled)
        {
            animator.enabled = true;
        }
        bool thisBool = animator.GetBool(boolName);
        if (thisBool)
        {
            animator.SetBool(boolName, false);
        }
        else
        {
            animator.SetBool(boolName, true);
        }

        yield return new WaitForSeconds(waitTime);
    }
}
