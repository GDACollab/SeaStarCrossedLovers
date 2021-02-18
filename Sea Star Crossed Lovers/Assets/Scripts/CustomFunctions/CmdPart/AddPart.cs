using System.Collections;
using UnityEngine;

public class AddPart : MonoBehaviour, ICmdPart
{
	public IEnumerator CmdPart(VN_Character charObj, CharacterData characterData)
    {
		if (charObj.data == null)
		{
			charObj.SetData(characterData);
			charObj.ChangeSprite(characterData.defaultSpriteName);
			yield return StartCoroutine(charObj._characterTransition.Co_EnterScreen());
		}
	}
}
