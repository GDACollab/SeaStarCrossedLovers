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
	// Settings
	[Tooltip("Speed of slow text in characters per second")]
	public float TextSpeed = 10;
	public int OffScreenDistance = 500;

	// Keep track of story creation event
	public static event Action<Story> OnCreateStory;

	// Needed to create story from JSON
	[SerializeField]
	[Tooltip("Intermediate file for Unity to work with Ink; Created when a .ink file is saved in Unity")]
	private TextAsset inkJSONAsset = null;
	public Story story;

	// Textbox objects
	[SerializeField]
	private Canvas TextCanvas = null;
	[SerializeField]
	private Canvas ButtonCanvas = null;
	[SerializeField]
	private Canvas NameCanvas = null;
	private Text NameText;

	// UI Prefabs
	[SerializeField]
	[Tooltip("Used for VN text context")]
	private Text textPrefab = null;
	[SerializeField]
	[Tooltip("Used for VN buttons")]
	private Button buttonPrefab = null;

	[SerializeField]
	private VN_Character PlayerCharacter;
	// List of characters in VN to pull from
	[SerializeField]
	private List<VN_Character> AllCharacters;
	// List of characters in scene now
	private List<VN_Character> ActiveCharacters;


	// Internal
	// What is left to be displayed by slow text
	private string RemainingContent = "";
	// The full line of text to be displayed
	private string CurrentLine = "";
	private string CurrentSpeakerName = "";
	private VN_Character CurrentSpeaker = null;

	void Start()
	{
		ActiveCharacters = new List<VN_Character>();

		// Remove the default message
		ClearContent();
        StartStory();
    }

	// Creates a new Story object with the compiled story which we can then play!
	void StartStory()
	{
		story = new Story(inkJSONAsset.text);
		if (OnCreateStory != null) OnCreateStory(story);
		RefreshView();
	}

	// This is the main function called every time the story changes. It does a few things:
	// Destroys all the old content and choices.
	// Continues over all the lines of text, then displays all the choices. If there are no choices, the story is finished!
	void RefreshView()
	{
		// Remove all VN text & buttons
		ClearContent();

		DisplaySlow();
	}

	void DisplaySlow()
    {
		// Gets all text until choices
		CurrentLine = story.Continue();

		// If blank line, skip trying to show
		if (CurrentLine.Trim() == "")
		{
			DisplaySlow();
			return;
		}

		// If line is in format "[character name]: [text to be spoken]"
		// lineSplit holds [character name] in index 0 and [text to be spoken] in index 1
		string[] lineSplit = CurrentLine.Split(':');
		if (lineSplit.Length == 2)
        {
			// Trim removes any white space from the beginning or end.
			CurrentSpeakerName = lineSplit[0].Trim();
			CurrentLine = lineSplit[1].Trim();
		}
		// If there is no colon, assume player is speaking
		else
        {
			CurrentLine = CurrentLine.Trim('"');
			CurrentSpeakerName = PlayerCharacter.name;
		}
		// Keep track of reaming content to be displayed through slow text
		RemainingContent = CurrentLine;

		// Instantiate text box
		Text storyText = Instantiate(textPrefab);
		// Clear default text
		storyText.text = "";
		// Set parent in TextCanvas
		storyText.transform.SetParent(TextCanvas.transform, false);

		// Instantiate character name text
		NameText = Instantiate(textPrefab);
		NameText.transform.SetParent(NameCanvas.transform, false);
		// Update NameText
		if (CurrentSpeakerName == "Narrator")
        {
			NameText.text = "";
			storyText.fontStyle = FontStyle.Italic;
		}
		else
        {
			NameText.text = CurrentSpeakerName;
		}

		// Update character sprites
		// Get CurrentSpeaker by finding CurrentSpeakerName in AllCharacters
		VN_Character CurrentSpeaker = AllCharacters.Find(x => x.name == CurrentSpeakerName);

		// If found...
		if (CurrentSpeaker)
		{
			// Add to ActiveCharacters if not already in
			if (!ActiveCharacters.Contains(CurrentSpeaker))
            {
				ActiveCharacters.Add(CurrentSpeaker);
				// Enter transition character
				CurrentSpeaker.Transition(CurrentSpeaker.transition, VN_Character.TransitionDirection.enter);
			}
			// Change CurrentSpeaker to talking sprite
			CurrentSpeaker.changeSprite("talking");

			// Change all other ActiveCharacters to default sprite
			foreach (VN_Character notTalking in ActiveCharacters)
            {
				if(notTalking != CurrentSpeaker)
                {
					notTalking.changeSprite("default");
				}
			}
		}
		// Catch CurrentSpeaker being null
		// Ignore special Narrator case
		else if (CurrentSpeakerName != "Narrator")
        {
			Debug.LogError("CurrentSpeaker of name " + CurrentSpeakerName + " could not be found");
		}

		// Start displaying text content
		StartCoroutine(SlowText(storyText));
	}

	IEnumerator SlowText(Text storyText)
    {
		// Remove each character from RemainingContent and put into storyText
		// unitl there is no more RemainingContent
		while (RemainingContent != "")
		{
			// Append first character of RemainingContent to text display
			storyText.text += RemainingContent[0];
			// Remove first character in RemainingContent
			RemainingContent = RemainingContent.Remove(0, 1);
			// Delay appending more characters
			// 1/TextSpeed because WaitForSeconds(TextSpeed) is 1 char per x seconds
			// Convert to x char per second by inverting
			yield return new WaitForSeconds(1/TextSpeed);
		}

		// Once done with displaying all text content...
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
		// If there are no choices on this line of text, refresh for next line
		// And add a button to continue
		else if (story.canContinue)
		{
			Button button = CreateChoiceView("Continue");
			button.onClick.AddListener(delegate {
				RefreshView();
		});
		// If there is no more content, prompt to restart
		}
		else
		{
			Button choice = CreateChoiceView("End of story.\nRestart?");
			choice.onClick.AddListener(delegate
			{
				StartStory();
			});
		}
		yield return true;
	}

	void skipSlowText()
    {

    }

	// When we click the choice button, tell the story to choose that choice!
	void OnClickChoiceButton(Choice choice)
	{
		story.ChooseChoiceIndex(choice.index);
		RefreshView();
	}

	// Creates a textbox showing the the line of text
	void CreateContentView(string text)
	{
		Text storyText = Instantiate(textPrefab) as Text;
		storyText.text = text;
		storyText.transform.SetParent(TextCanvas.transform, false);
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

	// Destroys all the children of text and button canvases
	void ClearContent()
	{
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

  //  private void Reset()
  //  {
  //      foreach (VN_Character character in AllCharacters)
  //      {
		//	RectTransform characterTransform = character.rectTransform;
		//	switch (character.Side)
		//	{
		//		case VN_Character.ScreenSide.left:
		//			characterTransform.anchoredPosition = new Vector2(, 0);
		//			break;
		//		case VN_Character.ScreenSide.right:
		//			characterTransform.anchoredPosition = new Vector2(Screen.width / 2 - ScreenEdgeDistance, 0);
		//			break;
		//	}
		//	//OffScreenDistance

		//}
  //  }
}
