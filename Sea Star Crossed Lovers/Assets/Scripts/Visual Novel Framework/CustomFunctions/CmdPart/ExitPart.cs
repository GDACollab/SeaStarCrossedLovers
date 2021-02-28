using System.Collections;
using UnityEngine;

public class ExitPart : MonoBehaviour, ICmdPart
{
	public IEnumerator CmdPart(VN_Character charObj, CharacterData characterData)
	{
		if (charObj.data == characterData)
		{
			yield return StartCoroutine(charObj.Co_CharacterTransition());
			//yield return StartCoroutine(charObj.data.transition.Co_ExitScreen(charObj, this));
			charObj.ChangeSprite("");
			charObj.SetData(null);
		}
	}
}
