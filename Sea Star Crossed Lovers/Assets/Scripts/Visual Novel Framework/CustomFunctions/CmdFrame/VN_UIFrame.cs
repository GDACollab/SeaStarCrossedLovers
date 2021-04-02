using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VN_UIFrame : MonoBehaviour, ICmdFrame
{
	static VN_Manager _manager;

	public void Construct(VN_Manager manager)
	{
		_manager = manager;
	}

	public IEnumerator CmdFrame(List<string> args, ICmdPart part, bool isImmediate)
	{
		if (args.Count != 1)
		{
			Debug.LogError("Args error");
			yield break;
		}

	}
}
