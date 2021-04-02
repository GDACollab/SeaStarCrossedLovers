using System.Collections;
using UnityEngine;

public class ExitPart : MonoBehaviour, ICmdPart
{
	public IEnumerator CmdPart(VN_Character charObj, CharacterData characterData, bool isImmediate)
	{
		if (charObj.data == characterData)
		{
			if (isImmediate)
			{
				StartCoroutine(charObj.data.transition.Co_ExitScreen(charObj, this));
			}
			else
			{
				yield return StartCoroutine(charObj.data.transition.Co_ExitScreen(charObj, this));
			}
			charObj.ChangeSprite("");
			charObj.SetData(null);
		}
	}
}
