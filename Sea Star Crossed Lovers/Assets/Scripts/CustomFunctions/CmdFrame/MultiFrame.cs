using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiFrame : MonoBehaviour, ICmdFrame
{
	static VN_Manager _manager;

	public void Construct(VN_Manager manager)
	{
		_manager = manager;
	}

	public IEnumerator CmdFrame(List<string> args, ICmdPart part)
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
				IEnumerator cmdPart = part.CmdPart(charObj, characterData);
				// Returns true if reached a yield, false if not
				bool result = cmdPart.MoveNext();
				if(result) {
					// If reached the yield, yield return it here
					yield return cmdPart.Current;
					cmdPart.MoveNext();
					// break out of for loop because found valid VN_Character to add
					break;
				}
			}
		}
	}
}
