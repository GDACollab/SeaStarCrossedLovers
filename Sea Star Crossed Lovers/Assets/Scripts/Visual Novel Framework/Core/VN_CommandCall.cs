using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VN_CommandCall : MonoBehaviour
{
	private VN_Manager _manager;

	// Inky custom command calling
	const string FunctionCallString = ">>>";
	const char ArgumentDelimiter = '.';
	// Store/call funcitons in a dictionary https://stackoverflow.com/questions/4233536/c-sharp-store-functions-in-a-dictionary
	Dictionary<string, Delegate> AllCommands =
		new Dictionary<string, Delegate>();
	private List<ICmdCall> commandCalls;
	private List<ICmdFrame> commandFrames;
	private List<ICmdPart> commandParts;

	/* ICmdFrame implementing classes must be named in the form
	 * "[Frame name]Frame"
	 * 
	 * ICmdPart implementing classes must be named in the form
	 * "[Inky custom command name]Part"
	 * 
	 * ICmdCall implementing classes must be named in the form
	 * "[ICmdFrame class name - 'Frame']_[ICmdPart class name - 'Part']_Cmd"
	 * 
	 * Example: Ink Command Call "Add.CharacterA.CharacterB" uses...
	 *		ICmdFrame called "MultiFrame" since it takes multiple character name args 
	 *		ICmdPart called "AddPart" since it sets a VN_Character with null data
	 *		to have the CharacterData of matching the character name and transitions
	 *		the VN_Character onto the screen
	 *		ICmdCall called "Multi_Add_Cmd" to combine the functionality of the MultiFrame
	 *		with the AddPart. Underscores are used to parse the command in order to construct
	 *		the class itself and add it to the AllCommands dictionary
	*/

	public void Construct(VN_Manager VN_Manager)
    {
		_manager = VN_Manager;

		// TODO Replace getting interface implementers with something
		// more efficent
		/* TODO Make ICmdCalls not implement MonoBehaviour (because they don't need it)
		*/
		commandCalls = FindObjectsOfType<MonoBehaviour>()
			.OfType<ICmdCall>().ToList();

		commandFrames = FindObjectsOfType<MonoBehaviour>()
			.OfType<ICmdFrame>().ToList();

		commandParts = FindObjectsOfType<MonoBehaviour>()
			.OfType<ICmdPart>().ToList();

		// TODO Make a class for initializing everything for command calls
		commandCalls.ForEach(command =>
		{
			string commandString = command.GetType().ToString();
			string[] parsedCommand = commandString.Split('_');

			string frameString = parsedCommand[0];
			string partString = parsedCommand[1];

			ICmdFrame newFrame = commandFrames.Find(frame => {
				string thisFrameString = VN_Util.RemoveSubstring(frame.GetType().ToString(), "Frame");
				return thisFrameString == frameString;
			});
			if (newFrame == null)
			{
				Debug.LogError("Couldn't find ICmdFrame \"" + frameString + "\"");
			}

			ICmdPart newPart = commandParts.Find(part => {
				string thisPartString = VN_Util.RemoveSubstring(part.GetType().ToString(), "Part");
				return thisPartString == partString;
			});
			if (newPart == null)
			{
				Debug.LogError("Couldn't find ICmdPart \"" + partString + "\"");
			}

			command.Construct(_manager, newFrame, newPart);
			Func<List<string>, IEnumerator> newCommand = command.Command;

			AllCommands.Add(partString, newCommand);
		});
	}

	public IEnumerator TryCommand(string line)
	{
		if (line.Length > 3 && line.Substring(0, 3) == FunctionCallString)
		{
			// (char)44 = ,
			string[] commands = line.Substring(3).Split((char)44);

			// Try to run all commands
			foreach (string rawCommand in commands)
			{
				var command = rawCommand.Trim(VN_Util.toTrim);
				// Assumes command is in form [function][ArgumentDelimiter][argument]
				// with only 1 argument
				string[] commandArray = command.Split(ArgumentDelimiter);
				List<string> commandList = new List<string>(commandArray);
				string function = commandList[0].Trim();
				// Assume all other strings after first (the function) are arguments
				List<string> arguments = commandList.GetRange(1, commandList.Count - 1);
				arguments.ForEach(arg => arg = arg.Trim(VN_Util.toTrim));

				if (AllCommands.ContainsKey(function))
				{
					Func<List<string>, IEnumerator> Co_Command =
						(Func<List<string>, IEnumerator>)AllCommands[function];

					yield return StartCoroutine(Co_Command(arguments));
				}
				else
				{
					Debug.LogError("AllCommands doesn't contain key \"" + function + "\"");
				}
			}
		}
	}
}
