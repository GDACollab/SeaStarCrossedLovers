using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharExitCmd : MonoBehaviour, ICommandCall
{
	public IEnumerator Command(List<string> args, MonoBehaviour caller, bool isImmediate)
	{
        if(args.Count != 1)
        {
            Debug.LogError("args error: " + this);
            yield break;
        }

		CharacterData data = VN_Util.FindCharacterData(args[0]);
		VN_Character charObj = VN_Util.FindCharacterObj(data);

		if (isImmediate)
        {
            caller.StartCoroutine(charObj.data.transition.Co_ExitScreen(charObj, caller));
        }
        else
        {
            yield return caller.StartCoroutine(charObj.data.transition.Co_ExitScreen(charObj, caller));
        }
        charObj.ChangeSprite("");
        charObj.SetData(null);
    }
}
