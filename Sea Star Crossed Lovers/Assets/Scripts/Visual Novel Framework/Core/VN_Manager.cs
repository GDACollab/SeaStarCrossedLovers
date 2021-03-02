using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEditor;

/// <summary>
/// Creates and manages VN components and flow
/// Based off of BasicInkExample.cs in Plugins/Ink/Example/Scripts
/// </summary>
public class VN_Manager : MonoBehaviour
{
	//I feel like I'm messing with a work of art here. Please feel free to rearrange this however it best fits.
	[Header("Transition Stuff (Unrelated to the things happening below)")]
	// The scene loader to use to load things
	[Tooltip("The scene loader to use to load things")]
	public SceneLoader activeLoader;

	// General settings for the textbox and appearance of text
	[Header("Settings")]
	[Tooltip("ActiveState of VN. Set before play to make VN appear or be hidden on start")]
	public ActiveState activeState = ActiveState.hidden;
	public enum ActiveState { hidden, active }
	[Tooltip("Distance of off screen characters in pixels away the screen edge")]
	public int OffScreenDistance = 500;
	[Tooltip("Max number of characters per line")]
	public int LineCharNum;
	// The different speeds the text can go in characters per second
	[Header("Text Speeds")]
	[Tooltip("Normal speed of text in characters per second. Used in normal dialouge")]
	public float normalSpeed = 60;
	[Tooltip("Speed of the text (in characters per second) during a pause. Used at punctuation marks")]
	public float pauseSpeed = 10;
	// Dictionary of the different speeds (Dictionaries are not serializable).
	private Dictionary<string, float> TextSpeeds;
	// List of characters the the text will pause at
	// TODO Put all settings/data for this manager into a scriptable object
	public static readonly List<float> PauseChars = new List<float>() { ',', '.', '…', '!', '?', '\n', '-', '–' };

	// Objects needed to create story from JSON
	[Header("Required Objects")]
	public Story story;
	[Tooltip("Intermediate file for Unity to work with Ink; Created when a .ink file is saved in Unity")]
	[SerializeField] private TextAsset inkJSONAsset = null;
	[Tooltip("CharacterData of the player in the VN")]
	[SerializeField] private CharacterData PlayerCharacterData;
	[Tooltip("Generic VN_Character GameObjects; There should be only 2 in a scene")]
	public List<VN_Character> CharacterObjects;
	[Tooltip("List of needed character data to pull from")]
	public List<CharacterData> AllCharacterData;

	// Internal References
	public RectTransform TextboxRectTransform;
	// Keep track of story creation event
	public static event Action<Story> OnCreateStory;
	// The content to be displayed
	private string currentLine = "";
	// The character who is currently speaking
	private VN_Character currentSpeaker = null;
	// The tags in effect on the current text
	private List<string> currentTags;

	// Subordinate classes
	private VN_CommandCall CommandCall;
	private VN_UIFactory UIFactory;

	// Flags/states
	// Whether or not the current text is done from slow text
	private bool currentTextDone = true;

	// Get parse results from corountine
	private string speaker;
	private string content;

	/* TODO Try to follow Single Responsibility Principle for this class
	 * Regions sort of map out responsibilities, but unclear as to
	 * what this class should be responsible for and how to separate
	 * currently tightly couple methods from this class
	*/

	/* TODO ? Make everything in VN loop a corountine
	 * - Maybe a function option for parallel function call so
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
		// Get CommandCall script in gameobject
		CommandCall = GetComponent<VN_CommandCall>();
		CommandCall.Construct(this);

		UIFactory = GetComponent<VN_UIFactory>();
		UIFactory.Construct(this);
		TextboxRectTransform = UIFactory.TextboxCanvas.GetComponent<RectTransform>();

		// Initialize the TextSpeeds Dictionary
		TextSpeeds = new Dictionary<string, float>{
			{"Normal", normalSpeed},
			{"Pause", pauseSpeed}
		};

		//Check to make sure all assigned text speeds are valid (i.e. speed > 0)
		foreach(float value in TextSpeeds.Values)
		{
			if(value <= 0) Debug.LogError("At least one speed value is not greater than 0. Please fix this in the inspector");
		}
	}

	// Called once upon the scene being enabled
	// Removes the default message and begins the text
	void Start()
	{
		ClearContent();
		UIFactory.CreateStartStoryButton();

		switch (activeState)
		{
			case ActiveState.hidden:
				float height = TextboxRectTransform.sizeDelta.y;
				TextboxRectTransform.anchoredPosition = new Vector2(0, -height);
				break;
			case ActiveState.active:
				break;
		}
	}
	void Update()
    {
		// Skips the animation of text appearing if the spacebar or primary mouse button is pressed
		// TODO replace with input system to not hard code input bindings
		if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0)) SkipSlowText();
	}
	#endregion

	// Functions involved with managing the story on a high level
	#region Main Functions

	// Initializes a story based on the imported JSON file
	public void StartStory()
	{
		story = new Story(inkJSONAsset.text);
		if (OnCreateStory != null) OnCreateStory(story);
		RefreshView();
	}

	// Remove all VN text & buttons, then starts displaying the text
	public void RefreshView()
	{
		ClearContent();

		StartCoroutine(Co_DisplaySlowText());
	}
	#endregion

	// Functions involved in the text appearing on the screen
	#region Slow Text Functions

	private IEnumerator Co_DisplaySlowText()
    {
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
				UIFactory.CreateRestartStoryButton();
            }
			yield break;
        }

		// Format text to have newline characters at porper locations
		currentLine = FormatText(currentLine);

		// Instantiate story content
		UIFactory.contentTextObj = UIFactory.CreateContentView("");
		// Instantiate character name text
		UIFactory.nameTextObj = UIFactory.CreateNameTextView(speaker);

		// Start displaying text content
		yield return StartCoroutine(Co_SlowText(UIFactory.contentTextObj));

		// Once done with showing text content, show all choice buttons
		UIFactory.CreateAllChoiceButtons();
		currentTextDone = true;
	}

	// Displays the current text on screen, one char at a time, then creates the choice buttons when it's done
	private IEnumerator Co_SlowText(Text storyText)
	{
		currentTextDone = false;
		float TextSpeed;

		foreach (char character in currentLine.ToCharArray())
		{
			storyText.text += character;
			
			if (PauseChars.Contains(character)) TextSpeed = TextSpeeds["Pause"];
			else TextSpeed = TextSpeeds["Normal"];

			// Delay appending more characters
			// 1/TextSpeed because WaitForSeconds(TextSpeed) is 1 char per x seconds
			// Convert to x char per second by inverting
			yield return new WaitForSeconds(1 / TextSpeed);
		}
	}

	private string FormatText(string text)
	{
		string result = "";
		int lineLength = 0;
		foreach (char character in text.ToCharArray())
		{
			if (lineLength > LineCharNum && character == ' ')
			{
				result += '\n';
				lineLength = 0;
			}
			else
			{
				if (character == '…')
                {
					result += "...";
				}
				else
                {
					result += character;
				}
				lineLength++;
			}
		}

		return result;
	}

	// Makes the remaining text appear instantly, then creates the choice buttons
	// TODO BUG: Trying to skip while function is running breaks VN
	private void SkipSlowText()
	{
		if (!currentTextDone)
		{
			currentTextDone = true;
			StopAllCoroutines();
			UIFactory.contentTextObj.text = currentLine;
			UIFactory.CreateAllChoiceButtons();
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
		// If line is in format "[character name]: [text to be spoken]"
		// lineSplit holds [character name] in index 0 and [text to be spoken] in index 1
		string[] lineSplit = line.Split(':');
		if (lineSplit.Length == 2)
		{
			// Trim removes any white space from the beginning or end.
			speaker = lineSplit[0].Trim(VN_Util.toTrim);
			content = lineSplit[1].Trim(VN_Util.toTrim);
			yield break;
		}
        // Check if line is a function call
		else
        {
			IEnumerator thisCommand = CommandCall.TryCommand(line);
			bool commandResult = thisCommand.MoveNext();
			// If line is a command;
			if(commandResult)
            {
				// Finish rest of command processing
				while(commandResult)
                {
					yield return thisCommand.Current;
					commandResult = thisCommand.MoveNext();
				}
				speaker = "Narrator";
				content = "";
			}
			else
            {
				// Assume player is speaking
				speaker = PlayerCharacterData.name;
				content = line.Trim(VN_Util.toTrim);
				yield break;
			}
		}
	}
    #endregion

	// Methods to clear and/or reset objects on the Visual Novel
	#region VN Clearing
	
	// Clears all text, buttons, and names from the Visul Novel
	void ClearContent()
	{
		// Reset all internal references
		UIFactory.contentTextObj = null;
		currentLine = null;
		currentSpeaker = null;
		currentTags = null;

		// Destroys all the children of all Canvases
		foreach (Transform child in UIFactory.TextCanvas.transform)
		{
			Destroy(child.gameObject);
		}

		foreach (Transform child in UIFactory.ButtonCanvas.transform)
		{
			Destroy(child.gameObject);
		}

		foreach (Transform child in UIFactory.NameCanvas.transform)
		{
			Destroy(child.gameObject);
		}
	}

	// Resets all characters in CharacterObjects
	public IEnumerator ResetAll()
    {
		ClearContent();
		foreach (VN_Character charObj in CharacterObjects)
		{
			if (charObj.data != null)
			{
				// HACK Make new teleport to temp replace transition
				CharacterTransition originalTransition = charObj.data.transition;
				TeleportCharacterTransition tempTeleport =
					(TeleportCharacterTransition)ScriptableObject.CreateInstance(typeof(TeleportCharacterTransition));
				charObj.data.transition = tempTeleport;

				// Do same thing as ExitPart
				yield return StartCoroutine(charObj.data.transition
					.Co_ExitScreen(charObj, this));
				charObj.data.transition = originalTransition;
				charObj.ChangeSprite("");
				charObj.SetData(null);
			}
		}
		yield return new WaitForSeconds(1);
		StartStory();
	}
	#endregion

	// Determines which choice the character selected and plays the corresponding text
	public void OnClickChoiceButton(Choice choice)
	{
		story.ChooseChoiceIndex(choice.index);
		RefreshView();
	}
}