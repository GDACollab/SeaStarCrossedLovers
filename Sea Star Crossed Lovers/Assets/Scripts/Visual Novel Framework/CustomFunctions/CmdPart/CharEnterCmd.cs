using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharEnterCmd : MonoBehaviour, ICommandCall
{
	public IEnumerator Command(List<string> args, MonoBehaviour caller, bool isImmediate)
	{
        foreach(string arg in args)
        {
            CharacterData data = VN_Util.FindCharacterData(arg);
            VN_Character charObj = VN_Util.FindEmptyCharObj(data);

            charObj.SetData(data);
            charObj.ChangeSprite(data.defaultSprite);
            if (isImmediate)
            {
                caller.StartCoroutine(charObj.data.transition.Co_EnterScreen(charObj, caller));
            }
            else
            {
                yield return caller.StartCoroutine(charObj.data.transition.Co_EnterScreen(charObj, caller));
            }
        }
    }
}
