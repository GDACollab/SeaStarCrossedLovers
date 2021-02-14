using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;

/// <summary>
/// Creates and manages VN components and flow
/// Based off of BasicInkExample.cs in Plugins/Ink/Example/Scripts
/// </summary>
public class VN_Manager : MonoBehaviour
{
	[Header("Settings")]
	[Tooltip("Speed of slow text in characters per second")]
	public float TextSpeed = 60;
	[Tooltip("Distance in pixels off screen away the edge")]
	public int OffScreenDistance = 500;

	[Header("VN_Character Related")]
	[SerializeField]
	private VN_Character PlayerCharacter;
	// List of characters in VN to pull from
	[SerializeField]
	private List<VN_Character> AllCharacters;
	

	[Header("Required Object References")]
	// Needed to create story from JSON
	[SerializeField]
	[Tooltip("Intermediate file for Unity to work with Ink; Created when a .ink file is saved in Unity")]
	private TextAsset inkJSONAsset = null;
	public Story story;

	// Textbox canvas objects
	[SerializeField]
	private Canvas TextCanvas = null;
	[SerializeField]
	private Canvas ButtonCanvas = null;
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

	// Internal
	// Keep track of story creation event
	public static event Action<Story> OnCreateStory;
	// The content to be displayed
	private string currentLine = "";
	private VN_Character currentSpeaker = null;
	private List<VN_Character> ActiveCharacters;
	private List<string> currentTags;
	private bool currentTextDone = false;
	
	void Start()
	{
		ActiveCharacters = new List<VN_Character>();

		// Remove the default message
		ClearContent();
        StartStory();
    }

    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Space)) SkipSlowText();
	}

    // Creates a new Story object with the compiled story which we can then play!
    void StartStory()
	{
		story = new Story(inkJSONAsset.text);
		if (OnCreateStory != null) OnCreateStory(story);
		RefreshView();
	}

	// The main function called every time the story changes
	void RefreshView()
	{
		// Remove all VN text & buttons
		ClearContent();

		DisplaySlow();
	}

	void DisplaySlow()
    {
		currentTextDone = false;
		// Get the next line of text
		string nextLine = story.Continue();

		// Get tags
		currentTags = story.currentTags;

		// Parses the line for speakerName and sets currentLine to content
		var (speaker, content) = ParseLine(nextLine);

		// Special case: Narrator is not a VN_Character and not in AllCharacters
		if (speaker != "Narrator")
        {
			currentSpeaker = FindCharacter(speaker);
			// If valid speaker found, try changing emotion by tag
			if (currentSpeaker) tagChangeEmotion();
		}
		// Set currentLine to content
		currentLine = content;

		// If content is blank, skip making content
		if (currentLine == "")
		{
			DisplaySlow();
			return;
		}

		// Temp add to ActiveCharacters
		// TODO change adding to swap CharacterData on existing
		// 2 generic GameObjects with VN_Character component
		AddCharacter(currentSpeaker);

		// Instantiate story content
		contentTextObj = CreateContentView("");
		// Instantiate character name text
		nameTextObj = CreateNameTextView(speaker);

		// Start displaying text content
		StartCoroutine(SlowText(contentTextObj));
	}

	IEnumerator SlowText(Text storyText)
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
		yield return true;
	}

	void AddCharacter(VN_Character character)
    {
		// Add to ActiveCharacters if not already in
		if (!ActiveCharacters.Contains(character))
		{
			ActiveCharacters.Add(character);
			// Enter transition character
			character.Transition(character.data.transition, CharacterData.TransitionDirection.enter);
		}
	}

	VN_Character FindCharacter(string characterName)
    {
		// Get currentSpeaker by finding speakerName in AllCharacters
		VN_Character character = AllCharacters.Find(x => x.name == characterName);

		// Catch character being null
		if (!character)
		{
			character = null;
			Debug.LogError("Character of name " + characterName + " could not be found");
		}
		return character;
	}

	void tagChangeEmotion()
    {
		// Check that there are any tags
		if (currentTags.Count > 0)
		{
			// Get first tag in currentTags
			string emotionTag = currentTags[0];
			currentSpeaker.changeSprite(emotionTag);
		}
	}

	void speakerChangeSprite()
    {
		// "happy" when currently talking, else "dafault"
		currentSpeaker.changeSprite("happy");

		// Change all other ActiveCharacters to default sprite
		foreach (VN_Character notTalking in ActiveCharacters)
		{
			if (notTalking != currentSpeaker)
			{
				notTalking.changeSprite("default");
			}
		}
	}

	// Tuple of 2 elements with element 0 being speakerName and 1 being line content
	(string, string) ParseLine(string line)
    {
		// If line is in format "[character name]: [text to be spoken]"
		// lineSplit holds [character name] in index 0 and [text to be spoken] in index 1
		string[] lineSplit = line.Split(':');
		if (lineSplit.Length == 2)
		{
			// Trim removes any white space from the beginning or end.
			return (lineSplit[0].Trim(), lineSplit[1].Trim());
		}
		// If there is no colon, assume player is speaking
		else
		{
			// Remove trailing and leading quotations, spaces, new line characters
			char[] toTrim = { (char)34, ' ', '\n' };
			return (PlayerCharacter.name, line.Trim(toTrim));
		}
	}

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

	// When we click the choice button, tell the story to choose that choice!
	void OnClickChoiceButton(Choice choice)
	{
		story.ChooseChoiceIndex(choice.index);
		RefreshView();
	}

	// Creates a textbox showing the the line of text
	Text CreateContentView(string text)
	{
		Text contentText = Instantiate(textPrefab);
		contentText.text = text;
		contentText.transform.SetParent(TextCanvas.transform, false);
		return contentText;
	}

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

	// Creates a button showing the choice text
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
			// If there is no more content, prompt to restart
		}
		// If there is no more story content, reset and star story again
		else
		{
			Button choice = CreateChoiceView("End of story.\nRestart?");
			choice.onClick.AddListener(delegate
			{
				ResetAllCharacters();
				StartStory();
			});
		}
	}

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

	void ResetAllCharacters()
    {
		ActiveCharacters.Clear();
		foreach (VN_Character character in AllCharacters)
        {
			// Change to default sprite
			character.changeSprite("default");
			// Teleport transition exit
			character.Transition(CharacterData.MoveTransition.teleport, CharacterData.TransitionDirection.exit);
		}
    }
}
