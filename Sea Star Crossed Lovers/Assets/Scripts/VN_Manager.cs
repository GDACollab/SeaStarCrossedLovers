using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using System.Linq;

/// <summary>
/// Creates and manages VN components and flow
/// Based off of BasicInkExample.cs in Plugins/Ink/Example/Scripts
/// </summary>
public class VN_Manager : MonoBehaviour
{
	// General settings for the textbox and appearance of text
	[Header("Settings")]
	[Tooltip("Speed of slow text in characters per second")]
	public float TextSpeed = 60;
	[Tooltip("Distance in pixels off screen away the edge")]
	public int OffScreenDistance = 500;

	// Settings regarding the 2 speakers
	[Header("Characters")]
	[SerializeField] private CharacterData PlayerCharacterData;
	[Tooltip("Generic VN_Character GameObjects; There should be only 2 in a scene")]
	public List<VN_Character> CharacterObjects;
	[Tooltip("List of needed character data to pull from")]
	public List<CharacterData> AllCharacterData;

	// Objects needed to create story from JSON
	[Header("Required Objects")]
	[SerializeField]
	[Tooltip("Intermediate file for Unity to work with Ink; Created when a .ink file is saved in Unity")]
	private TextAsset inkJSONAsset = null;
	public Story story;

	// Textbox canvas objects
	// Canvas for text
	[SerializeField]
	private Canvas TextCanvas = null;
	// Canvas for the buttons
	[SerializeField]
	private Canvas ButtonCanvas = null;
	// Canvas for the characters' names
	[SerializeField]
	private Canvas NameCanvas = null;
	// Textbox text objects
	// Text object that displays text content
	private Text contentTextObj = null;
	// Text object that displays speaker name
	private Text nameTextObj = null;

	// UI Prefabs
	[SerializeField]
	[Tooltip("Used for VN text context")]
	private Text textPrefab = null;
	[SerializeField]
	[Tooltip("Used for VN buttons")]
	private Button buttonPrefab = null;

	// Internal References
	// Keep track of story creation event
	public static event Action<Story> OnCreateStory;
	// The content to be displayed
	private string currentLine = "";
	// The character who is currently speaking
	private VN_Character currentSpeaker = null;
	// The tags in effect on the current text
	private List<string> currentTags;
	// Whether or not the current text is done from slow text
	private bool currentTextDone = false;

	// Get parse results from corountine
	private string speaker;
	private string content;

	// Inky custom function calling
	const string FunctionCallString = ">>>";
	const char ArgumentDelimiter = '.';
	// Store/call funcitons in a dictionary https://stackoverflow.com/questions/4233536/c-sharp-store-functions-in-a-dictionary
	Dictionary<string, Delegate> AllCommands =
		new Dictionary<string, Delegate>();
	private List<ICmdCall> commandCalls;
	private List<ICmdFrame> commandFrames;
	private List<ICmdPart> commandParts;

	/* TODO Try to follow Single Responsibility Principle for this class
	 * Regions sort of map out responsibilities, but unclear as to
	 * what this class should be responsible for and how to separate
	 * currently tightly couple methods from this class
	*/

	/* TODO ? Make everything in VN loop a corountine so that function
	 * calls like the transition are finished before text is started
	 * to be shown; function calls are finished before another is called
	 * + Reset causes errors because subtracting happens too slow for fast
	 * reseting methods and next start story in this class
	 * + doing ">>> add.CharacterA, subtract.CharacterA" breaks because
	 * subtract is called when add isnt finished. Should result in behavior
	 * where Character A enters and then immediately leaves while locking out
	 * the VN from continuing until the sequence is done
	 * - Maybe a function option/make default for parallel function call so
	 * adding character isn't waited on to be finished before text is started
	 * to be shown
	*/

	//Functions involved directly in the frame-to-frame running of the sceen
	#region Unity gameloop

	// Called once upon initialization of the scene 
	// Adds the AddCharacter and SubtractCharacter functions to the AllCommands Dictionary
	void Awake()
    {
        // TODO replace with even high level factory?
        VN_Util Helper = new VN_Util(this);

		// TODO Replace getting interface implementers with something
		// more efficent
		// TODO Make ICmdCalls not implement MonoBehaviour (because they don't need it)
		commandCalls = FindObjectsOfType<MonoBehaviour>()
			.OfType<ICmdCall>().ToList();

		commandFrames = FindObjectsOfType<MonoBehaviour>()
			.OfType<ICmdFrame>().ToList();

		commandParts = FindObjectsOfType<MonoBehaviour>()
			.OfType<ICmdPart>().ToList();

		// TODO Make a class for initializing everything for command calls
		commandCalls.ForEach(command =>
		{
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

			command.Construct(this, newFrame, newPart);
			Func<List<string>, IEnumerator> newCommand = command.Command;

			AllCommands.Add(partString, newCommand);
		});
	}

	// Called once upon the scene being enabled
	// Removes the default message and begins the text
	void Start()
	{
		ClearContent();
        StartStory();
    }

    // Called once per frame
	// Skips the animation of text appearing if the spacebar is pressed
	void Update()
    {
		if (Input.GetKeyDown(KeyCode.Space)) SkipSlowText();
	}
	#endregion

	// Functions involved with managing the story on a high level
	#region Main Functions
	
	// Initializes a story based on the imported JSON file
	void StartStory()
	{
		story = new Story(inkJSONAsset.text);
		if (OnCreateStory != null) OnCreateStory(story);
		RefreshView();
	}

	// Remove all VN text & buttons, then starts displaying the text
	void RefreshView()
	{
		ClearContent();

		StartCoroutine(Co_DisplaySlowText());
	}
	#endregion

	// Functions involved in the text appearing on the screen
	#region Slow Text Functions

	// Displays the current section of the story
	/* TODO Fix text spasm when reaching edge of TextCanvas
	 * Current solution: 
	 * - Add method to adjust position of TextCanvas according to 
	 * currentContent.Length such that the text will be centered on 
	 * the screen when it is fully shown. 
	 * - Change TextCanvas width to be maximum.
	 * - Add field and functionality for maximum width in characters 
	 * until a new line character is added (replace auto text new line 
	 * from canvas that's probably causing the spasm with adding \n
	 * character if any line of content reaches maxLineLength)
	*/
	IEnumerator Co_DisplaySlowText()
    {
		currentTextDone = false;

		// Get the next line of text
		string nextLine = story.Continue();

		// Get tags
		currentTags = story.currentTags;

		// Parses the line for speakerName and sets currentLine to content
		yield return StartCoroutine(Co_ParseLine(nextLine));

		// Special case: Narrator is not a VN_Character and not in CharacterObjects
		if (speaker != "Narrator")
        {
			currentSpeaker = VN_Util.FindCharacterObj(
				VN_Util.FindCharacterData(speaker));
			// If valid speaker found, try changing emotion by tag
			if (currentSpeaker) tagChangeEmotion();
		}
		// Set currentLine to content
		currentLine = content;

		// If content is blank, skip making content
		if (currentLine == "")
		{
			if (story.canContinue)
            {
				StartCoroutine(Co_DisplaySlowText());
			}
            else
            {
				CreateRestartStoryButton();
            }
			yield break;
        }

		// Instantiate story content
		contentTextObj = CreateContentView("");
		// Instantiate character name text
		nameTextObj = CreateNameTextView(speaker);

		// Start displaying text content
		StartCoroutine(Co_SlowText(contentTextObj));
	}

	// Displays the current text on screen, one char at a time, then creates the choice buttons when it's done
	IEnumerator Co_SlowText(Text storyText)
	{
		foreach (char letter in currentLine.ToCharArray())
		{
			storyText.text += letter;
			// Delay appending more characters
			// 1/TextSpeed because WaitForSeconds(TextSpeed) is 1 char per x seconds
			// Convert to x char per second by inverting
			yield return new WaitForSeconds(1 / TextSpeed);
		}

		// Once done with showing text content, show all choice buttons
		CreateAllChoiceButtons();

		currentTextDone = true;
		yield break;
	}

	// Makes the remaining text appear instantly, then creates the choice buttons
	void SkipSlowText()
	{
		if (!currentTextDone)
		{
			currentTextDone = true;
			StopAllCoroutines();
			contentTextObj.text = currentLine;
			CreateAllChoiceButtons();
		}
	}
	#endregion

    // Methods to extract and/or edit data from lines of the story
	#region Line Reading
    
	// Changes the current emotion portrayed by the text
	void tagChangeEmotion()
    {
		// Check that there are any tags
		if (currentTags.Count > 0)
		{
			// Get first tag in currentTags
			string emotionTag = currentTags[0];
			currentSpeaker.ChangeSprite(emotionTag);
		}
	}

	/**
	* On a specified line (string of text), determines who the speaker is and what they're saying
	* 
	* @param line: The line being parsed
	* @return: tuple of 2 elements with element 0 being the speaker's name and 1 being what the speaker says
	*/
	IEnumerator Co_ParseLine(string line)
    {
		// Remove trailing and leading quotations, spaces, new line characters
		char[] toTrim = { (char)34, ' ', '\n' };
		// If line is in format "[character name]: [text to be spoken]"
		// lineSplit holds [character name] in index 0 and [text to be spoken] in index 1
		string[] lineSplit = line.Split(':');
		if (lineSplit.Length == 2)
		{
			// Trim removes any white space from the beginning or end.
			ParseResult(lineSplit[0].Trim(), lineSplit[1].Trim());
			yield break;
		}
		// Check if line is a function call
		else if (line.Length > 3 && line.Substring(0, 3) == FunctionCallString)
		{
			// (char)44 = ,
			string[] commands = line.Substring(3).Split((char)44);
			
			// Try to run all commands
			foreach (string rawCommand in commands)
			{
				var command = rawCommand.Trim(toTrim);
				// Assumes command is in form [function][ArgumentDelimiter][argument]
				// with only 1 argument
				string[] commandArray = command.Split(ArgumentDelimiter);
				List<string> commandList = new List<string>(commandArray);
				string function = commandList[0].Trim();
				// Assume all other strings after first (the function) are arguments
				List<string> arguments = commandList.GetRange(1, commandList.Count - 1);
				arguments.ForEach(arg => arg = arg.Trim(toTrim));

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
				
				// Can't say I understand but https://stackoverflow.com/questions/36184355/unity3d-return-value-with-a-delegate
				// helped get the return value of the delegate
				//Delegate[] invokeList = AllCommands[function].GetInvocationList();
				// Assume Delegate has only 1 method it invokes
				//bool commandResult = (bool)invokeList[0]
				//	.DynamicInvoke(arguments);
				//if (!commandResult) { Debug.LogError("Inky custom command call error at line " + inkyLineNumber); }
				// Was going to log error to show where in the Inky file the command error came from
				// but the inky sciprt line number is invisible to story.Continue()
			}
			ParseResult("Narrator", "");
			yield break;
		}
		else
        {
			// Assume player is speaking
			ParseResult(PlayerCharacterData.name, line.Trim(toTrim));
			yield break;
		}
	}

	void ParseResult(string first, string second)
    {
		speaker = first;
		content = second;
	}
    #endregion

    // Methods regarding player input in the story
	#region Player Control
    
	// Determines which choice the character selected and plays the corresponding text
	void OnClickChoiceButton(Choice choice)
	{
		story.ChooseChoiceIndex(choice.index);
		RefreshView();
	}
    #endregion

	// Methods to create certain parts of the visual novel
    #region Component Creation
    
	/**
	 * Creates a Text object of the content of a story
	 * 
	 * @param text: the content of the story
	 * @return: the Text object of the content
	 */
	Text CreateContentView(string text)
	{
		Text contentText = Instantiate(textPrefab);
		contentText.text = text;
		contentText.transform.SetParent(TextCanvas.transform, false);
		return contentText;
	}

	/**
	 * Creates a Text object of the name of a character
	 * 
	 * @param name: the name of the character
	 * @return: the Text object of the the character's name
	 */
	Text CreateNameTextView(string name)
    {
		Text nameText = Instantiate(textPrefab);
		nameText.transform.SetParent(NameCanvas.transform, false);
		// Update NameText
		if (name == "Narrator")
		{
			nameText.text = "";
			contentTextObj.fontStyle = FontStyle.Italic;
		}
		else
		{
			nameText.text = name;
		}

		return nameText;
	}

	/**
	 * Creates a buttons for a choice in a story
	 * 
	 * @param text: the text and choice for the button
	 * @return: the Button object for the choice
	 */
	Button CreateChoiceView(string text)
	{
		// Creates the button from a prefab
		Button choice = Instantiate(buttonPrefab) as Button;
		choice.transform.SetParent(ButtonCanvas.transform, false);

		// Gets the text from the button prefab
		Text choiceText = choice.GetComponentInChildren<Text>();
		choiceText.text = text;

        return choice;
	}

	// Creates a button to restart the story and resets the story if the button is clicked
	void CreateRestartStoryButton()
	{
		Button choice = CreateChoiceView("End of story.\nRestart?");
		choice.onClick.AddListener(delegate
		{
			StartCoroutine(ResetAll());
		});
	}

	// Creates all buttons correlating to a choice
	void CreateAllChoiceButtons()
    {
		// Display all the choices, if there are any!
		if (story.currentChoices.Count > 0)
		{
			for (int i = 0; i < story.currentChoices.Count; i++)
			{
				Choice choice = story.currentChoices[i];
				Button button = CreateChoiceView(choice.text.Trim());
				// Tell the button what to do when we press it
				button.onClick.AddListener(delegate
				{
					OnClickChoiceButton(choice);
				});
			}
		}
		// If there are no choices on this line of text
		// And add a button to continue
		else if (story.canContinue)
		{
			Button button = CreateChoiceView("Continue");
			button.onClick.AddListener(delegate {
				RefreshView();
			});
		}
		// If there is no more content, prompt to restart
		else
		{
			CreateRestartStoryButton();
		}
	}
	#endregion

	// Methods to clear and/or reset objects on the Visual Novel
	#region VN Clearing
	
	// Clears all text, buttons, and names from the Visul Novel
	void ClearContent()
	{
		// Reset all internal references
		contentTextObj = null;
		currentLine = null;
		currentSpeaker = null;
		currentTags = null;

		// Destroys all the children of all Canvases
		foreach (Transform child in TextCanvas.transform)
		{
			Destroy(child.gameObject);
		}

		foreach (Transform child in ButtonCanvas.transform)
		{
			Destroy(child.gameObject);
		}

		foreach (Transform child in NameCanvas.transform)
		{
			Destroy(child.gameObject);
		}
	}

	// Resets all characters in CharacterObjects

	// TODO broken due to this level not being all corountines
	IEnumerator ResetAll()
    {
		ClearContent();
		foreach (VN_Character charObj in CharacterObjects)
		{
			if (charObj.data != null)
			{
				yield return StartCoroutine(charObj.GetComponent<TeleportCharacterTransition>().Co_ExitScreen());
				charObj.ChangeSprite("");
				charObj.SetData(null);
			}
		}
		yield return new WaitForSeconds(3);
		StartStory();
	}
    #endregion

}