using System.Collections;
using UnityEngine;

public class EnterPart : MonoBehaviour, ICmdPart
{
	public IEnumerator CmdPart(VN_Character charObj, CharacterData characterData)
    {
		if (charObj.data == null)
		{
			charObj.SetData(characterData);
			charObj.ChangeSprite(characterData.defaultSprite);
			yield return StartCoroutine(charObj.data.transition.Co_EnterScreen(charObj, this));
		}
	}
}
