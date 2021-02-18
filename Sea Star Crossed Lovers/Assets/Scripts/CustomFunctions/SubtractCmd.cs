using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubtractCmd : MonoBehaviour
	//, ICmdCall
{
	static VN_Manager _manager;

	public void Construct(VN_Manager manager)
	{
		_manager = manager;
	}

	/**
	* Removes a specified VN_Character from CharacterObjects
	*
	* @param characterName: the character whom we are attempting to remove
	* @return: whether or not the character could be removed
	*/
	public IEnumerator Command(List<string> args)
    {
		if (args.Count == 0)
		{
			Debug.LogError("Args error");
			yield break;
		}

		
		foreach (string name in args)
		{
			bool thisNameResult = false;

			CharacterData characterData = VN_Util.FindCharacterData(name);

			// Search for VN_Character with no data
			// Assuming any with no data is offscreen
			foreach (VN_Character charObj in _manager.CharacterObjects)
			{
				if (charObj.data == characterData)
				{
					yield return StartCoroutine(charObj._characterTransition.Co_ExitScreen());
					charObj.ChangeSprite("");
					charObj.SetData(null);

					thisNameResult = true;
					break;
				}
			}

			if (!thisNameResult)
			{
				Debug.LogError("No CharacterObjects with data of " + name);
			}
		}
	}
}
