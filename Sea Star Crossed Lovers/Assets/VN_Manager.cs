﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using Ink.Runtime;

// Put this script as a component of the VN canvas
public class VN_Manager : MonoBehaviour
{
	// Settings
	// TextSpeed in characters per second
	public float TextSpeed = 10;

	// Keep track of story creation event
	public static event Action<Story> OnCreateStory;

	// Needed to create story from JSON
	[SerializeField]
	private TextAsset inkJSONAsset = null;
	public Story story;

	// Textbox objects
	[SerializeField]
	private Canvas TextCanvas = null;
	[SerializeField]
	private Canvas ButtonCanvas = null;

	// UI Prefabs
	[SerializeField]
	private Text textPrefab = null;
	[SerializeField]
	private Button buttonPrefab = null;


	// Internal
	// Used in slow text
	// What is left to be displayed by slow text
	private string RemainingContent = "";
	// The full line of text to be displayed
	private string CurrentLine = "";

	void Awake()
	{
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
		// Remove all the UI on screen
		ClearContent();

		DisplaySlow();
	}

	void DisplayAll()
    {
		// Read all the content until we can't continue any more
		while (story.canContinue)
		{
			// Continue gets the next line of the story
			string text = story.Continue();
			// This removes any white space from the text.
			text = text.Trim();
			// Display the text on screen!
			CreateContentView(text);
		}

		// Display all the choices, if there are any!
		if (story.currentChoices.Count > 0)
		{
			for (int i = 0; i < story.currentChoices.Count; i++)
			{
				Choice choice = story.currentChoices[i];
				Button button = CreateChoiceView(choice.text.Trim());
				// Tell the button what to do when we press it
				button.onClick.AddListener(delegate {
					OnClickChoiceButton(choice);
				});
			}
		}
		// If we've read all the content and there's no choices, the story is finished!
		else
		{
			Button choice = CreateChoiceView("End of story.\nRestart?");
			choice.onClick.AddListener(delegate {
				StartStory();
			});
		}
	}

	void DisplaySlow()
    {
		// Gets all text until choices
		CurrentLine = story.Continue();
		// This removes any white space from the text.
		CurrentLine = CurrentLine.Trim();
		RemainingContent = CurrentLine;

		// If blank line, skip trying to show
		if (CurrentLine == "")
		{
			DisplaySlow();
			return;
		}

		// Instantiate text box
		Text storyText = Instantiate(textPrefab);
		// Set parent in TextCanvas
		storyText.transform.SetParent(TextCanvas.transform, false);

		// Clear default text
		storyText.text = "";
		StartCoroutine(SlowText(storyText));
	}
	IEnumerator SlowText(Text storyText)
    {
		// Remove each character from RemainingContent and put into storyText
		// unitl there is no more RemainingContent
		while (RemainingContent != "")
		{
			storyText.text += RemainingContent[0];
			RemainingContent = RemainingContent.Remove(0, 1);
			yield return new WaitForSeconds(1/TextSpeed);
		}

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
	}
}
