using System.Collections;
using UnityEngine;

public class EnterPart : MonoBehaviour, ICmdPart
{
	public IEnumerator CmdPart(VN_Character charObj, CharacterData characterData, bool isImmediate)
    {
		if (charObj.data == null)
		{
			charObj.SetData(characterData);
			charObj.ChangeSprite(characterData.defaultSprite);
			if (isImmediate)
            {
				StartCoroutine(charObj.data.transition.Co_EnterScreen(charObj, this));
			}
			else
            {
				yield return StartCoroutine(charObj.data.transition.Co_EnterScreen(charObj, this));
			}
		}
	}
}
