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

	// Inky custom function calling
	const string FunctionCallString = ">>>";
	const char ArgumentDelimiter = '.';
	// Store/call funcitons in a dictionary https://stackoverflow.com/questions/4233536/c-sharp-store-functions-in-a-dictionary
	Dictionary<string, Delegate> AllCommands =
		new Dictionary<string, Delegate>();
	private List<ICommandCall> commandCalls;

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
        VN_HelperFunctions Helper = new VN_HelperFunctions(this);

		commandCalls = FindObjectsOfType<MonoBehaviour>()
			.OfType<ICommandCall>().ToList();

		commandCalls.ForEach(command =>
		{
			command.Construct(this);
			Func<List<string>, IEnumerator> newCommand = command.Command;
			string rawCommandName = command.GetType().ToString();
			string removeString = "Command";
			string commandName = rawCommandName.Remove(rawCommandName.IndexOf(removeString), removeString.Length);
            AllCommands.Add(commandName, newCommand);
        });

        foreach (var item in AllCommands)
        {
			print(item.Key);
			print(item.Value.GetType().ToString());
		}
		//AllCommands.Add("add", new Func<List<string>, bool>(AddCharacter));
		//AllCommands.Add("subtract", new Func<List<string>, bool>(SubtractCharacter));
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

		DisplaySlowText();
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
	void DisplaySlowText()
    {
		currentTextDone = false;

		// Get the next line of text
		string nextLine = story.Continue();

		// Get tags
		currentTags = story.currentTags;

		// Parses the line for speakerName and sets currentLine to content
		var (speaker, content) = ParseLine(nextLine);

		// Special case: Narrator is not a VN_Character and not in CharacterObjects
		if (speaker != "Narrator")
        {
			currentSpeaker = VN_HelperFunctions.FindCharacterObj(
				VN_HelperFunctions.FindCharacterData(speaker));
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
				DisplaySlowText();
			}
            else
            {
				CreateRestartStoryButton();
            }
            return;
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

	// The functions in AllCommands
	#region Function Calls
	
	/**
	* Assigns an available VN_Character the data of a specified character
	*
	* @param characterName: the character whom we are attempting to add
	* @return: whether or not the character could be added
	*/
	

	/**
	* Removes a specified VN_Character from CharacterObjects
	*
	* @param characterName: the character whom we are attempting to remove
	* @return: whether or not the character could be removed
	*/
	bool SubtractCharacter(List<string> characterNames)
	{
		bool result = true;

		if (characterNames.Count == 0)
		{
			Debug.LogError("Args error");
			return false;
		}

		foreach (string name in characterNames)
        {
			bool thisNameResult = false;

			CharacterData characterData = VN_HelperFunctions.FindCharacterData(name);
			// Search for VN_Character with matching data
			foreach (VN_Character charObj in CharacterObjects)
			{
				if (charObj.data == characterData)
				{
					// If found, transition out of screen, set default sprite, clear data
					//charObj.ExitScreen(characterData.moveTransition);
					thisNameResult = true;
					break;
				}
			}

			if (!thisNameResult)
			{
				result = false;
				Debug.LogError("No CharacterObjects with data of " + name);
			}
		}
		return result;
	}

	#endregion

	// Methods to find specified objects or data
	#region Finders

	/**
	* Finds the data corresponding to a specified character
	*
	* @param characterName: the name of the character we are getting the data from
	* @return: the data for the specified character
	*/


	/**
	* Finds a character corresponding to the given data
	*
	* @param data: the data of the character we are trying to find
	* @return: the character for the specified data
	*/
	
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
	(string, string) ParseLine(string line)
    {
		// Remove trailing and leading quotations, spaces, new line characters
		char[] toTrim = { (char)34, ' ', '\n' };
		// If line is in format "[character name]: [text to be spoken]"
		// lineSplit holds [character name] in index 0 and [text to be spoken] in index 1
		string[] lineSplit = line.Split(':');
		if (lineSplit.Length == 2)
		{
			// Trim removes any white space from the beginning or end.
			return (lineSplit[0].Trim(), lineSplit[1].Trim());
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

				print("function: " + function);

				Func<List<string>, IEnumerator> Co_Command =
					(Func<List<string>, IEnumerator>)AllCommands[function];

				StartCoroutine(Co_Command(arguments));
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
			return ("Narrator", "");
		}
		else
        {
			// Assume player is speaking
			return (PlayerCharacterData.name, line.Trim(toTrim));
		}
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
			StopAllCoroutines();
			ResetCharacterObjects();
			StartStory();
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
	void ResetCharacterObjects()
    {
		// Make all CharacterObjects teleport exit
		foreach (VN_Character charObj in CharacterObjects)
        {
			//charObj.StopCoroutines();
			//charObj.ExitScreen(CharacterData.MoveTransition.teleport);
		}
    }
    #endregion

}