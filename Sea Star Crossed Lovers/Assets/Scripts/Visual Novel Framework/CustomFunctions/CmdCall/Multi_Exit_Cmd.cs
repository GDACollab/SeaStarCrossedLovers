using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Multi_Exit_Cmd : MonoBehaviour, ICmdCall
{
	private VN_Manager _manager;
	private ICmdFrame _cmdFrame;
	private ICmdPart _cmdPart;

	public void Construct(VN_Manager manager, ICmdFrame cmdFrame, ICmdPart cmdPart)
	{
		_manager = manager;
		_cmdFrame = cmdFrame;
		_cmdPart = cmdPart;
	}

	public IEnumerator Command(List<string> args)
	{
		_cmdFrame.Construct(_manager);

		yield return _cmdFrame.CmdFrame(args, _cmdPart);
	}
}

