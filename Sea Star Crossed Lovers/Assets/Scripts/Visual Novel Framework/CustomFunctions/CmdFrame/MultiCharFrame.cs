using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiCharFrame : MonoBehaviour, ICmdFrame
{
	static VN_Manager _manager;

	public void Construct(VN_Manager manager)
	{
		_manager = manager;
	}

	public IEnumerator CmdFrame(List<string> args, ICmdPart part, bool isImmediate)
	{
		if (args.Count == 0)
		{
			Debug.LogError("Args error");
			yield break;
		}

		foreach (string name in args)
		{
			CharacterData characterData = VN_Util.FindCharacterData(name);

			foreach (VN_Character charObj in _manager.CharacterObjects)
			{
				IEnumerator cmdPart = part.CmdPart(charObj, characterData, isImmediate);
				// Returns true if reached a yield, false if not
				bool result = cmdPart.MoveNext();
				if(result) {
					// Yield return it here
					while (result)
					{
						yield return cmdPart.Current;
						result = cmdPart.MoveNext();
					}
					// break out of for loop because found valid VN_Character
					// to run part.CmdPart functionality
					break;
				}
			}
		}
	}
}
