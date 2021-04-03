using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharExitCmd : MonoBehaviour, ICommandCall
{
	public IEnumerator Command(List<string> args, bool isImmediate)
	{
        foreach (string arg in args)
        {

            CharacterData data = VN_Util.FindCharacterData(arg);
            VN_Character charObj = VN_Util.FindCharacterObj(data);

            yield return StartCoroutine(charObj.data.transition.Co_ExitScreen(charObj, this));
            charObj.ChangeSprite("");
            charObj.SetData(null);
        }
    }
}
