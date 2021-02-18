using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddCommand : MonoBehaviour, ICommandCall
{
	static VN_Manager _manager;

	public void Construct(VN_Manager manager)
	{
		_manager = manager;
	}

	public IEnumerator Command(List<string> args)
    {
		if (args.Count == 0)
		{
			Debug.LogError("Args error");
			yield break;
		}

		foreach (string name in args)
		{
			print("adding name: " + name);

			bool thisNameResult = false;

			CharacterData characterData = VN_HelperFunctions.FindCharacterData(name);

			// Search for VN_Character with no data
			// Assuming any with no data is offscreen
			foreach (VN_Character charObj in _manager.CharacterObjects)
			{
				if (charObj.data == null)
				{
					// If found, enter screen

					charObj.AddCharacter(characterData);
					thisNameResult = true;
					break;
				}
			}

			if (!thisNameResult)
			{
				Debug.LogError("There is no empty VN_Character to give " + name + " in CharacterObjects");
			}
		}
	}
}
