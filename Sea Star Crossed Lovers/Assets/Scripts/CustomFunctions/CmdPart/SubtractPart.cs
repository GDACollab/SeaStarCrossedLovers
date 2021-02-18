using System.Collections;
using UnityEngine;

public class SubtractPart : MonoBehaviour, ICmdPart
{
	public IEnumerator CmdPart(VN_Character charObj, CharacterData characterData)
	{
		if (charObj.data == characterData)
		{
			yield return StartCoroutine(charObj._characterTransition.Co_ExitScreen());
			charObj.ChangeSprite("");
			charObj.SetData(null);
		}
	}
}
